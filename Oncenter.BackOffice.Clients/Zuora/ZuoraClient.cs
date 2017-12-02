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

namespace Oncenter.BackOffice.Clients.Zuora
{
    public class ZuoraClient
    {
        public string ZuoraUrl { get => "https://rest.zuora.com/"; }
        public string ZuoraSandBoxUrl { get => "https://rest.apisandbox.zuora.com/"; }
        public string UserName { get; set; }
        public string Password { get; set; }

        string url = string.Empty;
        public ZuoraClient(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }
        public ZuoraClient(string userName, string password, bool test)
        {
            UserName = userName;
            Password = password;
            if (test)
                url = ZuoraSandBoxUrl;
            else
                url = ZuoraUrl;
        }

        public dynamic GetSubscriptionDetails(string subscriptionId)
        {
            string requestUrl = string.Format("{0}v1/subscriptions/{1}", url, subscriptionId);
            return JsonConvert.DeserializeObject(ProcessRequest(requestUrl, Method.GET));


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

        public void FulfillSubscription(FulfillOrderRequest request, FulfillOrderResponse response)
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
            zuoraSubscription.SubscriptionData.Subscription.ContractEffectiveDate = request.Order.EffectiveDate;
            zuoraSubscription.SubscriptionData.Subscription.InitialTerm = request.Order.Term;
            zuoraSubscription.SubscriptionData.Subscription.TermType = request.Order.TermType;

            var jsonParameter = JsonConvert.SerializeObject(zuoraSubscription);
            string requestUrl = string.Format("{0}/v1/action/subscribe", url);

            dynamic resp = JsonConvert.DeserializeObject(ProcessRequest(requestUrl, Method.POST, jsonParameter));


            if (resp.success == true)
            {
                response.AccountNumber = resp.AccountNumber;
                response.SubscriptionNumber = resp.SubscriptionNumber;
                response.InvoiceNumber = resp.InvoiceNumber;

            }
            else
            {
                StringBuilder s = new StringBuilder();
                foreach( var err in resp.Errors)
                {
                    s.AppendLine(err.Code + " : " + err.Message);
                }
                throw new Exception(s.ToString());
            }

            

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

        private List<dynamic> GetProductRatePlanData(List<IOrderLineItemRequest> lineItems)
        {
            List<dynamic> ratePlanDatalist = new List<dynamic>();

            var RatePlanDataGroup = GroupProductRatePlanData(lineItems);

            foreach( var rateplandata in RatePlanDataGroup)
            {
                dynamic ratePlanDataObject = new ExpandoObject();
                ratePlanDataObject.RatePlan = new ExpandoObject();
                ratePlanDataObject.RatePlan.ProductRatePlanId = rateplandata.Key;

                var chargeItemsList = rateplandata.Value;
                ratePlanDataObject.RatePlanChargeData = new List<dynamic>();

                foreach(var chargeItem in chargeItemsList)
                {
                    dynamic ratePlanChargeItem = new ExpandoObject();
                    ratePlanChargeItem.RatePlanCharge = new ExpandoObject();
                    ratePlanChargeItem.RatePlanCharge.ProductRatePlanChargeId = chargeItem;
                    ratePlanDataObject.RatePlanChargeData.Add(ratePlanChargeItem);
                }

                ratePlanDatalist.Add(ratePlanDataObject);

            }

            return ratePlanDatalist;

        }

        public Dictionary<string, List<string>> GroupProductRatePlanData(List<IOrderLineItemRequest> lineItems)
        {
            var RatePlanGroup = new Dictionary<string, List<string>>();
            List<string> rateChargePlans;
            foreach (var item in lineItems)
            {
                var ratePlan = GetProductRatePlanChargeDetails(item.ProductRatePlanChargeId);
                if(!RatePlanGroup.ContainsKey(ratePlan.ProductRatePlanId))
                {
                    rateChargePlans = new List<string>();
                    rateChargePlans.Add(item.ProductRatePlanChargeId);
                    RatePlanGroup.Add(ratePlan.ProductRatePlanId, rateChargePlans);
                }
                else
                {
                    rateChargePlans = RatePlanGroup[ratePlan.ProductRatePlanId];
                    rateChargePlans.Add(item.ProductRatePlanChargeId);
                }
            }

            return RatePlanGroup;
            
        }

    }
}
