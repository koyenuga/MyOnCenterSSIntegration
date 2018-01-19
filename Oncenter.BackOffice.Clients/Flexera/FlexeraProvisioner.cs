using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oncenter.BackOffice.Entities.Interfaces;
using Oncenter.BackOffice.Entities.Orders;
using Oncenter.BackOffice.Clients.Flexera.Devices;
using Oncenter.BackOffice.Entities.License;

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

            List<string> LicenseServers = GetLicensServers(request);
            
            var productFamilies = request.Order.LineItems.Select(c => new { c.EntitlementFamily, c.IsSingleSeat }).Distinct();
            
            foreach (var p in productFamilies)
            {
                if (!string.IsNullOrWhiteSpace(p.EntitlementFamily))
                {
                    var orderEntitlement = new OrderEntitlement();
                    orderEntitlement.EntitlementFamily = p.EntitlementFamily;

                    orderEntitlement.LineItems = GetLineEntitlementLineItems(request, p.EntitlementFamily);

                    if (orderEntitlement.LineItems.Count > 0)
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
                            var entLiResp = new EntitlementLineItemResponse();
                            if (request.RequestType == FulfillmentRequestType.Renewal)
                            {
                                entLiResp = flexeraClient.AddLineItemToEntitlement(entResp.EntitlementId, li);
                                entLiResp.TotalQty = li.Quantity;
                            }
                            else
                            {
                                var existingLineItem = (from i in entitlementList
                                                        from j in i.LineItems
                                                        where j.PartNumber == li.PartNumber
                                                        select j).FirstOrDefault();

                                if (existingLineItem != null)
                                {
                                    existingLineItem.Quantity += li.Quantity;
                                    flexeraClient.Update(existingLineItem);
                                }
                                else
                                {
                                    entLiResp = flexeraClient.AddLineItemToEntitlement(entResp.EntitlementId, li);
                                    entLiResp.TotalQty = li.Quantity;
                                }
                            }
                           
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

        
        List<OrderEntitlementLineItem> GetLineEntitlementLineItems(FulfillOrderRequest request, string entitlementFamily)
        {
            return (from i in request.Order.LineItems
                    where i.EntitlementFamily == entitlementFamily
                    & i.IsCloudLicenseServer == false
                    & (i.IsPerpetualLicense == true
                    || i.IsMaintenanceItem == true)
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
        }

        List<string> GetLicensServers(FulfillOrderRequest request)
        {
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

            return LicenseServers;
        }

        public OCSLicense ProvisionTrialLicense(string trialDays, string partNumber, string servicePartNumber="")
        {
            return flexeraClient.CreateTrialLicense(partNumber, servicePartNumber, trialDays);
        }

        public string GetEntitlement(string id)
        {
            return flexeraClient.search(id);
        }
    }
}
