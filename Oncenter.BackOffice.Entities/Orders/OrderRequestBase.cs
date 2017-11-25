using Oncenter.BackOffice.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oncenter.BackOffice.Entities.Orders
{
    public class OrderRequestBase
    {
        public string Id { get; set; }
        public string OrderNumber { get; set; }
        public string AccountNumber { get; set; }
    }
}
