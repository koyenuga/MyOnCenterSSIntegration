using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oncenter.BackOffice.Entities.Interfaces;

namespace Oncenter.BackOffice.Clients.Flexera
{
    public class FlexeraOperation :IOperation
    {
        protected string UserName { get; set; }
        protected string Password { get; set; }
        protected string EndPointUrl { get; set; }
      
        public string Execute( string endpoint, string action, string soapXml)
        {
            var client = new RestClient(EndPointUrl + endpoint);

            var request = new RestRequest(Method.POST);

            byte[] credentialBuffer = new System.Text.UTF8Encoding().GetBytes(UserName + ":" + Password);

            var authorization = "Basic " + Convert.ToBase64String(credentialBuffer);

            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("accept", "application/json");
            request.AddHeader("pragma", "no-cache");
            request.AddHeader("soapaction", action);
            request.AddHeader("authorization", authorization);
            request.AddHeader("content-type", "text/xml; charset=utf-8");
            request.AddParameter("text/xml; charset=utf-8", soapXml, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            return response.Content;
        }

        public string Execute(string username, string password, string endpointUrl, string action, string param, string version)
        {
            throw new NotImplementedException();
        }

        public string Execute(string authorization, string endpointUrl, string action, string param)
        {
            throw new NotImplementedException();
        }

        public string Execute(Method requestType, object JsonParameter = null, string version = "211.0")
        {
            throw new NotImplementedException();
        }
    }
}
