using Oncenter.BackOffice.Entities.Interfaces;
using Oncenter.BackOffice.Zuora.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oncenter.BackOffice.Zuora.Account
{
    public class ZuoraGetAccountCommand : ZuoraCommand, ICommand<string, ZuoraAccount>
    {
        public ZuoraAccount Execute(string accountKey, IClient client)
        {
            string responseString = client.ProcessRequest("/v1/accounts/"+ accountKey + "/summary", HttpMethod.POST, _version);
          
        }

        public bool Rollback(string request, IClient client)
        {
            return true;
        }
    }
}
