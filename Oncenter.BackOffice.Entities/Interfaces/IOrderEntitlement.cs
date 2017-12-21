using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace Oncenter.BackOffice.Entities.Interfaces
{
    public interface IOrderEntitlement
    {
        string ProductRatePlanChargeId { get; set; }
        string EntitlementId { get; set; }
        string EntitlementLineItemId { get; set; }
        int Quantity { get; set; }
        string ActivationId { get; set; }
        string DeviceId { get; set; }
        string LicenseManagerId { get; set; }
        string PartNumber { get; set; }
        DateTime EffectiveDate { get; set; }
        DateTime ExpirationDate { get; set; }
        bool IsPerpertual { get; set; }

    }
}
