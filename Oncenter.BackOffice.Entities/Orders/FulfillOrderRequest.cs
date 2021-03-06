﻿using Oncenter.BackOffice.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oncenter.BackOffice.Entities.Accounts;
using Oncenter.BackOffice.Entities.Contacts;

namespace Oncenter.BackOffice.Entities.Orders
{
    public class FulfillOrderRequest
    {
        public FulfillmentRequestType RequestType { get; set; }
        public OncenterAccountRequest Account { get; set; }  
        public OrderRequest Order { get; set; }
        public OncenterContact BillToContact { get; set; }
        public OncenterContact SoldToContact { get; set; }

        private List<string> Devices = new List<string>();
        public List<string> GetDevices( )
        {
            return Devices;
        }

        public void setDevices( List<string> devices)
        {
            Devices = devices;
        }

        
    }

    public class OrderRequest
    {
        public string SubscriptionNumber { get; set; }
        public string Term { get; set; }
        public LicenseModelType LicenseModel { get; set; }
        public string TermType { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public bool AutoRenew { get; set; } = false;
        public string InvoiceOwnerAccountNumber { get; set; }
        public string InvoiceOwnerCompanyName { get; set; } = "Invoice Owner";
        public bool InvoiceOwnerIsTaxExempt { get; set; } = false;
        public string InvoiceNetsuiteIntegrationId { get; set; }

        public List<OrderLineItemRequest> LineItems { get; set; }
    }

    public enum FulfillmentRequestType
    {
        NewOrder =0,
        Renewal =1
        
    }
}
