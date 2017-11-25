using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Oncenter.BackOffice.Entities;
using Oncenter.BackOffice.Entities.Flexera;
using Oncenter.BackOffice.Entities.Zuora;
using Oncenter.BackOffice.Clients.Zuora;


namespace Oncenter.BackOffice.RestApi.Controllers
{
   
    public class SubscriptionController : ApiController
    {
         ZuoraClient zuoraClient = new ZuoraClient("kamar.oyenuga@oncenter.com", "@ncent3r@!", true);
        [Route("Account/{accountkey}/Subscriptions")]
        public Account Get(string accountkey)
        {
            
            var s = zuoraClient.GetSubscriptionDetails("A-S00002886");
            return new Account();
        }

        // GET: api/Subscription/5


        [Route("Account/Subscription/")]
        public Account  Post(Account account)
        {
            return new Account();
        }

        [Route("Subscription/Update")]
        public Account Put(Account account)
        {
            return new Account();
        }

        [Route("Subscription/Upgrade")]
        public Account Upgrade(Account account)
        {
            return new Account();
        }

        [Route("Subscription/Renew")]
        public Account Renew(Account account)
        {
            return new Account();
        }
        [Route("Subscription/{subscriptionId}/LicenseModel/{licenseModel}")]
        public Account LicenseModelChange(string subscriptionId, string licenseModel)
        {
            return new Account();
        }
        [Route("Subscription/License/Transfer")]
        public Account LicenseTransfer()
        {
            return new Account();
        }

         private void UpdateProductRateChargeDetails(List<ZuoraSubscriptionLineItem> lineItems)
        {
            foreach (var p in lineItems)
            {
                var pcrd = zuoraClient.GetProductRatePlanChargeDetails(p.ProductRatePlanChargeId);
                p.ProductRatePlanId = pcrd.ProductRatePlanId;
            }
        }

    }
}
