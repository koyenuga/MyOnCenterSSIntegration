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
        public List<OrderEntitlement>Provision(FulfillOrderRequest request)
        {
            var id = flexeraClient.CreateOrganization(request.Account.CompanyName, 
                request.Account.AccountNumber);

            var entitlements = new List<IOrderEntitlement>();
            foreach (var i in request.Order.LineItems)
            {
                entitlements.Add(new OrderEntitlement
                {
                    PartNumber = i.PartNo,
                    Quantity = i.Quantity,
                    EffectiveDate = i.EffectiveDate,
                    ExpirationDate = i.ExpirationDate,
                    ProductRatePlanChargeId = i.ProductRatePlanChargeId,


                });
            }
            var resultEntitlements = flexeraClient.CreateEntitlement(request.Order.SubscriptionNumber,
                entitlements, id, request.Order.LicenseModel);

            return resultEntitlements;
        }
    }
}
