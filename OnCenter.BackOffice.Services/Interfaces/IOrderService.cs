using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Oncenter.BackOffice.Entities.Orders;
using OnCenter.BackOffice.Repository.Interfaces;
namespace OnCenter.BackOffice.Services.Interfaces
{
    public interface IOrderService
    {
        CreateOrderResponse CreateNewOrder(Order request, IRepository<Order> repo);
        AmendOrderResponse AmendOrder(AmendOrderRequest request, IRepository<Order> repo);

    }
}
