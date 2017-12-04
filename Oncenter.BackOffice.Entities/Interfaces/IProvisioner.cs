using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oncenter.BackOffice.Entities.Orders;

namespace Oncenter.BackOffice.Entities.Interfaces
{
    public interface IProvisioner
    {
       List<OrderEntitlement> Provision(FulfillOrderRequest request);
    }
}
