using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Oncenter.BackOffice.Entities.License;
using Oncenter.BackOffice.Entities.Interfaces;
using Oncenter.BackOffice.Clients.Flexera;
using System.Configuration;

namespace Oncenter.BackOffice.RestApi.Controllers
{
    public class LicenseController : ApiController
    {
       
        // POST: api/License
        [Route("License/OST/TRIAL")]
        public OCSLicense Post()
        {
            return new FlexeraProvisioner(
                ConfigurationManager.AppSettings["FNOUserName"],
                ConfigurationManager.AppSettings["FNOPassword"],
                ConfigurationManager.AppSettings["FNOEnvUrl"])
                .ProvisionOSTTrialLicense();
        }

        
    }
}
