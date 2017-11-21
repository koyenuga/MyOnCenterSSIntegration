using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Oncenter.BackOffice.Entities;
using Oncenter.BackOffice.Entities.Flexera;
using Oncenter.BackOffice.Entities.Zuora;


namespace Oncenter.BackOffice.RestApi.Controllers
{
    public class SubscriptionController : ApiController
    {
        [Route("Account/{accountkey}/Subscriptions")]
        public Account Get(string accountkey)
        {
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


    }
}
