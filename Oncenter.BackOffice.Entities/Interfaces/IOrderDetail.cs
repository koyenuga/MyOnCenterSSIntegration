using Oncenter.BackOffice.Entities.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oncenter.BackOffice.Entities.Interfaces
{
    public interface IOrderDetail : IOrder, IOrderRequest
    {
        OncenterAccount Account { get; set; }
    }
}
