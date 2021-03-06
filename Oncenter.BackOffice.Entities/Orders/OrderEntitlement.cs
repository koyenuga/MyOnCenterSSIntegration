﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oncenter.BackOffice.Entities.Interfaces;

namespace Oncenter.BackOffice.Entities.Orders
{
    public class OrderEntitlement
    { 
    
        public string EntitlementId { get; set; }
        public string EntitlementFamily { get; set; }
        public List<OrderEntitlementLineItem> LineItems { get; set; } = new List<OrderEntitlementLineItem>();
    }

    public class OrderEntitlementLineItem
    {
        public string ProductRatePlanChargeId { get; set; }
        public string EntitlementId { get; set; }
        public string EntitlementLineItemId { get; set; }
        public int Quantity { get; set; }
        public string ActivationId { get; set; }
        public string DeviceId { get; set; }
        public string LicenseManagerId { get; set; }
        public string PartNumber { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool IsPerpertual { get; set; }
        public LicenseModelType LicenseModel { get; set; }
        public string Term { get; set; }
    }
}
