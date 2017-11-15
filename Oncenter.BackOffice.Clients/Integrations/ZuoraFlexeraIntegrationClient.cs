﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Oncenter.BackOffice.Common;
using Oncenter.BackOffice.Entities;
using Oncenter.BackOffice.Entities.Zuora;
using Oncenter.BackOffice.Clients.Interfaces;
using Oncenter.BackOffice.Clients.Zuora;
using Oncenter.BackOffice.Entities.Interfaces;
using Oncenter.BackOffice.Entities.Integrations;

namespace Oncenter.BackOffice.Clients.Integrations
{
    public class ZuoraFlexeraIntegrationClient : IBackOfficeProductDeliveryClient
    {
        public ZuoraClient zuora = new ZuoraClient("", "");
        public T ProcessDownGrade<T>(IBackOfficeProductDeliveryWorkItem workItem)
        {
            throw new NotImplementedException();
        }

        public T ProcessLicenceModelChange<T>(IBackOfficeProductDeliveryWorkItem workItem)
        {
            throw new NotImplementedException();
        }

        public T ProcessMidTermAddOn<T>(IBackOfficeProductDeliveryWorkItem workItem)
        {
            throw new NotImplementedException();
        }

        public T ProcessNewOrder<T>(IBackOfficeProductDeliveryWorkItem workItem)
        {
            var subscription = zuora.GetSubscriptionDetails(workItem.EntityId);
            
            return default(T);
        }

        public T ProcessReInstation<T>(IBackOfficeProductDeliveryWorkItem workItem)
        {
            throw new NotImplementedException();
        }

        public T ProcessRenewal<T>(IBackOfficeProductDeliveryWorkItem workItem)
        {
            throw new NotImplementedException();
        }

        public T ProcessTransfer<T>(IBackOfficeProductDeliveryWorkItem workItem)
        {
            throw new NotImplementedException();
        }

        public T ProcessUpgrade<T>(IBackOfficeProductDeliveryWorkItem workItem)
        {
            throw new NotImplementedException();
        }
    }
}