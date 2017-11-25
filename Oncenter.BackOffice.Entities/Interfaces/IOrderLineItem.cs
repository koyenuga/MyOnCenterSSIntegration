﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oncenter.BackOffice.Entities.Interfaces
{
    public interface IOrderLineItem
    {
        string Id { get; set; }
        string OrderId { get; set; }
        string OrderNumber { get; set; }
        string OrderLineItemNumber { get; set; }
        string ProductRatePlanId { get; set; }
        string ProductRatePlanChargeId { get; set; }
        string ProductName { get; set; }
        string PartNo { get; set; }
        string SKU { get; set; }
        string LicenseModel { get; set; }
        int Quantity { get; set; }
        double Price { get; set; }
        bool IsLicenseServer { get; set; }
        bool IsMaintenanceItem { get; set; }
        DateTime EffectiveDate { get; set; }
        DateTime ExpirationDate { get; set; }
    }
}
