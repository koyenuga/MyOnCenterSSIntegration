using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oncenter.BackOffice.Entities.Flexera
{
    public class FlexeraEntitlementLineItem
    {
        public string ZuoraLineItemId { get; set; }
        public string LineItemId { get; set; }
        public string ProductId { get; set; }
        public string PartNo { get; set; }
        public string SKU { get; set; }
        public string LicenseModel { get; set; }
        public int Quantity { get; set; }
        public bool IsLicenseServer { get; set; }
        public bool IsMaintenanceItem { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
