using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using OnCenter.BackOffice.Repository.Interfaces;
using OnCenter.BackOffice.Services.Interfaces;
using Oncenter.BackOffice.Entities;
using Oncenter.BackOffice.Entities.Interfaces;
using Oncenter.BackOffice.Entities.Orders;
using System.Dynamic;

namespace OnCenter.BackOffice.Services
{
    public class OrderService : IOrderService
    {

        ISubscription SubscriptionManager;
        IProvisioner ProvisionManager;
        IStorage StorageManager;
        public OrderService(ISubscription subscriptionManager, 
            IProvisioner provisionManager, 
            IStorage storageManager)
        {
            SubscriptionManager = subscriptionManager;
            ProvisionManager = provisionManager;
            StorageManager = storageManager;
        }
        public AmendOrderResponse AmendOrder(AmendOrderRequest request)
        {
            throw new NotImplementedException();
        }

        public FulfillOrderResponse FulfillOrder(FulfillOrderRequest request)
        {
            FulfillOrderResponse response = new FulfillOrderResponse();

            using (var scope = new TransactionScope())
            {
                try
                {

                    dynamic zuoraResp = new ExpandoObject();
                    var subscription = SubscriptionManager.Get(request.Account.AccountNumber);

                    if (subscription != null)
                        zuoraResp = SubscriptionManager.Amend(request, subscription);
                    else
                    {
                        zuoraResp = SubscriptionManager.Create(request);
                        response.AccountNumber = zuoraResp[0].AccountNumber;
                        request.Account.AccountNumber = zuoraResp[0].AccountNumber;
                        request.Order.SubscriptionNumber = zuoraResp[0].SubscriptionNumber;
                        response.SubscriptionNumber = zuoraResp[0].SubscriptionNumber;
                        response.InvoiceNumber = zuoraResp[0].InvoiceNumber;
                    }

                    var entitlements = ProvisionManager.Provision(request);

                    if (StorageManager != null)
                        StorageManager.Save<OrderDetail>(null);

                    scope.Complete();


                }


                catch (Exception e)
                {
                    throw e;
                }

            }

            return response;
        }

        public IOrder GetOrder(string orderId)
        {
            throw new NotImplementedException();
        }

        public List<IOrder> GetOrders(string accountNumber)
        {
            throw new NotImplementedException();
        }
    }
}
