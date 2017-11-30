using Oncenter.BackOffice.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oncenter.BackOffice.Entities.Orders
{
    public class CreateOrderRequest
    {
        public string CompanyName { get; set; }
        public string OrganizationId { get; set; }
        public string OrderNumber { get; set; }
        public string Term { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool AutoProvision { get; set; } = true;
        public  List<OrderLineItem> LineItems { get; set; }
        public LicenseModelType LicenseModel { get; set; }
        public string TermType { get; set; }
        public DateTime EffectiveDate { get; set; }
    }
}
