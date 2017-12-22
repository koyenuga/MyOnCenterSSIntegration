using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;
using Microsoft.WindowsAzure.Storage.Table;


namespace Oncenter.BackOffice.Entities.Interfaces
{
    public interface IOrder 
    {
        string Id { get; set; }
        string OrderNumber { get; set; }
        string Term { get; set; }
        string TermType { get; set; }
        DateTime EffectiveDate { get; set; }
        DateTime ExpirationDate { get; set; }
        List<IOrderLineItem> LineItems { get; set; }
        List<IOrderEntitlement> Entitlements { get; set; }
      


    }
}
