using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Oncenter.BackOffice.Entities;
using Oncenter.BackOffice.Entities.Interfaces;
using Oncenter.BackOffice.Entities.Orders;
using OnCenter.BackOffice.Repository.Interfaces;
using Oncenter.BackOffice.Clients.Zuora;
using Oncenter.BackOffice.Clients.Flexera;
using Oncenter.BackOffice.Clients.Azure;

namespace OnCenter.BackOffice.Repository
{
    public class OrderRepository : IRepository<IOrder>
    {
        ZuoraClient zuoraClient;
        FlexeraClient flexeraClient;
        AzureStorageTableClient azureClient;
        string OrderAzureContainer = "Orders";
        public OrderRepository()
        {
            zuoraClient = new ZuoraClient("kamar.oyenuga@oncenter.com", "@ncent3r@!", "");
            //flexeraClient = new FlexeraClient("koyenuga@icitsolutions.com", "@Ncent3r@!");

            azureClient = new AzureStorageTableClient();
            azureClient.CreateTableIfNotExist(OrderAzureContainer);
        }
        public void Create(IOrder data, string accountNumber)
        {
            //zuoraClient.CreateSubscription(data, accountNumber);
            var entitlements = new List<IOrderEntitlement>();
            //if (string.IsNullOrWhiteSpace(data.OrganizationId))
            //    data.OrganizationId = flexeraClient.CreateOrganization(data.CompanyName, data.AccountNumber);

            //foreach (var i in data.LineItems)
            //{
            //    entitlements.Add(new OrderEntitlement
            //    {
            //        PartNumber = i.PartNo,
            //        Quantity = i.Quantity,
            //        EffectiveDate = i.EffectiveDate,
            //        ExpirationDate = i.ExpirationDate,
            //        ProductRatePlanChargeId = i.ProductRatePlanChargeId,


            //    });
            //}
            //var resultEntitlements = flexeraClient.CreateEntitlement(data.OrderNumber, entitlements, data.OrganizationId,
            //    data.LicenseModel, data.AutoProvision);

            //if (data.Entitlements == null)
            //    data.Entitlements = new List<IOrderEntitlement>();

            //data.Entitlements.AddRange(entitlements);

            //azureClient.Save<Order>(new AzureStorageEntity<Order>(data.OrderNumber, data.AccountNumber, data), OrderAzureContainer);

        }

        public void Delete(object id)
        {
            throw new NotImplementedException();
        }

        public List<IOrder> Get()
        {
            throw new NotImplementedException();
        }

      
        public List<IOrder> Get<T2>(T2 data)
        {
            dynamic result = zuoraClient.GetAccountSubscriptions(data.ToString());
            List<IOrder> subscriptions = new List<IOrder>();

            if (result.success == true)
            {
                foreach (var s in result.subscriptions)
                {
                    subscriptions.Add(new Order
                    {
                        Id = s.id,
                        
                        EffectiveDate = s.contractEffectiveDate,
                        OrderNumber = s.subscriptionNumber,
                        ExpirationDate = s.termEndDate,
                        Term = s.currentTerm,
                        TermType = s.termType,
                        LineItems = GetLineItems(s.ratePlans),

                    });
                }
            }
            return subscriptions;

        }
        private List<IOrderLineItem> GetLineItems(dynamic items)
        {
            var lineItems = new List<IOrderLineItem>();
            foreach(var item in items)
            {
                var lineItem = new OrderLineItem();
                lineItem.ProductName = item.productName;
                lineItem.ProductRatePlanId = item.productRatePlanId;
                lineItem.ProductRatePlanChargeId = item.ratePlanCharges[0].productRatePlanChargeId;
                lineItem.Quantity = item.ratePlanCharges[0].quantity;
                lineItem.Id = item.ratePlanCharges[0].id;
                lineItem.EffectiveDate = item.ratePlanCharges[0].effectiveStartDate;
                lineItem.ExpirationDate = item.ratePlanCharges[0].effectiveEndDate;
                lineItems.Add(lineItem);
            }

            return lineItems;
        }
        public IOrder Get(object id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Amend Order
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public IOrder Update(IOrder data)
        {
            throw new NotImplementedException();
        }

        public void Create(IOrder data)
        {
            throw new NotImplementedException();
        }
    }
}
