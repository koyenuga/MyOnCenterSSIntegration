﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oncenter.BackOffice.Entities.Interfaces
{
    public interface IOrderLineItemRequest
    {
       
        string LineItemNumber { get; set; }
        string ProductRatePlanId { get; set; }
        string ProductRatePlanChargeId { get; set; }
        string ProductName { get; set; }
        string PartNo { get; set; }
        int Quantity { get; set; }
        double Amount { get; set; }
        bool IsDiscountLineItem { get; set; }
        bool IsCloudLicenseServer { get; set; }
        bool DeployToCloudLicenseServer { get; set; }
        string CloudLicenseServerName { get; set; }
        string NetSuitIntegrationId { get; set; }
        bool IsMaintenanceItem { get; set; }
        DateTime EffectiveDate { get; set; }
        DateTime ExpirationDate { get; set; }
        string EntitlementFamily { get; set; }
        string LicenseModel { get; set; }
       bool IsSingleSeat { get; set; }
    }
}
