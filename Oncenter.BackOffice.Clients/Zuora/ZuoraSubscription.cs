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

        public void Create(FulfillOrderRequest request, FulfillOrderResponse response)
        {
            var resp = zuoraClient.CreateSubscription(request);
            response.AccountNumber = resp.AccountNumber;
            response.SubscriptionNumber = resp.SubscriptionNumber;
            response.InvoiceNumber = resp.InvoiceNumber;
        }
    }
}
