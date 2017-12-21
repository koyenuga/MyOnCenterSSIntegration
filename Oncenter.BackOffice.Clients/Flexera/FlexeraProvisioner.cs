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
        public List<string>Provision(FulfillOrderRequest request)
        {
            var entitlements = new List<IOrderEntitlement>();
            var resultEntitlements = new List<string>();
            var id = flexeraClient.GetOrganization(request.Account.CompanyName,
                request.Account.AccountNumber);

            if (string.IsNullOrWhiteSpace(id))
            {
                id = flexeraClient.CreateOrganization(request.Account.CompanyName,
                request.Account.AccountNumber);
                foreach (var i in request.Order.LineItems)
                {
                    if (!string.IsNullOrWhiteSpace(i.PartNo))
                    {
                        entitlements.Add(new OrderEntitlement
                        {
                            PartNumber = i.PartNo,
                            Quantity = i.Quantity,
                            EffectiveDate = i.EffectiveDate,
                            ExpirationDate = i.ExpirationDate,
                            ProductRatePlanChargeId = i.ProductRatePlanChargeId,
                            IsPerpertual = i.IsPerpetualLicense


                        });
                    }
                }
            }


            if (entitlements.Count > 0)
            {
                resultEntitlements = flexeraClient.CreateEntitlement(request.Order.SubscriptionNumber,
                    entitlements, request.Account.AccountNumber, request.Order.LicenseModel);
            }

            return resultEntitlements;
        }
    }
}
