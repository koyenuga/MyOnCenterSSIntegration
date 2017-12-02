using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Oncenter.BackOffice.Entities.Orders
{
    public class FulfillOrderResponse
    {
        public string AccountNumber { get; set; }
        public string SubscriptionNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public List<OrderEntitlement> Entitlements { get; set; }
    }
}
