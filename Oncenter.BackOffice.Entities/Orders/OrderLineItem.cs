using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;

using Oncenter.BackOffice.Entities.Interfaces;

namespace Oncenter.BackOffice.Entities.Orders
{
    public class OrderLineItem :  IOrderLineItem
    {
        public string Id { get; set; }
        public string OrderId { get; set; }
        public string OrderNumber { get; set; }
        public string OrderLineItemNumber { get; set; }
        public string ProductName { get; set; }
        public string PartNo { get; set; }    
        public string LicenseModel { get; set; }
        public int Quantity { get; set; }
        public bool IsLicenseServer { get; set; }
        public bool IsMaintenanceItem { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public double Price { get; set; }
        public string ProductRatePlanId { get; set; }
        public string ProductRatePlanChargeId { get; set; }
    }
}
