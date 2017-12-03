using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Oncenter.BackOffice.Entities.Orders;
using OnCenter.BackOffice.Repository.Interfaces;
using Oncenter.BackOffice.Entities.Interfaces;

namespace OnCenter.BackOffice.Services.Interfaces
{
    public interface IOrderService
    {
        FulfillOrderResponse FulfillOrder(FulfillOrderRequest request);
        AmendOrderResponse AmendOrder(AmendOrderRequest request);
        List<IOrder> GetOrders(string accountNumber);
        IOrder GetOrder(string orderId);

    }
}
