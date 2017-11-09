using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oncenter.BackOffice.Entities;
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

        public void SaveAttachement<T>(byte[] fileBytes, string fileName, Action<T> callback)
        {

            var client = new RestClient("http://example.com");
            var client = new RestClient("https://rest.zuora.com/v1/attachments");
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("apisecretaccesskey", "dummyPassword");
            request.AddHeader("apiaccesskeyid", "dummyUser");
            var request = new RestRequest("resource/{id}", Method.POST);
            request.AddParameter("name", "value"); // adds to POST or URL querystring based on Method
            request.AddUrlSegment("id", "123"); // replaces matching token in request.Resource

            // easily add HTTP Headers
            request.AddHeader("header", "value");

            // add files to upload (works with compatible verbs)
            request.AddFile(path,);

            // execute the request
            IRestResponse response = client.Execute(request);
            var content = response.Content; // raw content as string

            // or automatically deserialize result
            // return content type is sniffed but can be explicitly set via RestClient.AddHandler();
            RestResponse<T> response2 = client.Execute<T>(request);
            var name = response2.Data.Name;

            // easy async support
            client.ExecuteAsync(request, response => {
                Console.WriteLine(response.Content);
            });

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
