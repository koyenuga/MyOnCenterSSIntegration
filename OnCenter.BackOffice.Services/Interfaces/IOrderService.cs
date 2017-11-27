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
        CreateOrderResponse CreateNewOrder(string accountNumber, CreateOrderRequest request);
        AmendOrderResponse AmendOrder(AmendOrderRequest request);

    }
}
