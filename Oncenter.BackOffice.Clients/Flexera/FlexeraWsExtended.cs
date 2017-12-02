using System;
using System.Web.Services;
using System.Diagnostics;
using System.Web.Services.Protocols;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Net;

namespace Oncenter.BackOffice.Clients.Flexera
{

    public class FlexeraWebServiceBase : System.Web.Services.Protocols.SoapHttpClientProtocol
    {
        protected override WebRequest GetWebRequest(Uri uri)
        {
            HttpWebRequest request;
            request = (HttpWebRequest)base.GetWebRequest(uri);
            if (PreAuthenticate)
            {
                NetworkCredential networkCredentials =
                Credentials.GetCredential(uri, "Basic");
                if (networkCredentials != null)
                {
                    byte[] credentialBuffer =
                    new
                    System.Text.UTF8Encoding().GetBytes(
                    networkCredentials.UserName + ":" +
                    networkCredentials.Password);
                    request.Headers["Authorization"] =
                    "Basic " +
                    Convert.ToBase64String(credentialBuffer);
                }
                else
                {
                    throw new ApplicationException(
                    "No network credentials");
                }
            }
            return request;
        }
    }

}
namespace Oncenter.BackOffice.Clients.Flexera.Entitlement
{
    public partial class EntitlementOrderService : FlexeraWebServiceBase
    {

    }

}
namespace Oncenter.BackOffice.Clients.Flexera.UserOrganizationHierachy
{
    
    public partial class UserOrgHierarchyService : FlexeraWebServiceBase
    {

    }
}