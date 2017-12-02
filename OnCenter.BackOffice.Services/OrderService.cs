using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnCenter.BackOffice.Repository.Interfaces;
using OnCenter.BackOffice.Services.Interfaces;
using Oncenter.BackOffice.Entities;
using Oncenter.BackOffice.Entities.Interfaces;
using Oncenter.BackOffice.Entities.Orders;

namespace OnCenter.BackOffice.Services
{
    public class OrderService : IOrderService
    {

        private IRepository<Order> Repo;
        public OrderService(IRepository<Order> repo)
        {
            Repo = repo;
        }
        public AmendOrderResponse AmendOrder(AmendOrderRequest request)
        {
            throw new NotImplementedException();
        }

        public FulfillOrderResponse FulfillOrder( string accountNumber, FulfillOrderRequest request)
        {
            //Order order = new Order();
            //order.AccountNumber = accountNumber;
            //order.AutoProvision = request.AutoProvision;
            //order.CompanyName = request.CompanyName;
            //order.EffectiveDate = request.EffectiveDate;
            //order.ExpirationDate = request.ExpirationDate;
            //order.LicenseModel = request.LicenseModel;
            //order.LineItems = new List<IOrderLineItem>();
            //order.LineItems.AddRange(request.LineItems);
            //order.Term = request.Term;
            //order.TermType = request.TermType;
            //Repo.Create(order);

            return new FulfillOrderResponse
            {
                 
            };
        }

        public Order GetOrder(string orderId)
        {
            throw new NotImplementedException();
        }

        public List<Order> GetOrders(string accountNumber)
        {
            return Repo.Get<string>(accountNumber);
        }
    }
}
