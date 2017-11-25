using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oncenter.BackOffice.Entities.Interfaces;

namespace Oncenter.BackOffice.Entities.Orders
{
    public class AmendOrderRequest : OrderRequestBase
    {
     
        public OrderAmendmentType AmendmentType { get; set; }
        public DateTime EffectiveDate { get; set; }
        public List<IOrderLineItem> LineItems { get; set; }

    }

    public enum OrderAmendmentType
    {
        Adjustment,
        Renewal,
        Cancellation
    }
}
