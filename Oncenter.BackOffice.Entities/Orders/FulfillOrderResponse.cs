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
        public string AccountId { get; set; }
        public string SubscriptionNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceId { get; set; }
        public string InvoiceTotalAmount { get; set; }
        public string InvoiceTaxAmount { get; set; }
        public string InvoiceBalance { get; set; }
        public List<EntitlementResponse> Entitlements { get; set; }
        public List<string> Errors { get; set; }
        public bool Successful { get; set; } = false;
        
    }

    public class EntitlementResponse
    {
       public string EntitlementFamily { get; set; }
       public List<string> Entitlements { get; set; }
    }
}
