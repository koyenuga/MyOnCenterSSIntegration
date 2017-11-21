using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oncenter.BackOffice.Entities.Zuora;
using Oncenter.BackOffice.Entities.Interfaces;
using Oncenter.BackOffice.Entities.Flexera;

namespace Oncenter.BackOffice.Entities
{
    public class UpdateSubscriptionRequestType
    {
        public ZuoraSubscription Subscription { get; set; }
    }
}
