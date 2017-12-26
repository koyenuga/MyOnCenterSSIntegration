using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oncenter.BackOffice.Entities.Interfaces;
using Oncenter.BackOffice.Entities.Orders;

namespace Oncenter.BackOffice.Clients.Flexera
{
    public class FlexeraProvisioner : IProvisioner
    {
        FlexeraClient flexeraClient;
        public FlexeraProvisioner(string userName, string password, string endPointUrl)
        {
            flexeraClient = new FlexeraClient(userName, password, endPointUrl);
        }
        public List<EntitlementResponse>Provision(FulfillOrderRequest request)
        {
           
            var resultEntitlements = new List<EntitlementResponse>();
            var id = flexeraClient.GetOrganization(request.Account.AccountNumber);

            if (string.IsNullOrWhiteSpace(id))
            {
                id = flexeraClient.CreateOrganization(request.Account.CompanyName,
                request.Account.AccountNumber);
                var productFamilies = request.Order.LineItems.Select(c => new { c.EntitlementFamily, c.LicenseModel}).Distinct();

                foreach (var p in productFamilies)
                {
                    var orderEntitlement = new OrderEntitlement();
                    orderEntitlement.EntitlementFamily = p.EntitlementFamily;

                    orderEntitlement.Entitlements = (from i in request.Order.LineItems
                                                     where i.EntitlementFamily == p.EntitlementFamily
                                                     select new OrderEntitlementLineItem
                                                     {
                                                         PartNumber = i.PartNo,
                                                         Quantity = i.Quantity,
                                                         EffectiveDate = i.EffectiveDate,
                                                         ExpirationDate = i.ExpirationDate,
                                                         ProductRatePlanChargeId = i.ProductRatePlanChargeId,
                                                         IsPerpertual = i.IsMaintenanceItem,
                                                         Term = request.Order.Term


                                                     }).ToList();
                    if (orderEntitlement.Entitlements.Count > 0)
                    {
                        if (!string.IsNullOrWhiteSpace(p.EntitlementFamily))
                        {
                            var licenseModel = p.LicenseModel.Trim().ToLower() == "local" ?
                            Entities.LicenseModelType.LocalSingleSeat :
                            Entities.LicenseModelType.LocalMultiSeat;

                            var productEntitlements = flexeraClient.CreateEntitlement(request.Order.SubscriptionNumber,
                                                    orderEntitlement.Entitlements, request.Account.AccountNumber,
                                                    licenseModel, request.Order.Term);


                            resultEntitlements.Add(new EntitlementResponse
                            {
                                EntitlementFamily = p.EntitlementFamily,
                                Entitlements = productEntitlements

                            });
                        }


                    }
                }
                
            }


           

            return resultEntitlements;
        }
    }
}
