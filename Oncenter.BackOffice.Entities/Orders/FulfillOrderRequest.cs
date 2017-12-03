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
        public OncenterAccountRequest Account { get; set; }  
        public OrderRequest Order { get; set; }

        public OncenterContact BillToContact { get; set; }

    }

    public class OrderRequest : IOrderRequest
    {
        public string SubscriptionNumber { get; set; }
        public string Term { get; set; }
        public LicenseModelType LicenseModel { get; set; }
        public string TermType { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public List<OrderLineItemRequest> LineItems { get; set; }
    }
}