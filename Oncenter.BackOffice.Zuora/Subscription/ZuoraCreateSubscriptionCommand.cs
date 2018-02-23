using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oncenter.BackOffice.Entities.Interfaces;
using Oncenter.BackOffice.Zuora.Entities;

namespace Oncenter.BackOffice.Zuora.Subscription
{
    public class ZuoraCreateSubscriptionCommand : ZuoraCommand, ICommand<ZuoraAccount, bool>
    {
        public bool Execute(ZuoraAccount request, IClient client)
        {
            throw new NotImplementedException();
        }

        public bool Rollback(ZuoraAccount request, IClient client)
        {
            throw new NotImplementedException();
        }
    }
}
