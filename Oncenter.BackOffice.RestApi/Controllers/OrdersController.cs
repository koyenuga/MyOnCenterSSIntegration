﻿using System;
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
using Oncenter.BackOffice.Entities.License;

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
        /// Get Account Order
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <returns></returns>
        [Route("Account/{accountNumber}/Subscription")]
        public OrderDetail Get(string accountNumber)
        {
            return Service.GetOrder(accountNumber);
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
        // POST: api/License
        [Route("Order/TRIAL")]
        [HttpPost]
        public OCSLicense Get(string pn, string spn, string pf = "", string an = "", string cn = "", int qty = 1, int trialdays = 14, bool networked = false)
        {

            return new FlexeraProvisioner(
                ConfigurationManager.AppSettings["FNOUserName"],
                ConfigurationManager.AppSettings["FNOPassword"],
                ConfigurationManager.AppSettings["FNOEnvUrl"])
                .ProvisionTrialLicense(trialdays, pn, spn, cn, an, pf, qty, networked);
        }

        [Route("Order/TRIAL/OST")]
        public OCSLicense GetOSTTrialLicense(int trialdays)
        {

            return new FlexeraProvisioner(
                ConfigurationManager.AppSettings["FNOUserName"],
                ConfigurationManager.AppSettings["FNOPassword"],
                ConfigurationManager.AppSettings["FNOEnvUrl"])
                .ProvisionTrialLicense(trialdays, "OSTLocal3.95", "OSTLocalServices", "", "", "OST-Local", 1, false);
        }



    }
}