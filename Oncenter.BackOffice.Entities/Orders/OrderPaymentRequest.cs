using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oncenter.BackOffice.Entities.Orders
{
    public class OrderPaymentRequest
    {
        public string AccountId { get; set; }
        public double Amount { get; set; }
        public double AppliedInvoiceAmount { get; set; }
        public double AppliedCreditBalanceAmount { get; set; }
        public DateTime EffectiveDate { get; set; } = DateTime.UtcNow;
        public string InvoiceId { get; set; }
        public string PaymentMethodId { get; set; }
        public string Status { get; set; } = "Processed";
        public string Type { get; set; } = "Electronic";
    }
}
