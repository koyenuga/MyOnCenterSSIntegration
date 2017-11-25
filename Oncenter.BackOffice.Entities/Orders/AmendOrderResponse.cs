using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oncenter.BackOffice.Entities.Orders
{
    public class AmendOrderResponse : OrderRequestBase
    {
        public bool OrderRenewed { get; set; }
        public DateTime OrderRenewedDate { get; set; }

        public bool OrderCancelled { get; set; }
        public DateTime OrderCancelledDate { get; set; }
    }
}
