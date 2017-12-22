using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oncenter.BackOffice.Entities.Interfaces;
using Oncenter.BackOffice.Entities.Accounts;

namespace Oncenter.BackOffice.Entities.Orders
{
    public class OrderDetail : IOrderDetail
    {
        public OncenterAccount Account { get; set; }
        public string Id { get; set; }
        public string OrderNumber { get; set; }
        public string Term { get; set; }
        public string TermType { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public List<IOrderLineItem> LineItems { get; set; }
        public List<IOrderEntitlement> Entitlements { get; set; }
        public string SubscriptionNumber { get; set; }
        public string InvoiceOwnerAccountNumber { get; set; }
        public string InvoiceNetsuiteIntegrationId { get; set; }
      
        List<OrderLineItemRequest> IOrderRequest.LineItems { get; set; }
    }
}
