using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oncenter.BackOffice.Entities;
using Oncenter.BackOffice.Entities.Flexera;
using Oncenter.BackOffice.Clients.Flexera.Entitlement;
using Oncenter.BackOffice.Clients.Flexera.UserOrganizationHierachy;

namespace Oncenter.BackOffice.Clients.Flexera
{
    public class FlexeraClient
    {
        public void CreateEntitlement(List<FlexeraEntitlement> entitlements, string organizationId)
        {
            List<createSimpleEntitlementDataType> flexeraEntitlements = new List<createSimpleEntitlementDataType>();

            foreach (var e in entitlements)
                flexeraEntitlements.Add(createEntitlementDataType(e, organizationId));

            var resp = new EntitlementOrderService().createSimpleEntitlement(flexeraEntitlements.ToArray());

            if (resp.statusInfo.status == Entitlement.StatusType.SUCCESS)
            {

            }
            else
                throw new Exception(resp.statusInfo.reason);

            //createEntitlementRequestype.lineItems[0].partNumber.
            //var c = new Oncenter.BackOffice.Clients.Flexera.Entitlement..createSimpleEntitlement()
            //https://flex1245-uat.flexnetoperations.com/flexnet/services/EntitlementOrderService?wsdl
            //https://flex1245-uat.flexnetoperations.com/flexnet/services/UserOrgHierarchyService?wsdl



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
    
        private createSimpleEntitlementDataType createEntitlementDataType(FlexeraEntitlement entitlement, string organizationId)
        {
            var csrtp = new createSimpleEntitlementDataType();
            
            csrtp.autoDeploy = entitlement.AutoProvision;
            csrtp.soldTo = organizationId;
           
            csrtp.lineItems = (from p in entitlement.LineItems
                               where p.IsMaintenanceItem == false
                               select new createEntitlementLineItemDataType
                               {
                                   orderLineNumber = p.ZuoraLineItemId,
                                   numberOfCopies = p.Quantity.ToString(),
                                   partNumber = new partNumberIdentifierType
                                   {
                                       uniqueId = p.PartNo

                                   },
                                   startDate = p.StartDate,
                                   expirationDate = p.ExpirationDate


                               }).ToArray();

            csrtp.maintenanceLineItems = (from p in entitlement.LineItems
                                          where p.IsMaintenanceItem == true
                                          select new createMaintenanceLineItemDataType
                                          {
                                              partNumber = new partNumberIdentifierType
                                              {
                                                  uniqueId = p.PartNo

                                              },
                                              startDate = p.StartDate,
                                              expirationDate = p.ExpirationDate


                                          }).ToArray();

            return csrtp;

        }

    }
}
