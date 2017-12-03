using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oncenter.BackOffice.Entities.Orders;

namespace Oncenter.BackOffice.Entities.Interfaces
{
    public interface IOrderRequest
    {
        string SubscriptionNumber { get; set; }
        string Term { get; set; }
        LicenseModelType LicenseModel { get; set; }
        string TermType { get; set; }
        DateTime ExpirationDate { get; set; }
        DateTime EffectiveDate { get; set; }
        List<OrderLineItemRequest> LineItems { get; set; }
       
       
    }
}
