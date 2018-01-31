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
using System.Text;

namespace Oncenter.BackOffice.RestApi.Controllers
{
    
    public class LicenseController : ApiController
    {
       
        // POST: api/License
        [Route("License/TRIAL")]
        [HttpPost]
        public OCSLicense Get(string pn, string spn, string pf="", string an="", string cn="", int qty=1, int trialdays=14, bool networked=false)
        {
           
            return new FlexeraProvisioner(
                ConfigurationManager.AppSettings["FNOUserName"],
                ConfigurationManager.AppSettings["FNOPassword"],
                ConfigurationManager.AppSettings["FNOEnvUrl"])
                .ProvisionTrialLicense(trialdays, pn, spn, cn, an, pf, qty, networked);
        }

        [Route("License/TRIAL/OST")]
        public OCSLicense GetOSTTrialLicense(int trialdays)
        {

            return new FlexeraProvisioner(
                ConfigurationManager.AppSettings["FNOUserName"],
                ConfigurationManager.AppSettings["FNOPassword"],
                ConfigurationManager.AppSettings["FNOEnvUrl"])
                .ProvisionTrialLicense(trialdays, "OSTLocal3.95", "OSTLocalServices", "", "", "", 1, false);
        }

        [Route("LicenseServer/{deviceId}/ClientId")]
        public string GetLicenseServerAccountNumber(string deviceId)
        {

            return new FlexeraProvisioner(
                ConfigurationManager.AppSettings["FNOUserName"],
                ConfigurationManager.AppSettings["FNOPassword"],
                ConfigurationManager.AppSettings["FNOEnvUrl"])
                .GetLicenseServerAccountNumber(deviceId);
        }

        [Route("Entitlement/{id}")]
        [HttpPost]
        public HttpResponseMessage GetEntitlement(string id)
        {
            var resp = new FlexeraProvisioner(
               ConfigurationManager.AppSettings["FNOUserName"],
               ConfigurationManager.AppSettings["FNOPassword"],
               ConfigurationManager.AppSettings["FNOEnvUrl"])
               .GetEntitlement(id);

            return new HttpResponseMessage
            {
                Content = new StringContent( resp, Encoding.UTF8, "application/xml")
            };
        }


    }
}
