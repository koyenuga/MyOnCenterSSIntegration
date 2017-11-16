using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oncenter.BackOffice.Entities.Flexera
{
    public class FlexeraOrganization
    {
        public string OrganizationId { get; set; }
        public string ZuoraAccountId { get; set; }
        public string CompanyName { get; set; }
        List<FlexeraEntitlement> Entitlements { get; set; }

    }
}
