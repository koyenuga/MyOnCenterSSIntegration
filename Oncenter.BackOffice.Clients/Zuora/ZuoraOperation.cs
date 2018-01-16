using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oncenter.BackOffice.Clients.Zuora
{
    public class ZuoraOperation
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        protected string ProcessRequest(string url, Method requestType, object JsonParameter = null, string version = "211.0")
        {
            var client = new RestClient(url);
            var request = new RestRequest(requestType);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("apisecretaccesskey", Password);
            request.AddHeader("apiaccesskeyid", UserName);
            request.AddHeader("zuora-version", version);

            if (JsonParameter != null)
                request.AddParameter("application/json", JsonParameter, ParameterType.RequestBody);



            IRestResponse response = client.Execute(request);
            return response.Content;
        }
    }
}
