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
        public double Price { get; set; }
        public bool IsLicenseServer { get; set; }
        public bool IsMaintenanceItem { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}