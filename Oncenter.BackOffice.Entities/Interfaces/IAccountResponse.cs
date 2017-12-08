using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oncenter.BackOffice.Entities.Interfaces
{
    public interface IAccountResponse
    {
         string BillToId { get; set; }
         string SoldToId { get; set; }
    }
}
