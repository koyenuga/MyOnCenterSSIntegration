using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oncenter.BackOffice.Clients.Flexera.Entitlement;

namespace Oncenter.BackOffice.Clients.Flexera
{
    public class FlexeraClient
    {
        public dynamic CreateEntitlement(string orgId, )
        {
            var createEntitlementRequestype = new createSimpleEntitlementDataType();
            createEntitlementRequestype.lineItems[0].partNumber.
            //var c = new Oncenter.BackOffice.Clients.Flexera.Entitlement.EntitlementOrderService().createSimpleEntitlement()
                //https://flex1245-uat.flexnetoperations.com/flexnet/services/EntitlementOrderService?wsdl

        }
    }
}
