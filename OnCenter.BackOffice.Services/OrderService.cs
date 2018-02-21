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
using Oncenter.BackOffice.Entities.Accounts;
using Oncenter.BackOffice.Azure;
using Oncenter.BackOffice.Common.Extension;
using Oncenter.BackOffice.Azure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace OnCenter.BackOffice.Services
{
    public class OrderService : IOrderService
    {

        ISubscription SubscriptionManager;
        IProvisioner ProvisionManager;
        IStorage StorageManager;
        string storageName = "Subscriptions";
        public OrderService(ISubscription subscriptionManager, 
            IProvisioner provisionManager, 
            IStorage storageManager)
        {
            SubscriptionManager = subscriptionManager;
            ProvisionManager = provisionManager;
            StorageManager = storageManager;
        }
       
        public FulfillOrderResponse FulfillOrder(FulfillOrderRequest request)
        {
            FulfillOrderResponse response = new FulfillOrderResponse();
            
            dynamic zuoraResp = new ExpandoObject();
            var subscription = SubscriptionManager.Get(request.Account.AccountNumber);

            if (subscription != null)
                zuoraResp = SubscriptionManager.Amend(request, subscription);
            else
                zuoraResp = SubscriptionManager.Create(request);

            if (zuoraResp.Errors.Count == 0)
            {

                response.AccountNumber = zuoraResp.AccountNumber;
                request.Account.AccountNumber = zuoraResp.AccountNumber;
                request.Order.SubscriptionNumber = zuoraResp.SubscriptionNumber;
                response.SubscriptionNumber = zuoraResp.SubscriptionNumber;
                response.InvoiceNumber = zuoraResp.InvoiceNumber;
                response.InvoiceId = zuoraResp.InvoiceId;
                response.AccountId = zuoraResp.AccountId;
                response.InvoiceTotalAmount = zuoraResp.TotalAmount;
                response.InvoiceTaxAmount = zuoraResp.Tax;
                response.InvoiceBalance = zuoraResp.Balance;
                response.BillToContactId = zuoraResp.BillToId;
                response.SoldToContactId = zuoraResp.SoldToId;
                response.InvoiceLineItems = request.Order.LineItems;
                response.Entitlements = ProvisionManager.Provision(request);
                response.CloudLicenseServers = request.GetDevices();

                response.Successful = true;
                var ase = new AzureStorageEntity(response.SubscriptionNumber, response.AccountNumber, response.ToJsonString());
                new AzureSaveToTableStorageCommand(storageName).
                    Execute(ase);
                   
              

            }
            else
            {
                response.Successful = false;
                response.Errors = zuoraResp.Errors;
            }


            return response;
        }

        public OrderPaymentResponse ProcessPayment(OrderPaymentRequest request)
        {
            OrderPaymentResponse response = new OrderPaymentResponse();
            try
            {
                dynamic zuoraResp = SubscriptionManager.MakePayment(request);
                if (zuoraResp.Errors.Count == 0)
                {
                    response.PaymentId = zuoraResp.PaymentId;
                    response.Sucessful = true;
                }
                else
                {
                    response.Errors = zuoraResp.Errors;
                    response.Sucessful = false;
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return response;
        }

        public OrderDetail GetOrder(string accountNumber)
        {
            var order = new OrderDetail();
            var subscription = SubscriptionManager.Get(accountNumber);
            order.Account = new OncenterAccount();
            order.Account.AccountNumber = subscription.accountNumber;
            order.InvoiceOwnerAccountNumber = subscription.invoiceOwnerAccountNumber;
            order.SubscriptionNumber = subscription.subscriptionNumber;
            order.EffectiveDate = subscription.termStartDate;
            order.ExpirationDate = subscription.termEndDate;
            order.LineItems = new List<IOrderLineItem>();
            order.LineItems.AddRange(GetSubscriptionLineItems(subscription.ratePlans));
            var azureStorageInfo = GetLastResponseFromAzure(accountNumber, order.SubscriptionNumber);

            foreach(var item in order.LineItems)
            {
                var existingItem = azureStorageInfo.InvoiceLineItems.FirstOrDefault(i => i.ProductRatePlanChargeId == item.ProductRatePlanChargeId);
                if (existingItem != null)
                {
                    item.PartNo = existingItem.PartNo;
                    item.ProductFamily = existingItem.EntitlementFamily;
                    item.LicenseModel = existingItem.LicenseModel;
                }
               
                
            }
            return order;
            
        }

        public List<Order> GetOrders(string accountNumber)
        {
            throw new NotImplementedException();
        }

        FulfillOrderResponse GetLastResponseFromAzure(string accountNumber, string subscriptionNumber)
        {
            var accountInfo = new Tuple<string, string>(accountNumber, subscriptionNumber);
            var resp =  new AzureGetAccountFromTableStorageCommand<FulfillOrderResponse>(storageName).Execute(
                accountInfo);

            return resp;
        }

        List<OrderLineItem> GetSubscriptionLineItems(dynamic existingRatePlans)
        {
            
            List<OrderLineItem> lineItems = new List<OrderLineItem>();
            foreach (var xItem in existingRatePlans)
            {

                var prpId = xItem.productRatePlanId.ToString();
                var productName = xItem.productName.ToString();

                foreach (var cItem in xItem.ratePlanCharges)
                {
                    var productRatePlanCharge = SubscriptionManager.GetProductRatePlanCharge(cItem.productRatePlanChargeId.ToString());
                    if (productRatePlanCharge.ChargeType == "OneTime")
                        continue;

                    var lineItem = lineItems.FirstOrDefault(l => l.ProductRatePlanChargeId == cItem.productRatePlanChargeId.ToString()
                    && l.ProductName == productName && l.ProductRatePlanId == prpId);

                    if (lineItem == null)
                    {
                        lineItem = new OrderLineItem();
                        lineItem.ProductRatePlanId = prpId;
                        lineItem.ProductRatePlanChargeId = cItem.productRatePlanChargeId;
                        lineItem.ProductName = productName;
                        lineItem.NetSuitIntegrationId = productRatePlanCharge.IntegrationId__NS;

                        if (productRatePlanCharge.ChargeType == "Recurring")
                            lineItem.IsPerpetualLicense = false;
                        else
                            lineItem.IsPerpetualLicense = true;

                        lineItems.Add(lineItem);
                    }
                  
                    lineItem.Quantity += (cItem.quantity == null ? 0 : (int)cItem.quantity);
                    lineItem.Price += (cItem.price == null ? 0 : (double)cItem.price);


                   
                }
            }

            return lineItems;
        }

        public FulfillOrderResponse RenewOrder(FulfillOrderRequest request)
        {
            FulfillOrderResponse response = new FulfillOrderResponse();

            dynamic zuoraResp = new ExpandoObject();
            var subscription = SubscriptionManager.Get(request.Account.AccountNumber);

            if (subscription != null)
                zuoraResp = SubscriptionManager.Amend(request, subscription);
            else
            {

            }
           

            if (zuoraResp.Errors.Count == 0)
            {

                response.AccountNumber = zuoraResp.AccountNumber;
                request.Account.AccountNumber = zuoraResp.AccountNumber;
                request.Order.SubscriptionNumber = zuoraResp.SubscriptionNumber;
                response.SubscriptionNumber = zuoraResp.SubscriptionNumber;
                response.InvoiceNumber = zuoraResp.InvoiceNumber;
                response.InvoiceId = zuoraResp.InvoiceId;
                response.AccountId = zuoraResp.AccountId;
                response.InvoiceTotalAmount = zuoraResp.TotalAmount;
                response.InvoiceTaxAmount = zuoraResp.Tax;
                response.InvoiceBalance = zuoraResp.Balance;

                response.Entitlements = ProvisionManager.Provision(request);
                response.CloudLicenseServers = request.GetDevices();
                response.Successful = true;

                if (StorageManager != null)
                    StorageManager.Save<OrderDetail>(null);

            }
            else
            {
                response.Successful = false;
                response.Errors = zuoraResp.Errors;
            }


            return response;
        }
    }
}
