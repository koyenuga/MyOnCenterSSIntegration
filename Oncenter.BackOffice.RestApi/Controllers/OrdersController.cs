using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Oncenter.BackOffice.Entities;
using Oncenter.BackOffice.Entities.Orders;
using OnCenter.BackOffice.Services;
using OnCenter.BackOffice.Services.Interfaces;
using OnCenter.BackOffice.Repository;
using OnCenter.BackOffice.Repository.Interfaces;



namespace Oncenter.BackOffice.RestApi.Controllers
{
    public class OrdersController : ApiController
    {
        private IOrderService Service;
        public OrdersController()
        {
            Service = new OrderService(new OrderRepository());
           
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
        public IEnumerable<Order> Get()
        {
            return new List<Order>();
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
        [Route("Account/{accountNumber}/Order/")]
        public CreateOrderResponse Post(string accountNumber, CreateOrderRequest request)
        {
            return Service.CreateNewOrder(accountNumber, request);
        }

        [Route("Account/{accountNumber}/Order/{subscriptionNumer}/Amend")]
        public AmendOrderResponse Put(string accountNumber, string subscriptionNumer, AmendOrderRequest request)
        {
            return new AmendOrderResponse();
        }

        
    }
}