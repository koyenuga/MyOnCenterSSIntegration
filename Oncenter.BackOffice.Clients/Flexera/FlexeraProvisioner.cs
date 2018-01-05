using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oncenter.BackOffice.Entities.Interfaces;
using Oncenter.BackOffice.Entities.Orders;
using Oncenter.BackOffice.Clients.Flexera.Devices;

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

            var entitlementList = flexeraClient.GetEntitlements(request.Account.AccountNumber);

            List<string> LicenseServers = new List<string>();
            LicenseServers = flexeraClient.GetLicenseServers(request.Account.AccountNumber);
            var licenServerItems = request.Order.LineItems.Where(l => l.IsCloudLicenseServer).ToList();

            foreach (var ls in licenServerItems)
            {
                var device = flexeraClient.CreateLicenseServer(request.Account.AccountNumber,
                     request.Account.CompanyName,
                     ls.EntitlementFamily, ls.Quantity, LicenseServers.Count);
                LicenseServers.AddRange(device);
            }

            var productFamilies = request.Order.LineItems.Select(c => new { c.EntitlementFamily, c.IsSingleSeat }).Distinct();


            foreach (var p in productFamilies)
            {
                var orderEntitlement = new OrderEntitlement();
                orderEntitlement.EntitlementFamily = p.EntitlementFamily;

                orderEntitlement.LineItems = (from i in request.Order.LineItems
                                                 where i.EntitlementFamily == p.EntitlementFamily
                                                 && i.IsCloudLicenseServer == false
                                                 select new OrderEntitlementLineItem
                                                 {
                                                     PartNumber = i.PartNo,
                                                     Quantity = i.Quantity,
                                                     EffectiveDate = i.EffectiveDate,
                                                     ExpirationDate = i.ExpirationDate,
                                                     ProductRatePlanChargeId = i.ProductRatePlanChargeId,
                                                     IsPerpertual = i.IsPerpetualLicense,
                                                     Term = request.Order.Term,
                                                     LicenseManagerId = i.CloudLicenseServerName


                                                 }).ToList();
                if (orderEntitlement.LineItems.Count > 0)
                {
                    if (!string.IsNullOrWhiteSpace(p.EntitlementFamily))
                    {
                        var entResp = new EntitlementResponse();
                        if (p.IsSingleSeat)
                            entResp.EntitlementId = flexeraClient.CreateEntitlement(request.Account.AccountNumber, p.EntitlementFamily);
                        else
                            entResp.EntitlementId = GetProductFamilyEntitlementId(entitlementList, request.Account.AccountNumber,
                                request.Account.CompanyName, p.EntitlementFamily);

                        entResp.EntitlementFamily = p.EntitlementFamily;

                        entResp.EntitlementLineItems = new List<EntitlementLineItemResponse>();
                        foreach (var li in orderEntitlement.LineItems)
                        {
                            var entLiResp = flexeraClient.AddLineItemToEntitlement(entResp.EntitlementId, li);
                            entLiResp.TotalQty = li.Quantity;
                            entLiResp.CloudLicenseServerId = li.LicenseManagerId;
                            
                            if (LicenseServers.Count > 0)
                            {
                                var cls = LicenseServers.FirstOrDefault(ls => ls == entLiResp.CloudLicenseServerId);
                                if (string.IsNullOrWhiteSpace(cls))
                                    cls = LicenseServers.Last();

                                flexeraClient.AddEntitlementLineItemToLicenseServer(entLiResp, cls);
                                entLiResp.CloudLicenseServerId = cls;

                            }
                            entResp.EntitlementLineItems.Add(entLiResp);



                          
                        }
                        resultEntitlements.Add(entResp);


                    }
                }




            }

            request.setDevices(LicenseServers);

            return resultEntitlements;
        }

        string GetProductFamilyEntitlementId(List<OrderEntitlement> entitlements, string accountNumber, string companyName, string ocsProductFamily)
        {
            
            if (entitlements != null)
            {
                foreach(var ent in entitlements)
                {
                    if (ent.EntitlementFamily == ocsProductFamily)
                        return ent.EntitlementId;
                }
            }
            var id = flexeraClient.GetOrganization(accountNumber);

            if (string.IsNullOrWhiteSpace(id))
            {
                id = flexeraClient.CreateOrganization(companyName,accountNumber);

            }
            return flexeraClient.CreateEntitlement(accountNumber, ocsProductFamily);
        }
        
    }
}
