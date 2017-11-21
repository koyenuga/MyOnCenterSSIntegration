using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oncenter.BackOffice;

namespace Oncenter.BackOffice.Entities.Zuora
{
    public class ZuoraSubscriptionLineItem
    {
        /// <summary>
        ///  Zuora Subscription Rate Plan Id
        /// </summary>
        public string LineItemId { get; set; }
        /// <summary>
        /// Zuora Product Id
        /// </summary>
        public string ProductId { get; set; }
        /// <summary>
        /// Product Sku
        /// </summary>
        public string ProductSKU { get; set; }
        /// <summary>
        /// product license count
        /// </summary>
        public int Qty { get; set; }
        /// <summary>
        /// License Model Type
        ///  - SingleSeat, MultiSeat, Local, Cloud.
        /// </summary>
        public LicenseModelType LicenModel { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        /// <summary>
        /// If product is or will be provisioned to a license manager
        /// </summary>
        public bool ProvisionedToLicenseServer { get; set; }
        /// <summary>
        /// License server details, if this is empty 
        /// and ProvisionedToLicenseServer is true, 
        /// a new license server will be created.
        /// </summary>
        public string LicenseManagerId { get; set; } 
        /// <summary>
        /// Flexera Entitlement ID
        /// </summary>
        public string EntitlementId { get; set; }
        /// <summary>
        /// Flexera Entitlement Line Item Id
        /// </summary>
        public string EntitlementLineItemId { get; set; }

    
    }
}
