﻿using System;
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

                response.Entitlements = ProvisionManager.Provision(request);
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
            //order.
            order.LineItems = new List<IOrderLineItem>();
            order.LineItems.AddRange(GetSubscriptionLineItems(subscription.ratePlans));
            return order;
            
        }

        public List<Order> GetOrders(string accountNumber)
        {
            throw new NotImplementedException();
        }

        List<OrderLineItem> GetSubscriptionLineItems(dynamic existingRatePlans)
        {
            List<OrderLineItem> lineItems = new List<OrderLineItem>();
            foreach (var xItem in existingRatePlans)
            {

                var prpId = xItem.productRatePlanId;
                foreach (var cItem in xItem.ratePlanCharges)
                {
                    var lineItem = new OrderLineItem();

                    lineItem.ProductRatePlanId = prpId;


                    lineItems.Add(lineItem);
                }
            }

            return lineItems;
        }
    }
}
