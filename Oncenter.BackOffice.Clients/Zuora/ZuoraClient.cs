using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oncenter.BackOffice.Entities;
using Oncenter.BackOffice.Entities.Interfaces;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Json;
using RestSharp;
using Newtonsoft.Json;
using System.Dynamic;
using Oncenter.BackOffice.Entities.Orders;
using Oncenter.BackOffice.Entities.Contacts;
using Oncenter.BackOffice.Entities.Accounts;
using System.Collections;

namespace Oncenter.BackOffice.Clients.Zuora
{
    public class ZuoraClient : ZuoraOperation
    {
        public string ZuoraUrl { get => "https://rest.zuora.com/"; }
        public string ZuoraSandBoxUrl { get => "https://rest.apisandbox.zuora.com/"; }
        
        string url = string.Empty;
        public ZuoraClient(string userName, string password, string envUrl)
        {
            UserName = userName;
            Password = password;
            url = envUrl;
        }

        public dynamic AmendSubscription(FulfillOrderRequest request, dynamic existingSubscription)
        {
            if (request.RequestType == FulfillmentRequestType.Renewal)
                RenewSubscription(request, existingSubscription);

            dynamic zuoraAmendmentRequest = new ExpandoObject();
            zuoraAmendmentRequest.requests = new List<dynamic>();
            dynamic zuoraReq = new ExpandoObject();
            zuoraReq.AmendOptions = new ExpandoObject();
            zuoraReq.AmendOptions.GenerateInvoice = true;

            //if (!string.IsNullOrWhiteSpace(request.Account.DefaultPaymentMethodId))
            //    zuoraReq.AmendOptions.ProcessPayments = true;

            zuoraReq.Amendments = new List<dynamic>();

            dynamic amendment = new ExpandoObject();
            amendment.SubscriptionId = existingSubscription.id;
            amendment.ContractEffectiveDate = request.Order.EffectiveDate.ToString("yyyy-MM-dd");
            //amendment.CurrentTerm = existingSubscription.currentTerm;
            //amendment.CurrentTermPeriodType = existingSubscription.currentTermPeriodType;
            //amendment.Name = request.RequestType.ToString();
            amendment.RatePlanData = GetProductRatePlanData(request.Order.LineItems);
            //amendment.Status = "Completed";
            amendment.Type = "NewProduct";
            //amendment.RenewalSetting = "RENEW_WITH_SPECIFIC_TERM";


            zuoraReq.Amendments.Add(amendment);
            zuoraAmendmentRequest.requests.Add(zuoraReq);

            var jsonParameter = JsonConvert.SerializeObject(amendment);
            string requestUrl = string.Format("{0}v1/object/amendment", url);

            dynamic resp = JsonConvert.DeserializeObject(ProcessRequest(requestUrl, Method.POST, jsonParameter));


            var sErr = string.Empty;
            try
            {

                foreach (var err in resp.results[0].Errors)
                {
                    sErr += err.Code + " : " + err.Message + "<br/>" ;
                }

            }
            catch
            {

            }

            if (sErr.Length > 0)
                throw new Exception(sErr.ToString());

            return resp;

        }

        public dynamic GetSubscriptionDetails(string subscriptionId)
        {
            string requestUrl = string.Format("{0}v1/subscriptions/{1}", url, subscriptionId);
            return JsonConvert.DeserializeObject(ProcessRequest(requestUrl, Method.GET));


        }
        public dynamic GetSubscriptionDetailForAccount(string accountNumber)
        {
            if (string.IsNullOrWhiteSpace(accountNumber))
                return null;

            string requestUrl = string.Format("{0}v1/subscriptions/accounts/{1}", url, accountNumber);
            dynamic resp = JsonConvert.DeserializeObject(ProcessRequest(requestUrl, Method.GET));

            if (resp.success == true)
                return resp.subscriptions[0];

            return null;
        }


        public string GetDefaultCommunicationProfileId()
        {
            dynamic req = new ExpandoObject();
            req.queryString = "select id, ProfileName from CommunicationProfile where ProfileName = 'Silent Profile' ";
            var jsonParameter = JsonConvert.SerializeObject(req);
            string requestUrl = string.Format("{0}/v1/action/query", url);

            dynamic resp = JsonConvert.DeserializeObject(ProcessRequest(requestUrl, Method.POST, jsonParameter));

            var communicationProfileId = string.Empty;
            if (resp.done == true)
            {
                if (resp.records != null)
                    communicationProfileId = resp.records[0].Id;
            }

            return communicationProfileId;

        }

        public dynamic  CreateAccount(OncenterAccountRequest account, OncenterContact BillToContact, OncenterContact SoldToContact )
        {
            dynamic zuoraAccount = new ExpandoObject();

            zuoraAccount.Name = account.CompanyName;
            zuoraAccount.AccountNumber = account.AccountNumber;
            zuoraAccount.AutoPay = false;
            zuoraAccount.Batch = account.Batch;
            zuoraAccount.BillCycleDay = account.BillCycleDay;
            zuoraAccount.Currency = account.Currency;
            zuoraAccount.PaymentTerm = account.PaymentTerm;
            zuoraAccount.Status = "Draft";
            zuoraAccount.InvoiceDeliveryPrefsEmail = account.InvoiceDeliveryByEmail;
            zuoraAccount.InvoiceDeliveryPrefsPrint = account.InvoiceDeliveryByPrint;

            if (account.IsTaxExempt)
            {
                zuoraAccount.TaxExemptStatus = "Yes";
                zuoraAccount.TaxExemptCertificateID = "0000000000";
            }

            var jsonParameter = JsonConvert.SerializeObject(zuoraAccount);
            string requestUrl = string.Format("{0}v1/object/account", url);
            dynamic resp = JsonConvert.DeserializeObject(ProcessRequest(requestUrl, Method.POST, jsonParameter));

            var sErr = string.Empty;
            try
            {

                foreach (var err in resp.results[0].Errors)
                {
                    sErr += err.Code + " : " + err.Message + "<br/>";
                }

            }
            catch
            {

            }

            if (sErr.Length > 0)
                throw new Exception(sErr.ToString());

            string billToId = string.Empty;
            string soldToId = string.Empty;
            
            if (string.IsNullOrWhiteSpace(BillToContact.Id))
               billToId = CreateContact(resp.Id.ToString(), BillToContact);
            else
                billToId = BillToContact.Id;

            if (SoldToContact != null)
            {
                if (string.IsNullOrWhiteSpace(SoldToContact.Id))
                    soldToId = CreateContact(resp.Id.ToString(), SoldToContact);
                else
                    soldToId = SoldToContact.Id;
            }
            else
            {
                soldToId = BillToContact.Id;
            }


            UpdateAccount(resp.Id.ToString(), soldToId, billToId, "Active");

            return resp.Id.ToString();
        }
        public dynamic CreateAccount(string accountNumber, string companyName, bool IsTaxExempt, OncenterContact BillToContact)
        {
            dynamic zuoraAccount = new ExpandoObject();

            zuoraAccount.Name = companyName;
            zuoraAccount.AccountNumber = accountNumber;
            zuoraAccount.AutoPay = false;
            zuoraAccount.Batch = "Batch1";
            zuoraAccount.BillCycleDay = "1";
            zuoraAccount.Currency = "USD";
            zuoraAccount.PaymentTerm = "Due On Receipt";
            zuoraAccount.Status = "Draft";
            zuoraAccount.InvoiceDeliveryPrefsEmail = true;
            zuoraAccount.InvoiceDeliveryPrefsPrint = true;
            zuoraAccount.CommunicationProfileId = GetDefaultCommunicationProfileId();

            if (IsTaxExempt)
            {
                zuoraAccount.TaxExemptStatus = "Yes";
                zuoraAccount.TaxExemptCertificateID = "0000000000";
            }

            var jsonParameter = JsonConvert.SerializeObject(zuoraAccount);
            string requestUrl = string.Format("{0}v1/object/account", url);
            dynamic resp = JsonConvert.DeserializeObject(ProcessRequest(requestUrl, Method.POST, jsonParameter));

            var sErr = string.Empty;
            try
            {

                foreach (var err in resp.results[0].Errors)
                {
                    sErr += err.Code + " : " + err.Message + "<br/>";
                }

            }
            catch
            {

            }

            if (sErr.Length > 0)
                throw new Exception(sErr.ToString());

            string billToId = string.Empty;
          

            if (string.IsNullOrWhiteSpace(BillToContact.Id))
                billToId = CreateContact(resp.Id.ToString(), BillToContact);
            else
                billToId = BillToContact.Id;


            UpdateAccount(resp.Id.ToString(), string.Empty, billToId, "Active");

            return resp.Id.ToString();
        }
        public string CreateContact(string accountId, OncenterContact contact)
        {
            dynamic ZuoraContact = new ExpandoObject();

            ZuoraContact.AccountId = accountId;
            ZuoraContact.FirstName = contact.FirstName;
            ZuoraContact.LastName = contact.LastName;
            ZuoraContact.WorkEmail = contact.Email;
            ZuoraContact.Address1 = contact.Address;
            ZuoraContact.City = contact.City;
            ZuoraContact.State = contact.State;
            ZuoraContact.PostalCode = contact.ZipCode;
            ZuoraContact.Country = contact.Country;

            var jsonParameter = JsonConvert.SerializeObject(ZuoraContact);
            string requestUrl = string.Format("{0}v1/object/contact", url);
            dynamic resp = JsonConvert.DeserializeObject(ProcessRequest(requestUrl, Method.POST, jsonParameter));

            var sErr = string.Empty;
            try
            {

                foreach (var err in resp.results[0].Errors)
                {
                    sErr += err.Code + " : " + err.Message + "<br/>";
                }

            }
            catch
            {

            }

            if (sErr.Length > 0)
                throw new Exception(sErr.ToString());

            return resp.Id;
        }

        void UpdateAccount(string accountId, string soldToId, string billToId, string accountStatus)
        {
            dynamic zuoraAccount = new ExpandoObject();

            zuoraAccount.BillToId = billToId;

            if (!string.IsNullOrWhiteSpace(soldToId))
                zuoraAccount.SoldToId = soldToId;
            else
                zuoraAccount.SoldToId = billToId;

            zuoraAccount.Status = accountStatus;

            var jsonParameter = JsonConvert.SerializeObject(zuoraAccount);
            string requestUrl = string.Format("{0}v1/object/account/{1}", url, accountId);
            dynamic resp = ProcessRequest(requestUrl, Method.PUT, jsonParameter);


        }
        void UpdateAccountSoldTo(string accountId, string soldToId)
        {
            dynamic zuoraAccount = new ExpandoObject();

        
            zuoraAccount.SoldToId = soldToId;
         
            var jsonParameter = JsonConvert.SerializeObject(zuoraAccount);
            string requestUrl = string.Format("{0}v1/object/account/{1}", url, accountId);
            dynamic resp = ProcessRequest(requestUrl, Method.PUT, jsonParameter);


        }

        void UpdateAccountNetsuiteIntegrationId(string accountId, string netsuiteIntegrationId)
        {
            dynamic zuoraAccount = new ExpandoObject();


            zuoraAccount.IntegrationId__NS = netsuiteIntegrationId;

            var jsonParameter = JsonConvert.SerializeObject(zuoraAccount);
            string requestUrl = string.Format("{0}v1/object/account/{1}", url, accountId);
            dynamic resp = ProcessRequest(requestUrl, Method.PUT, jsonParameter);


        }
        void UpdateInvoiceNetsuiteIntegrationId(string Id, string netsuiteIntegrationId)
        {
            dynamic zuoraInvoice = new ExpandoObject();


            zuoraInvoice.IntegrationId__NS = netsuiteIntegrationId;

            var jsonParameter = JsonConvert.SerializeObject(zuoraInvoice);
            string requestUrl = string.Format("{0}v1/object/invoice/{1}", url, Id);
            dynamic resp = ProcessRequest(requestUrl, Method.PUT, jsonParameter);


        }
        public dynamic CreateSubscription(IOrderRequest subscription, string accountNumber)
        {
            var order = new Order();
            dynamic zuoraSubscription = new ExpandoObject();
            zuoraSubscription.accountKey = accountNumber;
            zuoraSubscription.contractEffectiveDate = subscription.EffectiveDate.ToString("yyyy-MM-dd");
            zuoraSubscription.renewalTerm = subscription.Term;
            zuoraSubscription.initialTerm = subscription.Term;
            zuoraSubscription.autoRenew = false;
            zuoraSubscription.subscribeToRatePlans = GetProductDictionary(subscription.LineItems);
            zuoraSubscription.termType = subscription.TermType;
            zuoraSubscription.runBilling = true;
            zuoraSubscription.targetDate = DateTime.Now.ToString("yyyy-MM-dd");
            zuoraSubscription.termStartDate = DateTime.Now.ToString("yyyy-MM-dd");

            if (!string.IsNullOrWhiteSpace(subscription.InvoiceOwnerAccountNumber))
                zuoraSubscription.invoiceOwnerAccountKey = subscription.InvoiceOwnerAccountNumber;

            var jsonParameter = JsonConvert.SerializeObject(zuoraSubscription);
            //string requestUrl = string.Format("{0}/v1/action/subscribe", url);
            string requestUrl = string.Format("{0}v1/subscriptions", url);
            dynamic resp = JsonConvert.DeserializeObject(ProcessRequest(requestUrl, Method.POST, jsonParameter));

            if (resp.success == true)
            {
                order.OrderNumber = resp.subscriptionNumber;
               
            }
            else
                throw new Exception("error");

            return resp;
        }
        public dynamic GetAccount(string accountKey)
        {
            string requestUrl = string.Format("{0}v1/accounts/{1}", url, accountKey);
            return JsonConvert.DeserializeObject(ProcessRequest(requestUrl, Method.GET));
        }
        //public dynamic GetAccountId(string accountNumber)
        //{
        //    string requestUrl = string.Format("{0}v1/accounts/{1}", url, accountNumber);
        //    dynamic resp =  JsonConvert.DeserializeObject(ProcessRequest(requestUrl, Method.GET));

        //    if (resp.success)
        //    {
        //        if(resp.basicInfo != null)
        //            return resp.basicInfo.id;
        //    }

                
        //}
        public dynamic GetAccountById(string accountId)
        {
            string requestUrl = string.Format("{0}v1/object/account/{1}", url, accountId);
            return JsonConvert.DeserializeObject(ProcessRequest(requestUrl, Method.GET));
        }

        public dynamic GetInvoiceDetails(string invoiceId)
        {
            string requestUrl = string.Format("{0}v1/object/invoice/{1}", url, invoiceId);
            return JsonConvert.DeserializeObject(ProcessRequest(requestUrl, Method.GET));
        }

        public dynamic CreateSubscription(FulfillOrderRequest request)
        {
           
            dynamic zuoraSubscribeRequest = new ExpandoObject();
            zuoraSubscribeRequest.subscribes = new List<dynamic>();

            dynamic zuoraSubscription = new ExpandoObject();
            zuoraSubscription.Account = new ExpandoObject();

            zuoraSubscription.Account.Name = request.Account.CompanyName;
            zuoraSubscription.Account.AccountNumber = request.Account.AccountNumber;
            zuoraSubscription.Account.AutoPay = false;
            zuoraSubscription.Account.Batch = request.Account.Batch;
            zuoraSubscription.Account.BillCycleDay = request.Account.BillCycleDay;
            zuoraSubscription.Account.Currency = request.Account.Currency;
            zuoraSubscription.Account.PaymentTerm = request.Account.PaymentTerm;
            zuoraSubscription.Account.Status = request.Account.Status;
            zuoraSubscription.Account.CommunicationProfileId = GetDefaultCommunicationProfileId();
            zuoraSubscription.Account.IntegrationId__NS = request.Account.NetsuiteIntegrationId;
            if (request.Account.IsTaxExempt)
            {
                zuoraSubscription.Account.TaxExemptStatus = "Yes";
                zuoraSubscription.Account.TaxExemptCertificateID = "000000000";
                zuoraSubscription.Account.TaxExemptEffectiveDate = DateTime.Now.ToString("yyyy-MM-dd");
            }
            zuoraSubscription.BillToContact = new ExpandoObject();
            zuoraSubscription.BillToContact.FirstName = request.BillToContact.FirstName;
            zuoraSubscription.BillToContact.LastName = request.BillToContact.LastName;
            zuoraSubscription.BillToContact.WorkEmail = request.BillToContact.Email;
            zuoraSubscription.BillToContact.Address1 = request.BillToContact.Address;
            zuoraSubscription.BillToContact.City = request.BillToContact.City;
            zuoraSubscription.BillToContact.State = request.BillToContact.State;
            zuoraSubscription.BillToContact.PostalCode = request.BillToContact.ZipCode;
            zuoraSubscription.BillToContact.Country = request.BillToContact.Country;
            if (request.SoldToContact != null)
            {
                zuoraSubscription.SoldToContact = new ExpandoObject();
                zuoraSubscription.SoldToContact.FirstName = request.SoldToContact.FirstName;
                zuoraSubscription.SoldToContact.LastName = request.SoldToContact.LastName;
                zuoraSubscription.SoldToContact.WorkEmail = request.SoldToContact.Email;
                zuoraSubscription.SoldToContact.Address1 = request.SoldToContact.Address;
                zuoraSubscription.SoldToContact.City = request.SoldToContact.City;
                zuoraSubscription.SoldToContact.State = request.SoldToContact.State;
                zuoraSubscription.SoldToContact.PostalCode = request.SoldToContact.ZipCode;
                zuoraSubscription.SoldToContact.Country = request.SoldToContact.Country;

            }

            zuoraSubscription.SubscribeOptions = new ExpandoObject();
            zuoraSubscription.SubscribeOptions.GenerateInvoice = true;
          
            zuoraSubscription.SubscriptionData = new ExpandoObject();
            zuoraSubscription.SubscriptionData.RatePlanData = GetProductRatePlanData(request.Order.LineItems);
            zuoraSubscription.SubscriptionData.Subscription = new ExpandoObject();
            zuoraSubscription.SubscriptionData.Subscription.AutoRenew = request.Order.AutoRenew;
            zuoraSubscription.SubscriptionData.Subscription.ContractEffectiveDate = request.Order.EffectiveDate.ToString("yyyy-MM-dd");
            zuoraSubscription.SubscriptionData.Subscription.InitialTerm = request.Order.Term;
            zuoraSubscription.SubscriptionData.Subscription.TermType = request.Order.TermType;
            zuoraSubscription.SubscriptionData.Subscription.RenewalTerm = request.Order.Term;
            

            if (!string.IsNullOrWhiteSpace(request.Order.InvoiceOwnerAccountNumber))
            {
                dynamic invoiceOwnerAccount = GetAccount(request.Order.InvoiceOwnerAccountNumber);
                if (invoiceOwnerAccount.basicInfo == null)
                {
                    zuoraSubscription.SubscriptionData.Subscription.InvoiceOwnerId = CreateAccount(request.Order.InvoiceOwnerAccountNumber,
                        request.Order.InvoiceOwnerCompanyName, request.Account.IsTaxExempt, request.BillToContact);
                }
                else
                    zuoraSubscription.SubscriptionData.Subscription.InvoiceOwnerId = invoiceOwnerAccount.basicInfo.id;

                zuoraSubscription.SubscribeOptions.GenerateInvoice = false;

            }
           

            zuoraSubscribeRequest.subscribes.Add(zuoraSubscription);
            var jsonParameter = JsonConvert.SerializeObject(zuoraSubscribeRequest);
            string requestUrl = string.Format("{0}/v1/action/subscribe", url);

            dynamic resp = JsonConvert.DeserializeObject(ProcessRequest(requestUrl, Method.POST, jsonParameter));

            dynamic response = new ExpandoObject();
            response.Errors = new List<string>();


            if (resp[0].Success == true)
            {
                response.AccountNumber = resp[0].AccountNumber;
                response.AccountId = resp[0].AccountId;
                dynamic accountDetail = GetAccountById(resp[0].AccountId.ToString());
                response.SubscriptionNumber = resp[0].SubscriptionNumber;
                if (!string.IsNullOrWhiteSpace(request.Order.InvoiceOwnerAccountNumber))
                {
                    dynamic invoiceRq = new ExpandoObject();
                    invoiceRq.objects = new ExpandoObject[1];
                    invoiceRq.objects[0] = new ExpandoObject();
                    invoiceRq.objects[0].AccountId = zuoraSubscription.SubscriptionData.Subscription.InvoiceOwnerId;
                    invoiceRq.objects[0].InvoiceDate = DateTime.Now.ToString("yyyy-MM-dd");
                    invoiceRq.objects[0].TargetDate = DateTime.Now.ToString("yyyy-MM-dd");
                    invoiceRq.type = "Invoice";

                    jsonParameter = JsonConvert.SerializeObject(invoiceRq);
                    requestUrl = string.Format("{0}v1/action/generate", url);
                    resp = JsonConvert.DeserializeObject(ProcessRequest(requestUrl, Method.POST, jsonParameter));

                    if (resp[0].Success == true)
                        response.InvoiceId = resp[0].Id;

                }
                else
                {
                    
                    response.InvoiceId = resp[0].InvoiceId;
                }
                response.BillToId = accountDetail.BillToId;
                response.SoldToId = accountDetail.SoldToId;

                if (!string.IsNullOrEmpty(response.InvoiceId.ToString()))
                {
                    dynamic inv = GetInvoiceDetails(response.InvoiceId.ToString());
                    response.InvoiceNumber = inv.InvoiceNumber;
                    response.TotalAmount = inv.Amount;
                    response.Tax = inv.TaxAmount;
                    response.Balance = inv.Balance;

                    if (!string.IsNullOrWhiteSpace(request.Order.InvoiceNetsuiteIntegrationId))
                        UpdateInvoiceNetsuiteIntegrationId(response.InvoiceId.ToString(), request.Order.InvoiceNetsuiteIntegrationId);
                }
                else
                {
                    response.TotalAmount = "";
                    response.Tax = "";
                    response.Balance = "";
                }

            }
            else
            {
                foreach (var err in resp[0].Errors)
                {
                    response.Errors.Add(err.Code.ToString() + " : " + err.Message.ToString());
                }
            }





            return response;

        }

        public dynamic UpdateSubscription(FulfillOrderRequest request, dynamic existingSubscription)
        {
            if (request.RequestType == FulfillmentRequestType.Renewal)
                return RenewSubscription(request, existingSubscription);

            request.Order.SubscriptionNumber = existingSubscription.subscriptionNumber;
            dynamic zuoraSubscribeRequest = new ExpandoObject();

            dynamic zuoraSubscription = new ExpandoObject();
            zuoraSubscription.add = new List<dynamic>();
      
            foreach (var item in request.Order.LineItems)
            {
               
               
                var ratePlanId = string.Empty;
                var ratePlanChargeId = string.Empty;
               

                var ratePlan = GetProductRatePlanChargeDetails(item.ProductRatePlanChargeId);
                item.ProductRatePlanId = ratePlan.ProductRatePlanId.ToString();

                dynamic newItem = new ExpandoObject();
                newItem.contractEffectiveDate = item.EffectiveDate.ToString("yyyy-MM-dd");
                newItem.productRatePlanId = item.ProductRatePlanId;
                newItem.chargeOverrides = new List<dynamic>();

                dynamic chargeOverrideItem = new ExpandoObject();

                if (item.IsDiscountLineItem)
                    chargeOverrideItem.discountAmount = item.Amount;
                else
                {
                    chargeOverrideItem.price = item.Amount;
                    chargeOverrideItem.quantity = item.Quantity;
                }
                

                chargeOverrideItem.productRatePlanChargeId = item.ProductRatePlanChargeId;
               

                newItem.chargeOverrides.Add(chargeOverrideItem);
                zuoraSubscription.add.Add(newItem);
                
            }
            zuoraSubscription.invoice = true;
            zuoraSubscription.invoiceTargetDate = DateTime.Now.ToString("yyyy-MM-dd");
            var jsonParameter = JsonConvert.SerializeObject(zuoraSubscription);
            string requestUrl = string.Format("{0}v1/subscriptions/{1}", url, request.Order.SubscriptionNumber);

            dynamic resp = JsonConvert.DeserializeObject(ProcessRequest(requestUrl, Method.PUT, jsonParameter, "207.0"));
            dynamic response = new ExpandoObject();
            response.Errors = new List<string>();
            var sErr = string.Empty;
            try
            {

                foreach (var err in resp.results[0].Errors)
                {
                    response.Errors.Add(err.Code + " : " + err.Message);
                }

            }
            catch
            {

            }
            
            response.AccountNumber = request.Account.AccountNumber;
            response.SubscriptionNumber = request.Order.SubscriptionNumber;
            response.InvoiceNumber = string.Empty;
            response.AccountId = existingSubscription.accountId;
            response.InvoiceId = resp.invoiceId;
            dynamic accountDetail = GetAccountById(existingSubscription.accountId.ToString());
            response.BillToId = accountDetail.BillToId;
            response.SoldToId = accountDetail.SoldToId;

            if (resp.invoiceId != null)
            {
                dynamic inv = GetInvoiceDetails(resp.invoiceId.ToString());
                response.TotalAmount = inv.Amount;
                response.Tax = inv.TaxAmount;
                response.Balance = inv.Balance;

                if (!string.IsNullOrWhiteSpace(request.Order.InvoiceNetsuiteIntegrationId))
                    UpdateInvoiceNetsuiteIntegrationId(resp.invoiceId.ToString(), request.Order.InvoiceNetsuiteIntegrationId);
            }
            else
            {
                response.TotalAmount = "";
                response.Tax = "";
                response.Balance = "";
            }

            return response;

        }

        public dynamic RenewSubscription(FulfillOrderRequest request, dynamic existingSubscription)
        {

            dynamic renewalRq = new ExpandoObject();
            renewalRq.requests = new ExpandoObject[1];
            renewalRq.requests[0] = new ExpandoObject();
            renewalRq.requests[0].AmendOptions = new ExpandoObject();
            renewalRq.requests[0].AmendOptions.GenerateInvoice = true;
            renewalRq.requests[0].AmendOptions.InvoiceProcessingOptions = new ExpandoObject();
            renewalRq.requests[0].AmendOptions.InvoiceProcessingOptions.InvoiceDate = DateTime.Now.ToString("yyyy-MM-dd");
            renewalRq.requests[0].AmendOptions.InvoiceProcessingOptions.InvoiceTargetDate = request.Order.EffectiveDate.ToString("yyyy-MM-dd");
            renewalRq.requests[0].Amendments = new ExpandoObject[2];
            renewalRq.requests[0].Amendments[0] = new ExpandoObject();
            renewalRq.requests[0].Amendments[0].RenewalTerm = request.Order.Term;
            renewalRq.requests[0].Amendments[0].Name = "Subscription Renewal";
            renewalRq.requests[0].Amendments[0].RenewalSetting = "RENEW_WITH_SPECIFIC_TERM";
            renewalRq.requests[0].Amendments[0].EffectiveDate = request.Order.EffectiveDate.ToString("yyyy-MM-dd");
            renewalRq.requests[0].Amendments[0].Status = "Completed";
            renewalRq.requests[0].Amendments[0].SubscriptionId = existingSubscription.id;
            renewalRq.requests[0].Amendments[0].TermType = "TERMED";
            renewalRq.requests[0].Amendments[0].Type = "Renewal";
            renewalRq.requests[0].Amendments[0].ContractEffectiveDate = request.Order.EffectiveDate.ToString("yyyy-MM-dd");

            renewalRq.requests[0].Amendments[1] = new ExpandoObject();
            renewalRq.requests[0].Amendments[1].Name = "Subscription Renewal Products";
            renewalRq.requests[0].Amendments[1].EffectiveDate = request.Order.EffectiveDate.ToString("yyyy-MM-dd");
            renewalRq.requests[0].Amendments[1].Status = "Completed";
            renewalRq.requests[0].Amendments[1].SubscriptionId = existingSubscription.id;
            renewalRq.requests[0].Amendments[1].RatePlanData = GetProductRatePlanData(request.Order.LineItems);
            renewalRq.requests[0].Amendments[1].TermType = "TERMED";
            renewalRq.requests[0].Amendments[1].Type = "NewProduct";
            renewalRq.requests[0].Amendments[1].ContractEffectiveDate = request.Order.EffectiveDate.ToString("yyyy-MM-dd");

            var jsonParameter = JsonConvert.SerializeObject(renewalRq);
            string requestUrl = string.Format("{0}v1/action/amend", url);

            dynamic resp = JsonConvert.DeserializeObject(ProcessRequest(requestUrl, Method.POST, jsonParameter));
            //if(resp.Success == true)
            //{
            //    dynamic renewal = new ExpandoObject();
            //    renewal.invoice = true;
            //    renewal.invoiceTargetDate = request.Order.EffectiveDate.ToString("yyyy-MM-dd");
            //    jsonParameter = JsonConvert.SerializeObject(renewal);
            //    requestUrl = string.Format("{0}v1/subscriptions/{1}/renew", url, amendment.SubscriptionId);
            //    resp = JsonConvert.DeserializeObject(ProcessRequest(requestUrl, Method.PUT, jsonParameter, "207.0"));
            //}
            dynamic response = new ExpandoObject();
            response.Errors = new List<string>();
            var sErr = string.Empty;
            try
            {

                foreach (var err in resp.results[0].Errors)
                {
                    response.Errors.Add(err.Code + " : " + err.Message);
                }

            }
            catch
            {

            }

            response.AccountNumber = request.Account.AccountNumber;
            response.SubscriptionNumber = request.Order.SubscriptionNumber;
            response.InvoiceNumber = string.Empty;
            response.AccountId = existingSubscription.accountId;
            response.InvoiceId = resp.invoiceId;


            if (resp.invoiceId != null)
            {
                dynamic inv = GetInvoiceDetails(resp.invoiceId.ToString());
                response.TotalAmount = inv.Amount;
                response.Tax = inv.TaxAmount;
                response.Balance = inv.Balance;
                if (!string.IsNullOrWhiteSpace(request.Order.InvoiceNetsuiteIntegrationId))
                    UpdateInvoiceNetsuiteIntegrationId(resp[0].InvoiceId, request.Order.InvoiceNetsuiteIntegrationId);
            }
            else
            {
                response.TotalAmount = "";
                response.Tax = "";
                response.Balance = "";
            }

            return response;

        }

        public dynamic PostPayment( OrderPaymentRequest request)
        {
            dynamic zuoraPayment = new ExpandoObject();
            zuoraPayment.AccountId = request.AccountId;
            zuoraPayment.Amount = request.Amount;
            zuoraPayment.AppliedCreditBalanceAmount = request.AppliedCreditBalanceAmount;
            zuoraPayment.AppliedInvoiceAmount = request.AppliedInvoiceAmount;
            zuoraPayment.EffectiveDate = request.EffectiveDate.ToString("yyyy-MM-dd");
            zuoraPayment.InvoiceId = request.InvoiceId;
            zuoraPayment.PaymentMethodId = request.PaymentMethodId;
            zuoraPayment.Status = request.Status;
            zuoraPayment.Type = request.Type;

            var jsonParameter = JsonConvert.SerializeObject(zuoraPayment);
            string requestUrl = string.Format("{0}v1/object/payment", url);

            dynamic resp = JsonConvert.DeserializeObject(ProcessRequest(requestUrl, Method.POST, jsonParameter));

            dynamic response = new ExpandoObject();
            response.Errors = new List<string>();

            if (resp.Success = true)
            {
                response.PaymentId = resp.Id;
                response.Successful = true;
            }
            else
            {
                foreach (var err in resp.results[0].Errors)
                {
                    response.Errors.Add(err.Code + " : " + err.Message);
                }
                response.Successful = false;
            }

            
            return response;
        }
        public dynamic GetProductRatePlanChargeDetails(string productRatePlanChargeId)
        {
            string requestUrl = string.Format("{0}v1/object/product-rate-plan-charge/{1}", url, productRatePlanChargeId);
            return JsonConvert.DeserializeObject(ProcessRequest(requestUrl, Method.GET));

        }

       
        public dynamic GetAccountSubscriptions(string accountNumber)
        {
            string requestUrl = string.Format("{0}v1/subscriptions/accounts/{1}", url, accountNumber);
            return JsonConvert.DeserializeObject(ProcessRequest(requestUrl, Method.GET));
        }

        public string SaveAttachement(string entity, string description, string id, string fileName, byte[] file, string contentType, string attachmentId = "")
        {
            string url = string.Empty;

            url = string.IsNullOrWhiteSpace(attachmentId) ? string.Format(ZuoraUrl + "v1/attachments/?description={1}&associatedObjectType={2}&associatedObjectKey={3}", description, entity, id) :
               string.Format(url + "v1/attachments/{0}", attachmentId);

            var client = new RestClient(url);
            var request = new RestRequest(Method.POST);

            if (attachmentId.Trim() == string.Empty)
                request = new RestRequest(Method.PUT);

            request.AddHeader("content-type", "application/json");
            request.AddHeader("apiaccesskeyid", UserName);
            request.AddHeader("apisecretaccesskey", Password);


            request.AddFile(fileName, file, fileName, contentType);

            IRestResponse response = client.Execute(request);
            dynamic resp = JsonConvert.DeserializeObject(response.Content);

            return resp.Id;

        }


        

        private List<dynamic> GetProductDictionary(List<OrderLineItemRequest> lineItems)
        {
            List<dynamic> zuoraProductRateCharges = new List<dynamic>();

            foreach (var item in lineItems)
            {
                dynamic chargeOverrides = new ExpandoObject();

                chargeOverrides.quantity = item.Quantity;
                if (item.Amount > 0)
                {
                    if (item.IsDiscountLineItem)
                        chargeOverrides.discountAmount = item.Amount;
                    else
                        chargeOverrides.price = item.Amount;
                }

                chargeOverrides.productRatePlanChargeId = item.ProductRatePlanChargeId;

                dynamic listCharge = new ExpandoObject();
                var prd = GetProductRatePlanChargeDetails(
                    item.ProductRatePlanChargeId);
                listCharge.productRatePlanId = prd.ProductRatePlanId;
                listCharge.chargeOverrides = new List<dynamic>();
                listCharge.chargeOverrides.Add(chargeOverrides);
                zuoraProductRateCharges.Add(listCharge);
            }
            return zuoraProductRateCharges;
        }
        private List<dynamic> GetProductRatePlanData(List<OrderLineItemRequest> lineItems, bool IsAmendment = false)
        {
            List<dynamic> ratePlanDatalist = new List<dynamic>();

            var RatePlanDataGroup = GroupProductRatePlanData(lineItems);

            foreach (var rateplandata in RatePlanDataGroup)
            {


                dynamic ratePlanDataObject = new ExpandoObject();
                ratePlanDataObject.RatePlan = new ExpandoObject();

                if (IsAmendment)
                    ratePlanDataObject.RatePlan.AmendmentSubscriptionRatePlanId = rateplandata.Key;
                else
                    ratePlanDataObject.RatePlan.ProductRatePlanId = rateplandata.Key;

                var chargeItem = rateplandata.Value;
                ratePlanDataObject.RatePlanChargeData = new List<dynamic>();

                dynamic ratePlanChargeItem = new ExpandoObject();
                ratePlanChargeItem.RatePlanCharge = new ExpandoObject();
                ratePlanChargeItem.RatePlanCharge.ProductRatePlanChargeId = chargeItem.ProductRatePlanChargeId;
                ratePlanChargeItem.RatePlanCharge.Quantity = chargeItem.Quantity;

                if (chargeItem.IsDiscountLineItem)
                    ratePlanChargeItem.RatePlanCharge.DiscountAmount = chargeItem.Amount;
                else
                    ratePlanChargeItem.RatePlanCharge.Price = chargeItem.Amount;

                ratePlanChargeItem.RatePlanCharge.EffectiveStartDate = chargeItem.EffectiveDate;
                ratePlanChargeItem.RatePlanCharge.EffectiveEndDate = chargeItem.ExpirationDate;

                ratePlanDataObject.RatePlanChargeData.Add(ratePlanChargeItem);


                ratePlanDatalist.Add(ratePlanDataObject);

            }

            return ratePlanDatalist;

        }
        private List<dynamic> GetProductRatePlanData1(List<OrderLineItemRequest> lineItems)
        {
            List<dynamic> ratePlanDatalist = new List<dynamic>();

            var RatePlanDataGroup = GroupProductRatePlanData(lineItems);
         
            foreach ( var rateplandata in RatePlanDataGroup)
            {

                dynamic ratePlanChargeItem = new ExpandoObject();

                //if (IsAmendment)
                //    ratePlanDataObject.RatePlan.AmendmentSubscriptionRatePlanId = rateplandata.Key;
                //else
                //    ratePlanDataObject.RatePlan.ProductRatePlanId = rateplandata.Key;

                var chargeItem = rateplandata.Value;
                ratePlanChargeItem.productRatePlanId = rateplandata.Key;

               
                    ratePlanChargeItem.chargeOverrides = new List<dynamic>();
                    dynamic overrideItem = new ExpandoObject();

                    overrideItem.productRatePlanChargeId = chargeItem.ProductRatePlanChargeId;
                    overrideItem.quantity = chargeItem.Quantity;

                    if (chargeItem.Amount > 0)
                    {
                        if (chargeItem.IsDiscountLineItem)
                            overrideItem.discountAmount = chargeItem.Amount;
                        else
                            overrideItem.price = chargeItem.Amount;
                    }
                       

                    ratePlanChargeItem.chargeOverrides.Add(overrideItem);
                    
                

                ratePlanDatalist.Add(ratePlanChargeItem);

            }

            return ratePlanDatalist;

        }

        public Dictionary<string, OrderLineItemRequest> GroupProductRatePlanData(List<OrderLineItemRequest> lineItems)
        {
            var RatePlanGroup = new Dictionary<string, OrderLineItemRequest>();
          
            foreach (var item in lineItems)
            {
                var ratePlan = GetProductRatePlanChargeDetails(item.ProductRatePlanChargeId);
                OrderLineItemRequest prd = new OrderLineItemRequest();
                if(RatePlanGroup.TryGetValue(ratePlan.ProductRatePlanId.ToString(), out prd))
                {
                    prd.Quantity += item.Quantity;
                    prd.Amount += item.Amount;
                }
                else
                    RatePlanGroup.Add(ratePlan.ProductRatePlanId.ToString(), item);
               
            }

            return RatePlanGroup;
            
        }

       
        
    }
}
