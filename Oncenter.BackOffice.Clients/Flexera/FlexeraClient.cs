using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oncenter.BackOffice.Entities;
using Oncenter.BackOffice.Entities.Interfaces;
using Oncenter.BackOffice.Entities.Orders;
using Oncenter.BackOffice.Clients.Flexera.Entitlement;
using Oncenter.BackOffice.Clients.Flexera.UserOrganizationHierachy;
using System.Net;

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
            string organizationId, LicenseModelType licenseModel, bool autoProvision=true)
        {
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
            fnoWs.PreAuthenticate = true;
            fnoWs.Credentials = new NetworkCredential(UserName, Password);
            //var simpleEntitlementRqType = new createBulkEntitlementDataType
           // simpleEntitlementRqType.simpleEntitlement = flexeraEntitlements.ToArray();
            var resp = fnoWs.createBulkEntitlement(flexeraEntitlements.ToArray());

            var results = new List<OrderEntitlement>();
            if (resp.statusInfo.status == Entitlement.StatusType.SUCCESS)
            {
                foreach(var e in resp.responseData)
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
    
        public createBulkEntitlementDataType BuildEntitlementRequest(List<IOrderEntitlement> lineItems,
            string organizationId, string subscriptionNumber,
            string qty ="", 
            bool autoProvision =true)
        {

            var csrtp = new createBulkEntitlementDataType();
            csrtp.autoDeploy = autoProvision;
            csrtp.soldTo = organizationId;
            //csrtp..entitlementAttributes = new Entitlement.attributeDescriptorType[] {
            //    new Entitlement.attributeDescriptorType{
            //        attributeName = "SubscriptionNumber",
            //        stringValue = subscriptionNumber
            //    }
            //};
            csrtp.ilineItems = (from p in lineItems
                               select new createEntitlementLineItemDataType
                               {

                                   orderId = p.ProductRatePlanChargeId,
                                   numberOfCopies = string.IsNullOrWhiteSpace(qty)? p.Quantity.ToString() : qty,
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
