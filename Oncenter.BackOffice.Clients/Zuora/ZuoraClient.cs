using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oncenter.BackOffice.Entities;
using Oncenter.BackOffice.Entities.Zuora;
using Oncenter.BackOffice.Entities.Interfaces;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Json;
using RestSharp;
using Newtonsoft.Json;

namespace Oncenter.BackOffice.Clients.Zuora
{
    public class ZuoraClient
    {
        public string ZuoraUrl { get => "https://rest.apisandbox.zuora.com/"; }
        public string UserName { get; set; }
        public string Password { get; set; }
       
        public ZuoraClient(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }
        
        public dynamic GetSubscriptionDetails(string subscriptionId)
        {
            string requestUrl = "https://rest.zuora.com/v1/subscriptions/" + subscriptionId;
            return JsonConvert.DeserializeObject(ProcessRequest(requestUrl));
        }


        public dynamic SaveAttachement(string entity, string description, string id, string fileName, byte[] file, string contentType, string attachmentId="")
        {
            string url = string.Empty;

            url = string.IsNullOrWhiteSpace(attachmentId) ? string.Format( ZuoraUrl + "v1/attachments/?description={1}&associatedObjectType={2}&associatedObjectKey={3}", description, entity, id) :
               string.Format(ZuoraUrl +"v1/attachments/{0}", attachmentId);

            var client = new RestClient(url);
            var request = new RestRequest(Method.POST);

            if (attachmentId.Trim() == string.Empty)
                request = new RestRequest(Method.PUT);

            request.AddHeader("content-type", "application/json");
            request.AddHeader("apisecretaccesskey", Password);
            request.AddHeader("apiaccesskeyid", UserName);

            request.AddFile(fileName, file, fileName, contentType);

            IRestResponse response = client.Execute(request);
            return JsonConvert.DeserializeObject(response.Content);

        }

      
        private string  ProcessRequest(string url)
        {
            var client = new RestClient(url);
            var request = new RestRequest(Method.GET);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("apisecretaccesskey", "dummyPassword");
            request.AddHeader("apiaccesskeyid", "dummyUser");
            IRestResponse response = client.Execute(request);
            return response.Content;
        }

    }
}
