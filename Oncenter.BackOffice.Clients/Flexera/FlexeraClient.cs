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
using Oncenter.BackOffice.Clients.Flexera.Devices;
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
        public List<string> CreateEntitlement(string subscriptionNumber, List<OrderEntitlementLineItem> lineItems,
            string organizationId, LicenseModelType licenseModel, string term="12")
        {
            List<createSimpleEntitlementDataType> rqData = new List<createSimpleEntitlementDataType>();
            var results = new List<string>();

            if (licenseModel == LicenseModelType.LocalSingleSeat)
            {
                for (var count = lineItems[0].Quantity; count > 0; count--)
                {
                    //results.Add(subscriptionNumber + "-000-" + count);
                    //create(organizationId, subscriptionNumber, lineItems);
                    rqData.Add(BuildEntitlementRequest(lineItems, organizationId, 
                        subscriptionNumber + "-000-" + count, term, "1"));

                }
            }
            else
            {
                //results.Add(subscriptionNumber + "-000-1");
                //create(organizationId, subscriptionNumber + "-000-1", lineItems);
                rqData.Add(BuildEntitlementRequest(lineItems, organizationId, subscriptionNumber,term));
            }

            List<string> EntitlementIds = new List<string>();

            var fnoWs = new v1EntitlementOrderService();
            fnoWs.Url = EndPointUrl + "EntitlementOrderService";

            fnoWs.PreAuthenticate = true;
            CredentialCache credCache = new System.Net.CredentialCache();
            NetworkCredential netCred = new NetworkCredential(UserName, Password);
            credCache.Add(new Uri(fnoWs.Url), "Basic", netCred);
            fnoWs.Credentials = credCache;

            var rqType = new createSimpleEntitlementRequestType();
            rqType.simpleEntitlement = rqData.ToArray();
            var resp = fnoWs.createSimpleEntitlement(rqType);

            if(resp.statusInfo.status == Entitlement.StatusType.SUCCESS)
            {
                foreach(var i in resp.responseData)
                {
                    EntitlementIds.Add(i.entitlementId);
                }
            }
            return EntitlementIds;


           
      


        }

        void CreateLicenseServer( string id, string companyName, string productLine, int qty=1, int currentDeviceCount = 0)
        {
            var fnoWs = new v1ManageDeviceService();
            fnoWs.Url = EndPointUrl + "ManageDeviceService";

            fnoWs.PreAuthenticate = true;
            CredentialCache credCache = new System.Net.CredentialCache();
            NetworkCredential netCred = new NetworkCredential(UserName, Password);
            credCache.Add(new Uri(fnoWs.Url), "Basic", netCred);
            fnoWs.Credentials = credCache;
            var deviceRq = new deviceDataType[qty];
            for( int i = 0; i < qty; i++)
            {
                deviceRq[i] = new deviceDataType();
                deviceRq[i].deviceIdentifier = new createDeviceIdentifier
                {
                    deviceType = WSDeviceType.SERVER,
                    publisherName = id,
                    deviceIdType = deviceIdTypeType.STRING,
                    deviceId = companyName + "-" + productLine +"-CLM-0" + (currentDeviceCount + 1).ToString(),
                    deviceIdTypeSpecified = true
                };
                deviceRq[i].deployment = deployment.CLOUD;
                deviceRq[i].deploymentSpecified = true;
                deviceRq[i].channelPartners = new Devices.channelPartnerDataType[1];
                deviceRq[i].channelPartners[0] = new Devices.channelPartnerDataType();
                deviceRq[i].channelPartners[0].organizationUnit = new Devices.organizationIdentifierType();
                deviceRq[i].channelPartners[0].organizationUnit.primaryKeys = new Devices.organizationPKType();
                deviceRq[i].channelPartners[0].organizationUnit.primaryKeys.name = id;
                deviceRq[i].publisherIdName = new publisherIdentifier {
                     name = id
                };
                

            }
            var resp = fnoWs.createDevice(deviceRq);
        }

        void AddEntitlementToLicenseServer( Dictionary<string, int> ActivationCodes,
            deviceIdentifier device)
        {
            var fnoWs = new v1ManageDeviceService();
            fnoWs.Url = EndPointUrl + "ManageDeviceService";

            fnoWs.PreAuthenticate = true;
            CredentialCache credCache = new System.Net.CredentialCache();
            NetworkCredential netCred = new NetworkCredential(UserName, Password);
            credCache.Add(new Uri(fnoWs.Url), "Basic", netCred);
            fnoWs.Credentials = credCache;
            var lineItemRq = new linkAddonLineItemDataType[1];
            lineItemRq[0] = new linkAddonLineItemDataType();
            lineItemRq[0].deviceIdentifier = device;
            var linkLineItems = new linkLineItemDataType[ActivationCodes.Count()];
            for( int i =0;  i <  ActivationCodes.Count(); i++)
            {
                linkLineItems[i] = new linkLineItemDataType {
                    lineItemIdentifier = new linkLineItemIdentifier
                    {
                        // activationId = ActivationCodes[i].
                    }
                      
                };
            }

            lineItemRq[0].lineItem = linkLineItems;
           var resp =  fnoWs.linkAddonLineItems(lineItemRq);
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

        public void create(string accountNumber, string subscriptionNumber,  List<OrderEntitlementLineItem> items)
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
       
        List<XElement> BuildEntitlementLineItemRq(List<OrderEntitlementLineItem> items, XNamespace urn)
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
                       new XElement(urn + "orderId", item.ProductRatePlanChargeId),
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
                var entitlementList = new List<OrderEntitlementLineItem>();
                foreach(var e in resp.entitlement)
                {
                    entitlementList = (from i in e.simpleEntitlement.lineItems
                                       select new OrderEntitlementLineItem
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
        public string GetOrganization(string accountNumber)
        {
            var fnoWs = new v1UserOrgHierarchyService();
            fnoWs.Url = EndPointUrl + "UserOrgHierarchyService";

            fnoWs.PreAuthenticate = true;
            CredentialCache credCache = new System.Net.CredentialCache();
            NetworkCredential netCred = new NetworkCredential(UserName, Password);
            credCache.Add(new Uri(fnoWs.Url), "Basic", netCred);
            fnoWs.Credentials = credCache;

            var rqType = new getOrganizationsQueryRequestType();

            rqType.batchSize = "10";
            rqType.pageNumber = "1";
            rqType.queryParams = new organizationQueryParametersType
            {
                 orgName = new UserOrganizationHierachy.SimpleQueryType
                 {
                      searchType = UserOrganizationHierachy.simpleSearchType.EQUALS,
                       value = accountNumber
                 }
            };

            var resp = fnoWs.getOrganizationsQuery(rqType);

            if (resp.statusInfo.status == UserOrganizationHierachy.StatusType.SUCCESS)
            {
                if (resp.responseData != null)
                    return resp.responseData[0].organization.uniqueId;
                else
                    return string.Empty;
            }
            else
                return string.Empty;

        }


        public createSimpleEntitlementDataType BuildEntitlementRequest(List<OrderEntitlementLineItem> lineItems,
            string organizationId, string subscriptionNumber,
            string term,
            string qty=""
            )
        {

            var csrtp = new createSimpleEntitlementDataType();
            csrtp.autoDeploy = true;
            csrtp.autoDeploySpecified = true;
            csrtp.soldTo = organizationId;
            csrtp.entitlementId = new idType
            {
                autoGenerateSpecified  = true,
                autoGenerate = true,
                id = subscriptionNumber,
                
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
                                   activationId = new idType
                                   {
                                       id = Guid.NewGuid().ToString(),
                                       autoGenerate = true,
                                       autoGenerateSpecified = true

                                   },
                                   isPermanent = p.IsPerpertual,
                                   isPermanentSpecified = true,
                                   orderId = p.ProductRatePlanChargeId,
                                   numberOfCopies = string.IsNullOrWhiteSpace(qty) ? p.Quantity.ToString() : qty,

                                   partNumber = new partNumberIdentifierType
                                   {
                                       primaryKeys = new partNumberPKType {
                                           partId = p.PartNumber
                                       }

                                   },
                                   startDate = p.EffectiveDate,
                                   expirationDate =  p.ExpirationDate,
                                   expirationDateSpecified = p.IsPerpertual? false : true

                                
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
