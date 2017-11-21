using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oncenter.BackOffice.Entities.Interfaces;


namespace Oncenter.BackOffice.Entities.Zuora
{
    /// <summary>
    /// 
    /// </summary>
    public class ZuoraSubscription
    {

        /// <summary>
        /// Zuora subscription ID
        /// </summary>
        public string SubscriptionId { get; set; }

        /// <summary>
        /// Zuora Subscription Number
        /// </summary>
        public string SubscriptionNumber { get; set; }
        /// <summary>
        /// Netsuit Client Id
        /// </summary>
       public string ClientId { get; set; }

        /// <summary>
        /// Flexera Entitlement Id
        /// </summary>
        public string EntitlementId { get; set; }

        /// <summary>
        /// Subscription products.
        /// </summary>
        public List<ZuoraSubscriptionLineItem> LineItems { get; set; }
        
    }
}
