using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oncenter.BackOffice.Entities.Flexera
{
    public class FlexeraEntitlement
    {
        public string EntitlementId { get; set; }
        public string ZuoraSubscriptionId { get; set; }
        public bool AutoGenerateEntitlementId { get => true; }
        public bool AutoProvision { get => true; }
        
        public List<FlexeraEntitlementLineItem> LineItems { get; set; }
    }
}
