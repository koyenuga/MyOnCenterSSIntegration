using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oncenter.BackOffice.Clients.Zuora;
using Oncenter.BackOffice.Clients.Flexera;
using Oncenter.BackOffice.Entities;
using Oncenter.BackOffice.Entities.Zuora;

namespace Oncenter.BackOffice.Clients.Integrations
{
    public class OrderProcess
    {
        FlexeraClient flexeraClient = new FlexeraClient();
        ZuoraClient zuoraClient = new ZuoraClient("kamar.oyenuga@oncenter.com", "@ncent3r@!", true);
        public Account ProcessNewOrder( Account account)
        {
            //Get SubscriptionFrom Zuora
            if (string.IsNullOrWhiteSpace(account.Id))
                zuoraClient.CreateAccount(account);

            foreach (var subscription in account.Subscriptions)
            {
                if ((string.IsNullOrWhiteSpace(subscription.SubscriptionId))
                    && string.IsNullOrWhiteSpace(subscription.SubscriptionNumber))
                {
                    zuoraClient.CreateSubscription(account.ZuoraAccountId, subscription);
                }

                var zSubscription = zuoraClient.GetSubscriptionDetails(subscription.SubscriptionId);
                var fEntitlements = flexeraClient.GetEntitlements(account.FlexeraOrganizationId);
                
            }
                
            //Create Org in Flexera
            //Create Entitlement by license model
            // write Entitlement data to xml
            //Save xml as attachment to subscription in Zuora

            return null;
        }


    }
}
