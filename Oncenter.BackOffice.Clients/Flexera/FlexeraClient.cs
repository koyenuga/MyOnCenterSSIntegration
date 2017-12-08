using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using Oncenter.BackOffice.Entities;
using Oncenter.BackOffice.Entities.Interfaces;
using Oncenter.BackOffice.Entities.Orders;
using Oncenter.BackOffice.Clients.Flexera.Entitlement;
using Oncenter.BackOffice.Clients.Flexera.UserOrganizationHierachy;
using System.Net;
using System.IO;
using RestSharp;

namespace Oncenter.BackOffice.Clients.Flexera
{
    public class FlexeraClient
    {
        string UserName { get; set; }
        string Password { get; set; }
        string EndPointUrl { get; set; }
        public FlexeraClient(string userName, string password, string endPointUrl)
        {
            UserName = userName;
            Password = password;
            EndPointUrl = endPointUrl;
        }
        public List<OrderEntitlement> CreateEntitlement(string subscriptionNumber, List<IOrderEntitlement> lineItems,
            string organizationId, LicenseModelType licenseModel, bool autoProvision = true)
        {


            create(organizationId, subscriptionNumber, lineItems);
            List<createSimpleEntitlementDataType> flexeraEntitlements = new List<createSimpleEntitlementDataType>();


            if (licenseModel == LicenseModelType.LocalSingleSeat)
            {
                for (var count = lineItems[0].Quantity; count > 0; count--)
                {
                    flexeraEntitlements.Add(BuildEntitlementRequest(lineItems,
                            organizationId, subscriptionNumber, "1", autoProvision));

                }
            }
            else
                flexeraEntitlements.Add(BuildEntitlementRequest(lineItems,
                    organizationId, subscriptionNumber, "", autoProvision));

            var fnoWs = new v1EntitlementOrderService();
            fnoWs.Url = EndPointUrl + "EntitlementOrderService";
            NetworkCredential netCredential = new NetworkCredential(UserName, Password);
            Uri uri = new Uri(fnoWs.Url);
            ICredentials credentials = netCredential.GetCredential(uri, "Basic");
            fnoWs.Credentials = credentials;
            fnoWs.PreAuthenticate = true;

            var simpleEntitlementRqType = new createSimpleEntitlementRequestType();
            simpleEntitlementRqType.simpleEntitlement = flexeraEntitlements.ToArray();
            var resp = fnoWs.createSimpleEntitlement(simpleEntitlementRqType);

            var results = new List<OrderEntitlement>();
            if (resp.statusInfo.status == Entitlement.StatusType.SUCCESS)
            {
                foreach (var e in resp.responseData)
                {
                    results.AddRange(GetEntitlements(e.entitlementId));
                }

                return results;
            }
            else
                throw new Exception(resp.statusInfo.reason);

            //createEntitlementRequestype.lineItems[0].partNumber.
            //var c = new Oncenter.BackOffice.Clients.Flexera.Entitlement..createSimpleEntitlement()
            //https://flex1245-uat.flexnetoperations.com/flexnet/services/EntitlementOrderService?wsdl
            //https://flex1245-uat.flexnetoperations.com/flexnet/services/UserOrgHierarchyService?wsdl



        }

        private  HttpWebRequest CreateWebRequest(string url, string action)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Headers.Add("soapaction", action);
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            byte[] credentialBuffer =  new System.Text.UTF8Encoding().GetBytes( UserName + ":" + Password);
            webRequest.Headers["Authorization"] = "Basic " +   Convert.ToBase64String(credentialBuffer);
            
            return webRequest;
        }

        public void create(string accountNumber, string subscriptionNumber,  List<IOrderEntitlement> items)
        {
             
            XNamespace soapenv = "http://schemas.xmlsoap.org/soap/envelope/";
            XNamespace urn = "urn:v1.webservices.operations.flexnet.com";
            XElement soapEnv = new XElement(soapenv + "Envelope",
                                 new XAttribute(XNamespace.Xmlns + "soapenv", "http://schemas.xmlsoap.org/soap/envelope/"),
                                 new XAttribute(XNamespace.Xmlns + "urn", "urn:v1.webservices.operations.flexnet.com"),
                                    new XElement(soapenv+ "Header"),
                                    new XElement(soapenv + "Body",
                                        new XElement(urn + "createSimpleEntitlementRequest",
                                            new XElement(urn + "simpleEntitlement",
                                                new XElement(urn + "entitlementId",
                                                    new XElement(urn + "id", subscriptionNumber),
                                                    new XElement(urn + "autoGenerate", false)),
                                                new XElement(urn + "soldTo", accountNumber),
                                               BuildEntitlementLineItemRq(items, urn))),
                                               new XElement(urn + "autoDeploy", true)

                                        ));


            var soapXml = soapEnv.ToString();
            var client = new RestClient("https://flex1374-uat.flexnetoperations.com/flexnet/services/v1/EntitlementOrderService");
            var request = new RestRequest(Method.POST);
            byte[] credentialBuffer = new System.Text.UTF8Encoding().GetBytes(UserName + ":" + Password);
            var authorization  = "Basic " + Convert.ToBase64String(credentialBuffer);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("accept", "application/json");
            request.AddHeader("pragma", "no-cache");
            request.AddHeader("soapaction", "createSimpleEntitlement");
            request.AddHeader("authorization", authorization);
            request.AddHeader("content-type", "text/xml; charset=utf-8");
            request.AddParameter("text/xml; charset=utf-8", soapXml, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

         


        }
        private static void InsertSoapEnvelopeIntoWebRequest(XElement soapEnvelopeXml, HttpWebRequest webRequest)
        {
            using (Stream stream = webRequest.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }
        }
        List<XElement> BuildEntitlementLineItemRq(List<IOrderEntitlement> items, XNamespace urn)
        {
            List<XElement> elements = new List<XElement>();

            foreach (var item in items)
            {
                elements.Add(new XElement(urn + "lineItems",
                     new XElement(urn + "activationId",
                            new XElement(urn + "autoGenerate", true)),
                      new XElement(urn+ "partNumber",
                        new XElement(urn + "primaryKeys",
                            new XElement(urn + "partId", item.PartNumber))),
                       // new XElement(urn + "orderId", item.ProductRatePlanChargeId),
                        //new XElement("urn:orderLineNumber", item.li
                        new XElement(urn + "numberOfCopies", item.Quantity),
                        new XElement(urn + "startDate", item.EffectiveDate.ToString("yyyy-MM-dd")),
                        new XElement(urn + "expirationDate", item.ExpirationDate.ToString("yyyy-MM-dd"))));
            }

            return elements;
        }
        public List<OrderEntitlement> GetEntitlements( string id)
        {
            var searchQuery = new searchEntitlementRequestType
            {
                entitlementSearchCriteria = new searchEntitlementDataType
                {
                    entitlementId = new Entitlement.SimpleQueryType
                    {

                        searchType = Entitlement.simpleSearchType.EQUALS,
                        value = id

                    }
                }
            };
            var fnoWs = new v1EntitlementOrderService();
            fnoWs.Url = EndPointUrl + "EntitlementOrderService";
            fnoWs.PreAuthenticate = true;
            fnoWs.Credentials = new NetworkCredential(UserName, Password);
            var resp = fnoWs.getEntitlementsQuery(searchQuery);

            if (resp.statusInfo.status == Entitlement.StatusType.SUCCESS)
            {
                var entitlementList = new List<OrderEntitlement>();
                foreach(var e in resp.entitlement)
                {
                    entitlementList = (from i in e.simpleEntitlement.lineItems
                                       select new OrderEntitlement
                                       {
                                           ActivationId = i.activationId.id,
                                           EffectiveDate = i.startDate,
                                           Quantity = int.Parse(i.numberOfCopies),
                                           EntitlementId = e.simpleEntitlement.entitlementId.id,
                                           ProductRatePlanChargeId = i.orderLineNumber,
                                           ExpirationDate = i.expirationDate,
                                           PartNumber = i.partNumber.uniqueId,
                                           //EntitlementLineItemId = i.orderId
                                           

                                       }).ToList();

                }

            }
            throw new Exception();
        }

        public string CreateOrganization(string CompanyName, string accountNumber)
        {
            var fnoWs = new v1UserOrgHierarchyService();
            fnoWs.Url = EndPointUrl + "UserOrgHierarchyService";

            fnoWs.PreAuthenticate = true;
            CredentialCache credCache = new System.Net.CredentialCache();
            NetworkCredential netCred = new NetworkCredential(UserName, Password);
            credCache.Add(new Uri(fnoWs.Url), "Basic", netCred);
            fnoWs.Credentials = credCache;

            var rqType = new createOrgRequestType();


            var orgDataType = new List<organizationDataType>();
            orgDataType.Add(new organizationDataType
            {

                name = accountNumber,
                displayName = CompanyName,
                orgType = OrgType.CUSTOMER
            });

            rqType.organization = orgDataType.ToArray();
            var resp = fnoWs.createOrganization(rqType);

            if (resp.statusInfo.status == UserOrganizationHierachy.StatusType.SUCCESS)
            {
                return resp.responseData[0].uniqueId;
            }
            else
                throw new Exception(resp.statusInfo.reason);




        }
    
        public createSimpleEntitlementDataType BuildEntitlementRequest(List<IOrderEntitlement> lineItems,
            string organizationId, string subscriptionNumber,
            string qty ="", 
            bool autoProvision =true)
        {

            var csrtp = new createSimpleEntitlementDataType();
            csrtp.autoDeploy = autoProvision;
            csrtp.soldTo = organizationId;
            csrtp.entitlementId = new idType
            {
                autoGenerate = false,
                id = subscriptionNumber
            };
            //csrtp..entitlementAttributes = new Entitlement.attributeDescriptorType[] {
            //    new Entitlement.attributeDescriptorType{
            //        attributeName = "SubscriptionNumber",
            //        stringValue = subscriptionNumber
            //    }
            //};
            csrtp.lineItems = (from p in lineItems
                               select new createEntitlementLineItemDataType
                               {

                                   orderId = p.ProductRatePlanChargeId,
                                   numberOfCopies = string.IsNullOrWhiteSpace(qty) ? p.Quantity.ToString() : qty,

                                   partNumber = new partNumberIdentifierType
                                   {
                                       uniqueId = p.PartNumber

                                   },
                                   startDate = p.EffectiveDate,
                                   expirationDate = p.ExpirationDate


                               }).ToArray();
            return csrtp;

        }

        //public List<FlexeraEntitlement> GetEntitlementByOrg(string organizationId)
        //{
        //    var searchQuery = new searchEntitlementRequestType {
        //         entitlementSearchCriteria = new searchEntitlementDataType {
        //              soldTo = new Entitlement.SimpleQueryType
        //              {

        //                   searchType = Entitlement.simpleSearchType.EQUALS,
        //                   value = organizationId
                            
        //              }
        //         }
        //    };
        //    var resp = new EntitlementOrderService().getEntitlementsQuery(searchQuery);

        //    if (resp.statusInfo.status == Entitlement.StatusType.SUCCESS)
        //    {
        //        return (from e in resp.entitlement
        //                select new FlexeraEntitlement {
        //                    EntitlementId = e.simpleEntitlement.entitlementId.id,
        //                    OrganizationId = e.simpleEntitlement.soldTo,
                            
        //                    LineItems = (from li in e.simpleEntitlement.lineItems
        //                                 select new FlexeraEntitlementLineItem
        //                                 {
        //                                     ZuoraLineItemId = li.orderLineNumber,
        //                                     PartNo = li.partNumber.uniqueId,
        //                                     Quantity = li.numberOfCopies,
        //                                     StartDate = li.startDate,
        //                                     ExpirationDate = li.expirationDate
        //                                 }
        //                                 ).ToList(),


        //                }).ToList();
        //    }
        //    throw new Exception();
        //}

    }
}
