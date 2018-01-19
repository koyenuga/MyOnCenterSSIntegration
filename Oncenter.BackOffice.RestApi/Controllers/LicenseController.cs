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
using System.Xml.Linq;

namespace Oncenter.BackOffice.RestApi.Controllers
{
    
    public class LicenseController : ApiController
    {
       
        // POST: api/License
        [Route("License/TRIAL")]
        public OCSLicense Get(string partnumber, string trialdays="14")
        {
           
            return new FlexeraProvisioner(
                ConfigurationManager.AppSettings["FNOUserName"],
                ConfigurationManager.AppSettings["FNOPassword"],
                ConfigurationManager.AppSettings["FNOEnvUrl"])
                .ProvisionTrialLicense(trialdays, partnumber);
        }

        [Route("License/TRIAL/OST")]
        public OCSLicense GetOstTrial(string trialdays = "14")
        {

            return new FlexeraProvisioner(
                ConfigurationManager.AppSettings["FNOUserName"],
                ConfigurationManager.AppSettings["FNOPassword"],
                ConfigurationManager.AppSettings["FNOEnvUrl"])
                .ProvisionTrialLicense(trialdays, "OSTLocal3.95", "OSTLocalServices");
        }

        [Route("Entitlement/{id}")]
        [HttpPost]
        public string GetEntitlement(string id)
        {

            return new FlexeraProvisioner(
                ConfigurationManager.AppSettings["FNOUserName"],
                ConfigurationManager.AppSettings["FNOPassword"],
                ConfigurationManager.AppSettings["FNOEnvUrl"])
                .GetEntitlement(id);
        }


    }
}
