using Oncenter.BackOffice.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oncenter.BackOffice.Entities.Accounts;
using Oncenter.BackOffice.Entities.Contacts;

namespace Oncenter.BackOffice.Entities.Orders
{
    public class FulfillOrderRequest
    {
        public OncenterAccountRequest Account { get; set; }  
        public IOrderRequest Order { get; set; }

        public OncenterContact BillToContact { get; set; }

    }
}
