using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oncenter.BackOffice.Zuora.Entities;
using Oncenter.BackOffice.Entities.Interfaces;
using Newtonsoft.Json;
using System.Dynamic;

namespace Oncenter.BackOffice.Zuora.Account
{
    public class ZuoraCreateAccountCommand : ZuoraCommand, ICommand<ZuoraAccount, ZuoraAccount>
    {
        

        public ZuoraAccount Execute(ZuoraAccount newAccount, IClient client)
        {
            dynamic zuoraAccount = new ExpandoObject();

            zuoraAccount.Name = newAccount.CompanyName;
            zuoraAccount.AccountNumber = newAccount.AccountNumber;
            zuoraAccount.AutoPay = false;
            zuoraAccount.Batch = newAccount.Batch;
            zuoraAccount.BillCycleDay = newAccount.BillCycleDay;
            zuoraAccount.Currency = newAccount.Currency;
            zuoraAccount.PaymentTerm = newAccount.PaymentTerm;
            zuoraAccount.Status = "Draft";
            zuoraAccount.InvoiceDeliveryPrefsEmail = newAccount.InvoiceDeliveryByEmail;
            zuoraAccount.InvoiceDeliveryPrefsPrint = newAccount.InvoiceDeliveryByPrint;
            zuoraAccount.IntegrationId__NS = newAccount.NetsuiteIntegrationId;

            if (newAccount.IsTaxExempt)
            {
                zuoraAccount.TaxExemptStatus = "Yes";
                zuoraAccount.TaxExemptCertificateID = "0000000000";
            }
            var jsonParameter = JsonConvert.SerializeObject(zuoraAccount);
      
            string responseString = client.ProcessRequest("/v1/object/account", HttpMethod.POST, jsonParameter, _version);

            dynamic resp = JsonConvert.DeserializeObject(responseString);

            if (resp.Success == true)
                newAccount.AccountId = resp.id;
            else
                LogError(resp.Errors);

            return newAccount;
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
