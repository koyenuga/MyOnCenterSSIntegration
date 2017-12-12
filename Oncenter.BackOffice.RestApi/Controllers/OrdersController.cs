using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Configuration;
using Oncenter.BackOffice.Entities;
using Oncenter.BackOffice.Entities.Orders;
using OnCenter.BackOffice.Services;
using OnCenter.BackOffice.Services.Interfaces;
using OnCenter.BackOffice.Repository;
using OnCenter.BackOffice.Repository.Interfaces;
using Oncenter.BackOffice.Clients.Zuora;
using Oncenter.BackOffice.Clients.Flexera;



namespace Oncenter.BackOffice.RestApi.Controllers
{
    public class OrdersController : ApiController
    {
        private IOrderService Service;
        public OrdersController()
        {
            Service = new OrderService( new ZuoraSubscription(
                ConfigurationManager.AppSettings["ZuoraUserName"], 
                ConfigurationManager.AppSettings["ZuoraPassword"],
                ConfigurationManager.AppSettings["ZuoraEnvUrl"]),
                new FlexeraProvisioner(ConfigurationManager.AppSettings["FNOUserName"],
                ConfigurationManager.AppSettings["FNOPassword"],
                ConfigurationManager.AppSettings["FNOEnvUrl"]), null);
           
        }
        public OrdersController(IOrderService orderService)
        {
            Service = orderService;
        }

        /// <summary>
        /// Get All Account Orders/Subscription
        /// </summary>
        /// <returns></returns>
        [Route("Account/{accountNumber}/Orders")]
        public IEnumerable<Entities.Interfaces.IOrder> Get(string accountNumber)
        {
            return Service.GetOrders(accountNumber);
        }

        /// <summary>
        /// Get Account Order
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <param name="subscriptionNumer"></param>
        /// <returns></returns>
        [Route("Account/{accountNumber}/Order/{subscriptionNumer}")]
        public Order Get(string accountNumber, string subscriptionNumer)
        {
            return new Order();
        }
        /// <summary>
        /// Create New Order/Subscription
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        [Route("Order/Fulfill")]
        public FulfillOrderResponse Post(FulfillOrderRequest request)
        {
            return Service.FulfillOrder(request);
        }
        [Route("Order/Payment")]
        [HttpPost]
        public OrderPaymentResponse PostPayment(OrderPaymentRequest request)
        {
            return Service.ProcessPayment(request);
        }

        [Route("Account/{accountNumber}/Order/{subscriptionNumer}/Amend")]
        public AmendOrderResponse Put(string accountNumber, string subscriptionNumer, AmendOrderRequest request)
        {
            return new AmendOrderResponse();
        }

        
    }
}