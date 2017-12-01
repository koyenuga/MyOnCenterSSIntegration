﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oncenter.BackOffice.Entities.Interfaces;

namespace Oncenter.BackOffice.Entities.Orders
{
    public class OrderEntitlement : IOrderEntitlement
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
    }
}