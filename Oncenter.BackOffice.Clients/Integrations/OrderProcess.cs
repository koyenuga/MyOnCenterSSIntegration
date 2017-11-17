using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Oncenter.BackOffice.Entities;

namespace Oncenter.BackOffice.Clients.Integrations
{
    public class OrderProcess
    {
        public NewOrderResponseType ProcessNewOrder( NewOrderRequestType request)
        {
            //Get SubscriptionFrom Zuora
            //Create Org in Flexera
            //Create Entitlement by license model
            // write Entitlement data to xml
            //Save xml as attachment to subscription in Zuora

            return null;
        }
    }
}
