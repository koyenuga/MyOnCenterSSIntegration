using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oncenter.BackOffice.Entities.License
{
    public class OCSLicense
    {
        public string EntitlementId { get; set; }
        public string ActivationId { get; set; }
        public string MaintenanceActivationId { get; set; }

        public string CloudLicenseServerId { get; set; }
    }
}
