﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oncenter.BackOffice.Entities.Interfaces
{
    public interface IAccountOrders : IAccount
    {
        List<IOrder> Orders { get; set; }
    }
}
