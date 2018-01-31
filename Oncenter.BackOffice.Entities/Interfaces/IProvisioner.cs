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
        OCSLicense ProvisionTrialLicense(int trialDays, string partNumber, string servicePartNumber, string companyName, string accountNumber, string productFamily, int qty, bool networked);

        string GetEntitlement(string id);
        string GetLicenseServerAccountNumber(string deviceId);
    }
}
