using Oncenter.BackOffice.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oncenter.BackOffice.Clients.Interfaces
{
    public interface IBackOfficeProductDeliveryClient
    {
        T ProcessNewOrder<T>(IBackOfficeProductDeliveryWorkItem workItem);
        T ProcessMidTermAddOn<T>(IBackOfficeProductDeliveryWorkItem workItem);
        T ProcessRenewal<T>(IBackOfficeProductDeliveryWorkItem workItem);
        T ProcessTransfer<T>(IBackOfficeProductDeliveryWorkItem workItem);
        T ProcessUpgrade<T>(IBackOfficeProductDeliveryWorkItem workItem);
        T ProcessDownGrade<T>(IBackOfficeProductDeliveryWorkItem workItem);
        T ProcessReInstation<T>(IBackOfficeProductDeliveryWorkItem workItem);
        T ProcessLicenceModelChange<T> (IBackOfficeProductDeliveryWorkItem workItem);

    }
}
