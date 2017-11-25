using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;

using Oncenter.BackOffice.Entities.Interfaces;

namespace Oncenter.BackOffice.Entities.Orders
{
    public class Order : DynamicObject, IOrder
    {
        public string Id { get; set; }
        public string OrderNumber { get; set; }
        public string AccountNumber { get; set; }
        public string OrganizationId { get; set; }
        public string Term { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool AutoProvision { get; set; } = true;
        public virtual List<IOrderLineItem> LineItems { get; set; }
        public LicenseModelType LicenseModel { get; set; }
        public string TermType { get; set; }
        public DateTime EffectiveDate { get; set; }
        public List<IOrderEntitlement> Entitlements { get; set; }
    }
}
