using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oncenter.BackOffice.Entities.Interfaces;
using Oncenter.BackOffice.Zuora.Entities;
using System.Dynamic;
using Newtonsoft.Json;

namespace Oncenter.BackOffice.Zuora.Account
{
    public class ZuoraActivateAccountCommand : ZuoraCommand, ICommand<ZuoraAccount, bool>
    {
        dynamic zuoraAccount = new ExpandoObject();
        public bool Execute(ZuoraAccount account, IClient client)
        {
            zuoraAccount.BillToId = account.BillToId;

            if (!string.IsNullOrWhiteSpace(account.SoldToId))
                zuoraAccount.SoldToId = account.SoldToId;
            else
                zuoraAccount.SoldToId = account.BillToId;

            zuoraAccount.CommunicationProfileId = account.CommunicationProfileId;
            zuoraAccount.Status = "Active";
            var jsonParameter = JsonConvert.SerializeObject(zuoraAccount);
            string responseString = client.ProcessRequest("/v1/object/account/" + account.AccountId, HttpMethod.POST, jsonParameter, _version);
            dynamic resp = JsonConvert.DeserializeObject(responseString);
            if (resp.Success == true)
                return true;
            else
                return false;
        }

        public bool Rollback(ZuoraAccount account, IClient client)
        {
            zuoraAccount.BillToId = string.Empty;
            zuoraAccount.SoldToId = string.Empty;
            zuoraAccount.Status = "Draft";
            zuoraAccount.CommunicationProfileId = string.Empty;

            var jsonParameter = JsonConvert.SerializeObject(zuoraAccount);
            string responseString = client.ProcessRequest("/v1/object/account/" + account.AccountId, HttpMethod.POST, jsonParameter, _version);
            dynamic resp = JsonConvert.DeserializeObject(responseString);

            if (resp.Success == true)
                return true;
            else
                return false;
        }
    }
}
