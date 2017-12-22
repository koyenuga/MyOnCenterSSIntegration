using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Oncenter.BackOffice.Entities.Orders;

namespace Oncenter.BackOffice.Entities.Interfaces
{
    public interface IOrderEntitlement
    {
       
        string EntitlementFamily { get; set; }
        List<OrderEntitlementLineItem> Entitlements { get; set; }

    }
}
