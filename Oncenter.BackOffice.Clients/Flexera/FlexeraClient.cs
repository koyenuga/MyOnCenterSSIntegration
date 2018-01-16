using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using Oncenter.BackOffice.Entities;
using Oncenter.BackOffice.Entities.License;
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
        public string CreateEntitlement(
            string organizationId, string ProductFamily="")
        {
            List<createSimpleEntitlementDataType> rqData = new List<createSimpleEntitlementDataType>();
            var entitlementId = string.Empty;

            var fnoWs = new v1EntitlementOrderService();
            fnoWs.Url = EndPointUrl + "EntitlementOrderService";

            fnoWs.PreAuthenticate = true;
            CredentialCache credCache = new System.Net.CredentialCache();
            NetworkCredential netCred = new NetworkCredential(UserName, Password);
            credCache.Add(new Uri(fnoWs.Url), "Basic", netCred);
            fnoWs.Credentials = credCache;

            var rqType = new createSimpleEntitlementRequestType();
            rqType.simpleEntitlement = new createSimpleEntitlementDataType[] {
                 new createSimpleEntitlementDataType{
                      autoDeploy = true,
                      autoDeploySpecified = true,
                       soldTo = organizationId,
                        entitlementAttributes = new Entitlement.attributeDescriptorType[]{
                             new Entitlement.attributeDescriptorType(){
                                 attributeName = "OCS_ProductFamily",
                                  stringValue = ProductFamily
                                   
                             }
                        },
                       entitlementId = new idType
                       {
                        autoGenerateSpecified  = true,
                        autoGenerate = true,

                       }


                 }
            };
            var resp = fnoWs.createSimpleEntitlement(rqType);

            if(resp.statusInfo.status == Entitlement.StatusType.SUCCESS)
            {
                
              
               
                    entitlementId = resp.responseData[0].entitlementId;

            }
            return entitlementId;


           
      


        }

        public EntitlementLineItemResponse AddLineItemToEntitlement( string entitlementId, OrderEntitlementLineItem lineItem)
        {
            var lineItemResp = new EntitlementLineItemResponse();
            var fnoWs = new v1EntitlementOrderService();
            fnoWs.Url = EndPointUrl + "EntitlementOrderService";

            fnoWs.PreAuthenticate = true;
            CredentialCache credCache = new System.Net.CredentialCache();
            NetworkCredential netCred = new NetworkCredential(UserName, Password);
            credCache.Add(new Uri(fnoWs.Url), "Basic", netCred);
            fnoWs.Credentials = credCache;
            var entLineItem = new addOnlyEntitlementLineItemRequestType();
            entLineItem.opType = Entitlement.CreateOrUpdateOperationType.CREATE_OR_UPDATE;
            entLineItem.lineItem = new addEntitlementLineItemDataType[] {
                new addEntitlementLineItemDataType{
                     autoDeploy = true,
                      autoDeploySpecified = true,
                       entitlementIdentifier = new entitlementIdentifierType{
                            primaryKeys = new entitlementPKType
                            {
                                 entitlementId = entitlementId
                            }
                       },

                        lineItems = new createEntitlementLineItemDataType[]{
                             new createEntitlementLineItemDataType{
                                 activationId = new idType
                                   {

                                       autoGenerate = true,
                                       autoGenerateSpecified = true

                                   },
                                   isPermanent = lineItem.IsPerpertual,
                                   isPermanentSpecified = true,
                                   orderId = lineItem.ProductRatePlanChargeId,
                                   numberOfCopies = lineItem.Quantity.ToString(),

                                   partNumber = new partNumberIdentifierType
                                   {
                                       primaryKeys = new partNumberPKType {
                                           partId = lineItem.PartNumber
                                       }

                                   },
                                   startDate = lineItem.EffectiveDate,
                                   expirationDate =  lineItem.ExpirationDate,
                                   expirationDateSpecified = lineItem.IsPerpertual? false : true
                             }
                        }
                }
            };
            var resp = fnoWs.createEntitlementLineItem(entLineItem);
            
            if(resp.statusInfo.status == Entitlement.StatusType.SUCCESS)
            {
                lineItemResp.ActivationCode = resp.responseData[0].lineItemIdentifiers[0].primaryKeys.activationId;
                lineItemResp.EntitlementId = entitlementId;
                lineItemResp.EntitlementLineItemId = resp.responseData[0].lineItemIdentifiers[0].uniqueId;
               
            }

            return lineItemResp;
        }

        public List<string> CreateLicenseServer( string id, string companyName, string productLine, int qty=1, int currentDeviceCount = 0)
        {
            var devices = new List<string>();
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
                    publisherName = "oncenter",
                    deviceIdType = deviceIdTypeType.STRING,
                    deviceId = companyName + "-" + productLine +"-CLM-0" + (currentDeviceCount + 1).ToString(),
                    deviceIdTypeSpecified = true,
                   
                };
                deviceRq[i].deployment = deployment.CLOUD;
                deviceRq[i].deploymentSpecified = true;
                deviceRq[i].channelPartners = new Devices.channelPartnerDataType[1];
                deviceRq[i].channelPartners[0] = new Devices.channelPartnerDataType();
                deviceRq[i].channelPartners[0].organizationUnit = new Devices.organizationIdentifierType();
                deviceRq[i].channelPartners[0].organizationUnit.primaryKeys = new Devices.organizationPKType();
                deviceRq[i].channelPartners[0].organizationUnit.primaryKeys.name = id;
                deviceRq[i].channelPartners[0].currentOwner = true;
                deviceRq[i].channelPartners[0].currentOwnerSpecified = true;
                deviceRq[i].channelPartners[0].tierName = "bo.constants.partnertiernames.endcustomer";
                deviceRq[i].publisherIdName = new publisherIdentifier {
                     name = "OnCenter-Standard"
                };
                

            }
            var resp = fnoWs.createDevice(deviceRq);
            if (resp.statusInfo.status == OpsEmbeddedStatusType.SUCCESS)
            {
                foreach (var d in resp.responseData)
                    devices.Add(d.deviceId);
            }
            else
                throw new Exception(resp.failedData[0].reason);

            return devices;
        }

        public void AddEntitlementLineItemToLicenseServer( EntitlementLineItemResponse lineItem,
            string serverName)
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
            lineItemRq[0].deviceIdentifier =  GetLicenseServer(serverName);
         
            var linkLineItems = new linkLineItemDataType[1];
            linkLineItems[0] = new linkLineItemDataType();
            linkLineItems[0].lineItemIdentifier = new linkLineItemIdentifier
            {
                activationId = lineItem.ActivationCode,

                count = lineItem.TotalQty.ToString()

            };
          

           lineItemRq[0].lineItem = linkLineItems;
           var resp =  fnoWs.linkAddonLineItems(lineItemRq);
        }

        public List<string> GetLicenseServers(string accountNumber)
        {
            var respLicenseServers = new List<string>();
            var fnoWs = new v1ManageDeviceService();
            fnoWs.Url = EndPointUrl + "ManageDeviceService";

            fnoWs.PreAuthenticate = true;
            CredentialCache credCache = new System.Net.CredentialCache();
            NetworkCredential netCred = new NetworkCredential(UserName, Password);
            credCache.Add(new Uri(fnoWs.Url), "Basic", netCred);
            fnoWs.Credentials = credCache;
            var getDeviceRq = new getDevicesRequestType();
            getDeviceRq.batchSize = "100";
            getDeviceRq.pageNumber = "1";
            getDeviceRq.deviceResponseConfig = new deviceResponseConfigRequestType {
                soldTo = true,
                soldToSpecified = true,
                 
            
            };
            getDeviceRq.queryParams = new getDevicesParametersType {
                
                isServer = true,
                isServerSpecified = true,
                 organizationUnitName = new Devices.PartnerTierQueryType
                 {
                      partnerTier = "bo.constants.partnertiernames.endcustomer",
                        searchType = Devices.simpleSearchType.EQUALS,
                         value = accountNumber
                 }
              
            };
            var resp = fnoWs.getDevicesQuery(getDeviceRq);

            if(resp.statusInfo.status == OpsEmbeddedStatusType.SUCCESS)
            {
                foreach( var device in resp.responseData)
                {
                    if (device.soldTo != null)
                    {
                        if (device.soldTo.id == accountNumber)
                            respLicenseServers.Add(device.deviceIdentifier.deviceId);
                    }
                }
            }

            return respLicenseServers;
        }
       
        public deviceIdentifier GetLicenseServer(string deviceId)
        {
            var licenseServer = new deviceIdentifier();
            var fnoWs = new v1ManageDeviceService();
            fnoWs.Url = EndPointUrl + "ManageDeviceService";

            fnoWs.PreAuthenticate = true;
            CredentialCache credCache = new System.Net.CredentialCache();
            NetworkCredential netCred = new NetworkCredential(UserName, Password);
            credCache.Add(new Uri(fnoWs.Url), "Basic", netCred);
            fnoWs.Credentials = credCache;
            var getDeviceRq = new getDevicesRequestType();
            getDeviceRq.batchSize = "1";
            getDeviceRq.pageNumber = "1";
            getDeviceRq.deviceResponseConfig = new deviceResponseConfigRequestType
            {
                soldTo = true,
                soldToSpecified = true,


            };
            getDeviceRq.queryParams = new getDevicesParametersType
            {
                deviceId = new Devices.SimpleQueryType {
                     searchType = Devices.simpleSearchType.EQUALS,
                      value = deviceId
                },
            };
            var resp = fnoWs.getDevicesQuery(getDeviceRq);

            if (resp.statusInfo.status == OpsEmbeddedStatusType.SUCCESS)
            {
                licenseServer = resp.responseData[0].deviceIdentifier;
            }

            return licenseServer;
           
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
        public List<OrderEntitlement> GetEntitlements(string soldTo)
        {
            var entitlementList = new List<OrderEntitlement>();
            var searchQuery = new searchEntitlementRequestType
            {
                entitlementSearchCriteria = new searchEntitlementDataType
                {

                    soldTo = new Entitlement.SimpleQueryType
                     {
                          searchType = Entitlement.simpleSearchType.CONTAINS,
                          value = soldTo
                     },
                      
                     
                }
            };
            var fnoWs = new v1EntitlementOrderService();
            fnoWs.Url = EndPointUrl + "EntitlementOrderService";
            fnoWs.PreAuthenticate = true;
            fnoWs.Credentials = new NetworkCredential(UserName, Password);

            searchQuery.batchSize = "100";
            searchQuery.pageNumber = "1";
            var resp = fnoWs.getEntitlementsQuery(searchQuery);

            if (resp.statusInfo.status == Entitlement.StatusType.SUCCESS)
            {
                if (resp.entitlement == null)
                    return new List<OrderEntitlement>();

                foreach(var e in resp.entitlement)
                {
                    if (e.simpleEntitlement.soldTo != soldTo)
                        continue;

                    var entitlement = new OrderEntitlement();
                    entitlement.EntitlementId = e.simpleEntitlement.entitlementId.id;

                    if (e.simpleEntitlement.entitlementAttributes != null)
                    {
                        var productFamily = e.simpleEntitlement.entitlementAttributes.FirstOrDefault(j => j.attributeName == "OCS_ProductFamily");

                        if (productFamily != null)
                            entitlement.EntitlementFamily = productFamily.stringValue;
                    }

                    entitlement.LineItems = (from i in e.simpleEntitlement.lineItems
                                       select new OrderEntitlementLineItem
                                       {
                                          
                                           ActivationId = i.activationId.id,
                                           EffectiveDate = i.startDate,
                                           Quantity = int.Parse(i.numberOfCopies),
                                           EntitlementId = e.simpleEntitlement.entitlementId.id,
                                           ExpirationDate = i.expirationDate,
                                           PartNumber = i.partNumber.primaryKeys.partId,
                                           IsPerpertual = i.isPermanent

                                       }).ToList();

                    entitlementList.Add(entitlement);
                }

            }
            return entitlementList;
        }

        public bool  UpdateEntitlementLineItem(OrderEntitlementLineItem lineItem)
        {

            var fnoWs = new v1EntitlementOrderService();
            fnoWs.Url = EndPointUrl + "EntitlementOrderService";
            fnoWs.PreAuthenticate = true;
            CredentialCache credCache = new System.Net.CredentialCache();
            NetworkCredential netCred = new NetworkCredential(UserName, Password);
            credCache.Add(new Uri(fnoWs.Url), "Basic", netCred);
            fnoWs.Credentials = credCache;

            var updateRq = new updateEntitlementLineItemDataType();
            updateRq.entitlementIdentifier = new entitlementIdentifierType {
                 primaryKeys = new entitlementPKType {
                      entitlementId = lineItem.EntitlementId
                 }
            };

            updateRq.autoDeploy = true;
            updateRq.autoDeploySpecified = true;
            updateRq.lineItemData = new updateLineItemDataType[] {
                new updateLineItemDataType{
                   
                    activationId =  new idType{
                        id = lineItem.ActivationId,
                         autoGenerate = false,
                          autoGenerateSpecified = true
                        
                    },
                     numberOfCopies = lineItem.Quantity.ToString(),
                     partNumber = new partNumberIdentifierType
                     {
                          primaryKeys = new partNumberPKType
                          {
                              partId = lineItem.PartNumber
                          }
                     }
                }
            };
            
            var resp = fnoWs.updateEntitlementLineItem( new updateEntitlementLineItemDataType[] { updateRq });
            if (resp.statusInfo.status == Entitlement.StatusType.SUCCESS)
            {

                return true;
            }
            return false;
           

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

        public void Update(OrderEntitlementLineItem lineItem)
        {

            XNamespace soapenv = "http://schemas.xmlsoap.org/soap/envelope/";
            XNamespace urn = "urn:v1.webservices.operations.flexnet.com";
            XElement soapEnv = new XElement(soapenv + "Envelope",
                                 new XAttribute(XNamespace.Xmlns + "soapenv", "http://schemas.xmlsoap.org/soap/envelope/"),
                                 new XAttribute(XNamespace.Xmlns + "urn", "urn:v1.webservices.operations.flexnet.com"),
                                    new XElement(soapenv + "Header"),
                                    new XElement(soapenv + "Body",
                                        new XElement(urn + "updateEntitlementLineItemRequest",
                                            new XElement(urn + "lineItemData",
                                                new XElement(urn + "entitlementIdentifier",
                                                 new XElement(urn + "primaryKeys",
                                                    new XElement(urn + "entitlementId", lineItem.EntitlementId))),
                                                 new XElement(urn + "lineItemData",
                                                    new XElement(urn + "lineItemIdentifier",
                                                        new XElement(urn + "primaryKeys",
                                                        new XElement(urn + "activationId", lineItem.ActivationId))),
                                                    new XElement(urn + "numberOfCopies", lineItem.Quantity.ToString())),
                                                 new XElement(urn+ "autoDeploy", true)


                                             ))));
                                                


            var soapXml = soapEnv.ToString();
            var client = new RestClient(EndPointUrl + "EntitlementOrderService");
            var request = new RestRequest(Method.POST);
            byte[] credentialBuffer = new System.Text.UTF8Encoding().GetBytes(UserName + ":" + Password);
            var authorization = "Basic " + Convert.ToBase64String(credentialBuffer);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("accept", "application/json");
            request.AddHeader("pragma", "no-cache");
            request.AddHeader("soapaction", "updateEntitlementLineItem");
            request.AddHeader("authorization", authorization);
            request.AddHeader("content-type", "text/xml; charset=utf-8");
            request.AddParameter("text/xml; charset=utf-8", soapXml, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);




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


        public OCSLicense CreateTrialLicense(string partNumber, string servicePartNumber, string trialDays)
        {
            var trialAccountId = Guid.NewGuid().ToString();
            var id = CreateOrganization( trialDays + " Day TRIAL", trialAccountId);
            var entitlementId = CreateEntitlement(trialAccountId, "TRIAL");
            var activation = AddLineItemToEntitlement(entitlementId, new OrderEntitlementLineItem
            {
                IsPerpertual = false,
                EffectiveDate = DateTime.Now,
                ExpirationDate = DateTime.Now.AddDays(int.Parse(trialDays)),
                PartNumber = partNumber,
                Quantity = 1
            });

            var license = new OCSLicense
            {
                EntitlementId = entitlementId,
                ActivationId = activation.ActivationCode
            };

            if (!string.IsNullOrWhiteSpace(servicePartNumber))
            {
                var maintActivation = AddLineItemToEntitlement(entitlementId, new OrderEntitlementLineItem
                {
                    IsPerpertual = false,
                    EffectiveDate = DateTime.Now,
                    ExpirationDate = DateTime.Now.AddDays(int.Parse(trialDays)),
                    PartNumber = servicePartNumber,
                    Quantity = 1
                });

                license.MaintenanceActivationId = maintActivation.ActivationCode;
            }
            return license;

        }

     




    }
}
