using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        List<IOrderLineItemRequest> LineItems { get; set; }
       
       
    }
}
