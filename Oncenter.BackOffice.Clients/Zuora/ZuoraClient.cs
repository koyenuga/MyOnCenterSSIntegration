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
    public class ZuoraClient
    {
        public string ZuoraUrl { get => "https://rest.zuora.com/"; }
        public string ZuoraSandBoxUrl { get => "https://rest.apisandbox.zuora.com/"; }
        public string UserName { get; set; }
        public string Password { get; set; }

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

        public dynamic  CreateAccount(IAccountRequest account, OncenterContact BillToContact, OncenterContact SoldToContact )
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
                zuoraAccount.TaxExemptCertificateID = account.TaxCertificateID;
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

            return GetAccountById(resp.Id.ToString());
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
            zuoraAccount.SoldToId = soldToId;
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
            //var account =  CreateAccount(request.Account, request.BillToContact, request.SoldToContact);
            //var subscription = CreateSubscription(request.Order, account.AccountNumber.ToString());


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
            zuoraSubscription.Account.communicationProfileId = request.Account.CommunicationProfileId;
            zuoraSubscription.Account.IntegrationId__NS = request.Account.NetsuiteIntegrationId;

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
            //if (!string.IsNullOrWhiteSpace(request.Account.DefaultPaymentMethodId))
            //    zuoraSubscription.SubscribeOptions.ProcessPayments = true;

            zuoraSubscription.SubscriptionData = new ExpandoObject();
            zuoraSubscription.SubscriptionData.RatePlanData = GetProductRatePlanData(request.Order.LineItems);
            zuoraSubscription.SubscriptionData.Subscription = new ExpandoObject();
            zuoraSubscription.SubscriptionData.Subscription.AutoRenew = request.Order.AutoRenew;
            zuoraSubscription.SubscriptionData.Subscription.ContractEffectiveDate = request.Order.EffectiveDate.ToString("yyyy-MM-dd");
            zuoraSubscription.SubscriptionData.Subscription.InitialTerm = request.Order.Term;
            zuoraSubscription.SubscriptionData.Subscription.TermType = request.Order.TermType;
            zuoraSubscription.SubscriptionData.Subscription.RenewalTerm = request.Order.Term;

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
                response.SubscriptionNumber = resp[0].SubscriptionNumber;
                response.InvoiceNumber = resp[0].InvoiceNumber;
                response.InvoiceId = resp[0].InvoiceId;

                if (resp[0].InvoiceId != null)
                {
                    dynamic inv = GetInvoiceDetails(resp[0].InvoiceId.ToString());
                    response.TotalAmount = inv.Amount;
                    response.Tax = inv.TaxAmount;
                    response.Balance = inv.Balance;
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
            zuoraSubscription.update = new List<dynamic>();
          
            foreach (var item in request.Order.LineItems)
            {
               
                var itemExist = false;
                var ratePlanId = string.Empty;
                var ratePlanChargeId = string.Empty;
                var qty = 0;

                var ratePlan = GetProductRatePlanChargeDetails(item.ProductRatePlanChargeId);
                item.ProductRatePlanId = ratePlan.ProductRatePlanId.ToString();

                foreach (var xItem in existingSubscription.ratePlans)
                {
                    if (xItem.productRatePlanId.ToString() == item.ProductRatePlanId)
                    {
                        foreach(var cItem in xItem.ratePlanCharges)
                        {
                            if(cItem.productRatePlanChargeId.ToString().Trim() == item.ProductRatePlanChargeId.Trim())
                            {
                                if (!item.IsPerpetualLicense)
                                {
                                    itemExist = true;
                                    ratePlanId = xItem.id.ToString();
                                    ratePlanChargeId = cItem.id.ToString();
                                    qty = cItem.quantity;
                                }
                            }
                        }
                    }
                }

                if(itemExist)
                {
                    dynamic updateItem = new ExpandoObject();
                    updateItem.contractEffectiveDate = item.EffectiveDate.ToString("yyyy-MM-dd");
                    updateItem.ratePlanId = ratePlanId;
                    updateItem.chargeUpdateDetails = new List<dynamic>();
                   
                    dynamic chargeUpdateItem = new ExpandoObject();

                    if (item.Price > 0)
                        chargeUpdateItem.price = item.Price;

                    chargeUpdateItem.ratePlanChargeId = ratePlanChargeId;
                    chargeUpdateItem.quantity = qty + item.Quantity;

                    updateItem.chargeUpdateDetails.Add(chargeUpdateItem);
                    zuoraSubscription.update.Add(updateItem);
                }
                else
                {
                    dynamic newItem = new ExpandoObject();
                   newItem.contractEffectiveDate = item.EffectiveDate.ToString("yyyy-MM-dd");
                   newItem.productRatePlanId = item.ProductRatePlanId;
                   newItem.chargeOverrides  = new List<dynamic>();
                   
                    dynamic chargeOverrideItem = new ExpandoObject();

                    if (item.Price > 0)
                        chargeOverrideItem.price = item.Price;

                    chargeOverrideItem.productRatePlanChargeId = item.ProductRatePlanChargeId;
                    chargeOverrideItem.quantity = item.Quantity;

                    newItem.chargeOverrides.Add(chargeOverrideItem);
                    zuoraSubscription.add.Add(newItem);

                }
            }
            zuoraSubscription.invoice = true;
        
            //zuoraSubscription.status = "Active";
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


            if (resp.invoiceId != null)
            {
                dynamic inv = GetInvoiceDetails(resp.invoiceId.ToString());
                response.TotalAmount = inv.Amount;
                response.Tax = inv.TaxAmount;
                response.Balance = inv.Balance;
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
           

            dynamic amendment = new ExpandoObject();
            amendment.SubscriptionId = existingSubscription.id;
            amendment.ContractEffectiveDate = request.Order.EffectiveDate.ToString("yyyy-MM-dd");
            amendment.Name = "Subscription Renewal";
            amendment.RatePlanData = GetProductRatePlanData(request.Order.LineItems);
            amendment.Type = "Renewal";
            amendment.TermStartDate = request.Order.EffectiveDate.ToString("yyyy-MM-dd");
            amendment.TermType = "TERMED";
            amendment.RenewalTerm = "TermsAndConditions";
            amendment.RenewalSetting = "RENEW_WITH_SPECIFIC_TERM";
            amendment.RenewalTerm = string.IsNullOrEmpty(request.Order.Term)? "12" : request.Order.Term;

         

            var jsonParameter = JsonConvert.SerializeObject(amendment);
            string requestUrl = string.Format("{0}v1/object/amendment", url);

            dynamic resp = JsonConvert.DeserializeObject(ProcessRequest(requestUrl, Method.POST, jsonParameter));
            if(resp.Success == true)
            {
                dynamic renewal = new ExpandoObject();
                renewal.invoice = true;
                renewal.invoiceTargetDate = request.Order.EffectiveDate.ToString("yyyy-MM-dd");
                jsonParameter = JsonConvert.SerializeObject(renewal);
                requestUrl = string.Format("{0}v1/subscriptions/{1}/renew", url, amendment.SubscriptionId);
                resp = JsonConvert.DeserializeObject(ProcessRequest(requestUrl, Method.PUT, jsonParameter, "207.0"));
            }
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


        private string ProcessRequest(string url, Method requestType, object JsonParameter = null, string version= "211.0")
        {
            var client = new RestClient(url);
            var request = new RestRequest(requestType);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("apisecretaccesskey", Password);
            request.AddHeader("apiaccesskeyid", UserName);
            request.AddHeader("zuora-version", version);

            if (JsonParameter != null)
                request.AddParameter("application/json", JsonParameter, ParameterType.RequestBody);



            IRestResponse response = client.Execute(request);
            return response.Content;
        }

        private List<dynamic> GetProductDictionary(List<OrderLineItemRequest> lineItems)
        {
            List<dynamic> zuoraProductRateCharges = new List<dynamic>();

            foreach (var item in lineItems)
            {
                dynamic chargeOverrides = new ExpandoObject();

                chargeOverrides.quantity = item.Quantity;
                if (item.Price > 0)
                    chargeOverrides.price = item.Price;

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

                var chargeItemsList = rateplandata.Value;
                ratePlanDataObject.RatePlanChargeData = new List<dynamic>();

                foreach (var chargeItem in chargeItemsList)
                {
                    dynamic ratePlanChargeItem = new ExpandoObject();
                    ratePlanChargeItem.RatePlanCharge = new ExpandoObject();
                    ratePlanChargeItem.RatePlanCharge.ProductRatePlanChargeId = chargeItem.ProductRatePlanChargeId;
                    ratePlanChargeItem.RatePlanCharge.Quantity = chargeItem.Quantity;
                    ratePlanDataObject.RatePlanChargeData.Add(ratePlanChargeItem);
                }

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

                var chargeItemsList = rateplandata.Value;
                ratePlanChargeItem.productRatePlanId = rateplandata.Key;

                foreach (var chargeItem in chargeItemsList)
                {
                    
                    ratePlanChargeItem.chargeOverrides = new List<dynamic>();
                    dynamic overrideItem = new ExpandoObject();

                    overrideItem.productRatePlanChargeId = chargeItem.ProductRatePlanChargeId;
                    overrideItem.quantity = chargeItem.Quantity;

                    if (chargeItem.Price > 0)
                        overrideItem.price = chargeItem.Price;

                    ratePlanChargeItem.chargeOverrides.Add(overrideItem);
                    
                }

                ratePlanDatalist.Add(ratePlanChargeItem);

            }

            return ratePlanDatalist;

        }

        public Dictionary<string, List<OrderLineItemRequest>> GroupProductRatePlanData(List<OrderLineItemRequest> lineItems)
        {
            var RatePlanGroup = new Dictionary<string, List<OrderLineItemRequest>>();
            List<OrderLineItemRequest> rateChargePlans;
            foreach (var item in lineItems)
            {
                var ratePlan = GetProductRatePlanChargeDetails(item.ProductRatePlanChargeId);
                if(!RatePlanGroup.ContainsKey(ratePlan.ProductRatePlanId.ToString()))
                {
                    rateChargePlans = new List<OrderLineItemRequest>();
                    rateChargePlans.Add(item);
                    RatePlanGroup.Add(ratePlan.ProductRatePlanId.ToString(), rateChargePlans);
                }
                else
                {
                    rateChargePlans = RatePlanGroup[ratePlan.ProductRatePlanId.ToString()];
                    rateChargePlans.Add(item);
                }
            }

            return RatePlanGroup;
            
        }

       
        
    }
}
