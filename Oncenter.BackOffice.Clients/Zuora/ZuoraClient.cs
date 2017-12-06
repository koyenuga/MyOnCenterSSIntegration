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
            dynamic zuoraAmendmentRequest = new ExpandoObject();
            zuoraAmendmentRequest.requests = new List<dynamic>();
            dynamic zuoraReq = new ExpandoObject();
            zuoraReq.AmendOptions = new ExpandoObject();
            zuoraReq.AmendOptions.GenerateInvoice = true;

            if (!string.IsNullOrWhiteSpace(request.Account.DefaultPaymentMethodId))
                zuoraReq.AmendOptions.ProcessPayments = true;
            zuoraReq.Amendments = new List<dynamic>();

            dynamic amendment = new ExpandoObject();
            amendment.SubscriptionId = existingSubscription.id;
            amendment.ContractEffectiveDate = request.Order.EffectiveDate.ToString("yyyy-MM-dd");
            //amendment.CurrentTerm = existingSubscription.currentTerm;
            //amendment.CurrentTermPeriodType = existingSubscription.currentTermPeriodType;
            amendment.Name = request.RequestType.ToString();
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

        public void CreateAccount(IAccount account)
        {
            IDictionary<string, object> zuoraAccount = new Dictionary<string, object>();
            zuoraAccount.Add("AccountNumber", account.AccountNumber);
            zuoraAccount.Add("Name", account.CompanyName);
            zuoraAccount.Add("BillCycleDay", account.BillCycleDay);
            var jsonParameter = JsonConvert.SerializeObject(zuoraAccount);
            string requestUrl = string.Format("{0}/v1/object/account", url);
            dynamic resp = ProcessRequest(requestUrl, Method.POST, jsonParameter);

            if (resp.Success)
                account.AccountNumber = resp.id;
            else
                throw new Exception("error");
        }
        public void CreateSubscription(IOrder subscription, string accountNumber)
        {
            dynamic zuoraSubscription = new ExpandoObject();
            zuoraSubscription.accountKey = accountNumber;
            zuoraSubscription.contractEffectiveDate = subscription.EffectiveDate;
            zuoraSubscription.renewalTerm = subscription.Term;
            zuoraSubscription.initialTerm = subscription.Term;
            zuoraSubscription.autoRenew = false;
            zuoraSubscription.subscribeToRatePlans = GetProductDictionary(subscription.LineItems);
            zuoraSubscription.termType = subscription.TermType;
            zuoraSubscription.runBilling = false;
            var jsonParameter = JsonConvert.SerializeObject(zuoraSubscription);
            //string requestUrl = string.Format("{0}/v1/action/subscribe", url);
            string requestUrl = string.Format("{0}v1/subscriptions", url);
            dynamic resp = JsonConvert.DeserializeObject(ProcessRequest(requestUrl, Method.POST, jsonParameter));

            if (resp.success == true)
            {
                subscription.Id = resp.subscriptionId;
                subscription.OrderNumber = resp.subscriptionNumber;
            }
            else
                throw new Exception("error");
        }
        public dynamic GetAccount(string accountKey)
        {
            string requestUrl = string.Format("{0}v1/accounts/{1}", url, accountKey);
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

            zuoraSubscription.BillToContact = new ExpandoObject();
            zuoraSubscription.BillToContact.FirstName = request.BillToContact.FirstName;
            zuoraSubscription.BillToContact.LastName = request.BillToContact.LastName;
            zuoraSubscription.BillToContact.WorkEmail = request.BillToContact.Email;
            zuoraSubscription.BillToContact.Address1 = request.BillToContact.Address;
            zuoraSubscription.BillToContact.City = request.BillToContact.City;
            zuoraSubscription.BillToContact.State = request.BillToContact.State;
            zuoraSubscription.BillToContact.PostalCode = request.BillToContact.ZipCode;
            zuoraSubscription.BillToContact.Country = request.BillToContact.Country;

            zuoraSubscription.SubscribeOptions = new ExpandoObject();
            zuoraSubscription.SubscribeOptions.GenerateInvoice = true;
            if (!string.IsNullOrWhiteSpace(request.Account.DefaultPaymentMethodId))
                zuoraSubscription.SubscribeOptions.ProcessPayments = true;

            zuoraSubscription.SubscriptionData = new ExpandoObject();
            zuoraSubscription.SubscriptionData.RatePlanData = GetProductRatePlanData(request.Order.LineItems);
            zuoraSubscription.SubscriptionData.Subscription = new ExpandoObject();
            zuoraSubscription.SubscriptionData.Subscription.AutoRenew = false;
            zuoraSubscription.SubscriptionData.Subscription.ContractEffectiveDate = request.Order.EffectiveDate.ToString("yyyy-MM-dd");
            zuoraSubscription.SubscriptionData.Subscription.InitialTerm = request.Order.Term;
            zuoraSubscription.SubscriptionData.Subscription.TermType = request.Order.TermType;
            zuoraSubscription.SubscriptionData.Subscription.RenewalTerm = request.Order.Term;
            
            zuoraSubscribeRequest.subscribes.Add(zuoraSubscription);
            var jsonParameter = JsonConvert.SerializeObject(zuoraSubscribeRequest);
            string requestUrl = string.Format("{0}/v1/action/subscribe", url);

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

            return resp;

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


        private string ProcessRequest(string url, Method requestType, object JsonParameter = null)
        {
            var client = new RestClient(url);
            var request = new RestRequest(requestType);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("apisecretaccesskey", Password);
            request.AddHeader("apiaccesskeyid", UserName);
            request.AddHeader("zuora-version", "211.0");

            if (JsonParameter != null)
                request.AddParameter("application/json", JsonParameter, ParameterType.RequestBody);



            IRestResponse response = client.Execute(request);
            return response.Content;
        }

        private List<dynamic> GetProductDictionary(List<IOrderLineItem> lineItems)
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

        private List<dynamic> GetProductRatePlanData(List<OrderLineItemRequest> lineItems, bool IsAmendment =false)
        {
            List<dynamic> ratePlanDatalist = new List<dynamic>();

            var RatePlanDataGroup = GroupProductRatePlanData(lineItems);

            foreach( var rateplandata in RatePlanDataGroup)
            {
               

                dynamic ratePlanDataObject = new ExpandoObject();
                ratePlanDataObject.RatePlan = new ExpandoObject();
               
                if (IsAmendment)
                    ratePlanDataObject.RatePlan.AmendmentSubscriptionRatePlanId = rateplandata.Key;
                else
                    ratePlanDataObject.RatePlan.ProductRatePlanId = rateplandata.Key;

                var chargeItemsList = rateplandata.Value;
                ratePlanDataObject.RatePlanChargeData = new List<dynamic>();

                foreach(var chargeItem in chargeItemsList)
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
