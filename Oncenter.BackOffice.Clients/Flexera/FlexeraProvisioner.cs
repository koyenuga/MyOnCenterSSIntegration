using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oncenter.BackOffice.Entities.Interfaces;
using Oncenter.BackOffice.Entities.Orders;

namespace Oncenter.BackOffice.Clients.Flexera
{
    public class FlexeraProvisioner : IProvisioner
    {
        public List<OrderEntitlement>Provision(FulfillOrderRequest request)
        {
            //throw new NotImplementedException();

            return new List<OrderEntitlement>();
        }
    }
}
