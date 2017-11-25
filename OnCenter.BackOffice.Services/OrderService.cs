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
        public AmendOrderResponse AmendOrder(AmendOrderRequest request, IRepository<Order> repo)
        {
            throw new NotImplementedException();
        }

        public CreateOrderResponse CreateNewOrder(Order request, IRepository<Order> repo)
        {
            repo.Create(request);

            return new CreateOrderResponse {
                 
            };
        }
    }
}
