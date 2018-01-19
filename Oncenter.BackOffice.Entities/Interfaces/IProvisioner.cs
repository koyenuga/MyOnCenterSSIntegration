using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oncenter.BackOffice.Entities.Orders;
using Oncenter.BackOffice.Entities.License;

namespace Oncenter.BackOffice.Entities.Interfaces
{
    public interface IProvisioner
    {
       List<EntitlementResponse> Provision(FulfillOrderRequest request);
        OCSLicense ProvisionTrialLicense(string trialDays, string partNumber, string servicePartNumber);

        string GetEntitlement(string id);
    }
}
