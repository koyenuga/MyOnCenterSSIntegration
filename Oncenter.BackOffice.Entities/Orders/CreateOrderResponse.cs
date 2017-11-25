using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oncenter.BackOffice.Entities.Flexera;
using Oncenter.BackOffice.Entities.Zuora;

namespace Oncenter.BackOffice.Entities.Orders
{
    public class CreateOrderResponse
    {
        public string AccountNumber { get; set; }
        public List<Order> Subscriptions { get; set; }
        public List<Order> Entitlements { get; set; }
    }
}
