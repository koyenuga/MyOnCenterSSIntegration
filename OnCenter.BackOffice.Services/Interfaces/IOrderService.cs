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
        FulfillOrderResponse FulfillOrder(string accountNumber, FulfillOrderRequest request);
        AmendOrderResponse AmendOrder(AmendOrderRequest request);
        List<Order> GetOrders(string accountNumber);
        Order GetOrder(string orderId);

    }
}
