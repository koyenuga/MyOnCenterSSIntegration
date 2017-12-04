using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oncenter.BackOffice.Entities.Interfaces;
using Oncenter.BackOffice.Entities.Orders;

namespace Oncenter.BackOffice.Clients.Zuora
{
    public class ZuoraSubscription : ISubscription
    {
        ZuoraClient zuoraClient;

        public ZuoraSubscription(string userName, string password, string endPointUrl)
        {
            zuoraClient = new ZuoraClient(userName, password, endPointUrl);
        }

        public dynamic Create(FulfillOrderRequest request)
        {
            return zuoraClient.CreateSubscription(request);
           
        }

        public dynamic Amend(FulfillOrderRequest request, dynamic existingSubscription)
        {
            return zuoraClient.AmendSubscription(request, existingSubscription);
        }

        public dynamic Get(string accountNumber)
        {
            return zuoraClient.GetSubscriptionDetailForAccount(accountNumber);
        }
    }
}
