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

namespace Oncenter.BackOffice.Clients.Flexera
{
    public class FlexeraClient
    {
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

            var resp = new EntitlementOrderService().createSimpleEntitlement(flexeraEntitlements.ToArray());
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
            var resp = new EntitlementOrderService().getEntitlementsQuery(searchQuery);

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

        public void CreateOrganization(Account account)
        {
            var resp = new UserOrgHierarchyService().createOrganization(
                 new organizationDataType[] {
                     new organizationDataType{
                          orgType = OrgType.CUSTOMER,
                          name = account.CompanyName,
                          displayName = account.CompanyName,

                     }
                 });

            if (resp.statusInfo.status == UserOrganizationHierachy.StatusType.SUCCESS)
            {
                account.FlexeraOrganizationId = resp.responseData[0].uniqueId;
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
            csrtp.entitlementAttributes = new Entitlement.attributeDescriptorType[] {
                new Entitlement.attributeDescriptorType{
                    attributeName = "SubscriptionNumber",
                    stringValue = subscriptionNumber
                }
            };
            csrtp.lineItems = (from p in lineItems
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

        public List<FlexeraEntitlement> GetEntitlementByOrg(string organizationId)
        {
            var searchQuery = new searchEntitlementRequestType {
                 entitlementSearchCriteria = new searchEntitlementDataType {
                      soldTo = new Entitlement.SimpleQueryType
                      {

                           searchType = Entitlement.simpleSearchType.EQUALS,
                           value = organizationId
                            
                      }
                 }
            };
            var resp = new EntitlementOrderService().getEntitlementsQuery(searchQuery);

            if (resp.statusInfo.status == Entitlement.StatusType.SUCCESS)
            {
                return (from e in resp.entitlement
                        select new FlexeraEntitlement {
                            EntitlementId = e.simpleEntitlement.entitlementId.id,
                            OrganizationId = e.simpleEntitlement.soldTo,
                            
                            LineItems = (from li in e.simpleEntitlement.lineItems
                                         select new FlexeraEntitlementLineItem
                                         {
                                             ZuoraLineItemId = li.orderLineNumber,
                                             PartNo = li.partNumber.uniqueId,
                                             Quantity = li.numberOfCopies,
                                             StartDate = li.startDate,
                                             ExpirationDate = li.expirationDate
                                         }
                                         ).ToList(),


                        }).ToList();
            }
            throw new Exception();
        }

    }
}
