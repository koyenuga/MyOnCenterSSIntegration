using Newtonsoft.Json;
using Oncenter.BackOffice.Entities.Interfaces;
using Oncenter.BackOffice.Zuora.Entities;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oncenter.BackOffice.Zuora.Account
{
    public class ZuoraUpdateAccountCommand : ZuoraCommand, ICommand<ZuoraAccount, ZuoraAccount>
    {

        public ZuoraAccount Execute(ZuoraAccount account, IClient client)
        {
            dynamic zuoraAccount = new ExpandoObject();

            zuoraAccount.Name = account.CompanyName;
            zuoraAccount.AccountNumber = account.AccountNumber;
            zuoraAccount.AutoPay = false;
            zuoraAccount.Batch = account.Batch;
            zuoraAccount.BillCycleDay = account.BillCycleDay;
            zuoraAccount.Currency = account.Currency;
            zuoraAccount.PaymentTerm = account.PaymentTerm;
            zuoraAccount.InvoiceDeliveryPrefsEmail = account.InvoiceDeliveryByEmail;
            zuoraAccount.InvoiceDeliveryPrefsPrint = account.InvoiceDeliveryByPrint;
            zuoraAccount.IntegrationId__NS = account.NetsuiteIntegrationId;
            zuoraAccount.BillToId = account.BillToId;
            zuoraAccount.SoldToId = account.SoldToId;
            if (account.IsTaxExempt)
            {
                zuoraAccount.TaxExemptStatus = "Yes";
                zuoraAccount.TaxExemptCertificateID = "0000000000";
            }
            var jsonParameter = JsonConvert.SerializeObject(zuoraAccount);

            string responseString = client.ProcessRequest("/v1/object/account/" + account.AccountId, HttpMethod.POST, jsonParameter, _version);

            dynamic resp = JsonConvert.DeserializeObject(responseString);

            if (resp.Success == true)
                account.AccountId = resp.id;
            else
                LogError(resp.Errors);

            return account;
        }



        public bool Rollback(ZuoraAccount account, IClient client)
        {
            string responseString = client.ProcessRequest("/v1/object/account/" + account.AccountId, HttpMethod.DELETE, _version);
            dynamic resp = JsonConvert.DeserializeObject(responseString);

            if (resp.Success == true)
                return true;
            else
                return false;
        }
    }
}
