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
        public string InvoiceId { get; set; }
        public List<string> Entitlements { get; set; }
        public List<string> Errors { get; set; }
        public bool Successful { get; set; } = false;
        
    }
}
