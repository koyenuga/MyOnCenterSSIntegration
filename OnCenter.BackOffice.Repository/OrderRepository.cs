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
    public class OrderRepository : IRepository<Order>
    {
        ZuoraClient zuoraClient;
        FlexeraClient flexeraClient;
        AzureStorageTableClient azureClient;
        string OrderAzureContainer = "Orders";
        public OrderRepository()
        {
            zuoraClient = new ZuoraClient("kamar.oyenuga@oncenter.com", "@ncent3r@!", true);
            flexeraClient = new FlexeraClient("koyenuga@icitsolutions.com", "@Ncent3r@!");

            azureClient = new AzureStorageTableClient();
            azureClient.CreateTableIfNotExist(OrderAzureContainer);
        }
        public void Create(Order data)
        {
            zuoraClient.CreateSubscription(data);
            var entitlements = new List<IOrderEntitlement>();
            if (string.IsNullOrWhiteSpace(data.OrganizationId))
                data.OrganizationId = flexeraClient.CreateOrganization(data.CompanyName, data.AccountNumber);

            foreach(var i in data.LineItems)
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
            var resultEntitlements = flexeraClient.CreateEntitlement(data.OrderNumber, entitlements, data.OrganizationId,
                data.LicenseModel, data.AutoProvision);

            if (data.Entitlements == null)
                data.Entitlements = new List<IOrderEntitlement>();

            data.Entitlements.AddRange(entitlements);

           // azureClient.Save<Order>(new AzureStorageEntity<Order>(data.OrderNumber, data.AccountNumber, data), OrderAzureContainer);
          
        }

        public void Delete(object id)
        {
            throw new NotImplementedException();
        }

        public List<Order> Get()
        {
            throw new NotImplementedException();
        }

        public List<Order> Get<T2>(T2 data)
        {
            throw new NotImplementedException();
        }

        public Order Get(object id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Amend Order
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Order Update(Order data)
        {
            throw new NotImplementedException();
        }
    }
}
