using Oncenter.BackOffice.Entities.Flexera;
using Oncenter.BackOffice.Entities.Zuora;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oncenter.BackOffice.Entities
{
    public class Account
    {
        public string Id { get; set; }
        public string FlexeraOrganizationId { get; set; }
        public string ZuoraAccountId { get; set; }
        public string CompanyName { get; set; }
        public List<FlexeraEntitlement> Entitlements { get; set; }
        public List<ZuoraSubscription> Subscriptions { get; set; }
    }
}
