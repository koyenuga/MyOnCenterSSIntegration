﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oncenter.BackOffice.Clients.Interfaces
{
    public interface IBackOfficeProcess<T, S>
    {
        T Process(S request);
    }
}