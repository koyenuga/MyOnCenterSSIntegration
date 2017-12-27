using System;
using Oncenter.BackOffice.Entities.Interfaces;
namespace Oncenter.BackOffice.Entities.Orders
{
    public class OrderLineItemRequest : IOrderLineItemRequest
    {
        
        public string LineItemNumber { get; set; }
        public string ProductRatePlanId { get; set; }
        public string ProductRatePlanChargeId { get; set; }
        public string ProductName { get; set; }
        public string PartNo { get; set; }
        public int Quantity { get; set; }
        public double Amount { get; set; }
        public bool IsDiscountLineItem { get; set; } = false;
        public bool IsCloudLicenseServer { get; set; }
        public bool DeployToCloudLicenseServer { get; set; }
        public string CloudLicenseServerName { get; set; }
        public string NetSuitIntegrationId { get; set;  }
        public bool IsMaintenanceItem { get; set; }
        public bool IsSingleSeat { get; set; } = false;
        public bool IsPerpetualLicense { get; set; } = false;
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string EntitlementFamily { get; set; }
        public string LicenseModel { get; set; } = string.Empty;
    }
}