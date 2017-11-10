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

namespace Oncenter.BackOffice.Clients.Zuora
{
    public class ZuoraClient
    {

        public ZuoraClient()
        {

        }
        public ZuoraClient(IBackOfficeClient client)
        {

        }

        public void SaveAttachement<T>()//ZuoraContext<T> context)
        {

            var client = new RestClient("https://rest.zuora.com/v1/attachments");
            var request = new RestRequest(Method.POST);
            //request.AddHeader("content-type", "application/json");
            //request.AddHeader("apisecretaccesskey", context.AccessKey);
            //request.AddHeader("apiaccesskeyid", context.UserName);
            //request.AddParameter("description", context.Description);
            //request.AddParameter("associatedObjectType", context.AssociatedObject);
            //request.AddParameter("associatedObjectKey", context.AssociatedObjectKey);
            //request.AddParameter("application/json", "\"curl\\n--form \\\"file=@{0}\\\" \\\\\\n-X POST https://rest.zuora.com/v1/attachments/?description={1}&associatedObjectType={2}&associatedObjectKey={3}\"", ParameterType.RequestBody);

           
            //request.AddFile(context.FileName, context.File, context.FileName, context.ContentType);

            // execute the request
            IRestResponse<ZuoraSubscription> response = client.Execute<ZuoraSubscription>(request);
          

            // or automatically deserialize result
            // return content type is sniffed but can be explicitly set via RestClient.AddHandler();
            //RestResponse<T> response2 = client.Execute<T>(request);
            //IRestResponse response = client.Execute(request);

            //var name = response2.Data.Name;

            // easy async support
            //client.ExecuteAsync(request, response => {
            //    Console.WriteLine(response.Content.);
            //});

        }

        public void UpdateAttachment()
        {
            var client = new RestClient("https://rest.zuora.com/v1/attachments/{attachment-id}");
            var request = new RestRequest(Method.PUT);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("apisecretaccesskey", "dummyPassword");
            request.AddHeader("apiaccesskeyid", "dummyUser");
            request.AddParameter("application/json", "\"curl\\n-H \\\"filename\\\":\\\"Image123.png\\\" \\\\\\n-H \\\"description\\\":\\\"Updated Image\\\" \\\\\\n-H \\\"Accept:application:json\\\" \\\\\\n-X PUT https://rest.zuora.com/v1/attachments/{0}\"", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
        }

        private void GetResponse<T>(Uri uri, string method, string data, Action<T> callback)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);

            request.Method =  method;
            request.ContentType = "text/plain;charset=utf-8";

            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            byte[] bytes = encoding.GetBytes(data);

            request.ContentLength = bytes.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
               
                requestStream.Write(bytes, 0, bytes.Length);
            }

            request.BeginGetResponse((x) =>
            {
                using (HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(x))
                {
                    if (callback != null)
                    {
                        DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
                        callback((T)ser.ReadObject(response.GetResponseStream()));
                    }
                }
            }, null);
        }
    }
}
