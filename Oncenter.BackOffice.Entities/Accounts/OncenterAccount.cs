
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oncenter.BackOffice.Entities.Interfaces;
namespace Oncenter.BackOffice.Entities.Accounts
{
    public class OncenterAccount : OncenterAccountRequest, IAccountResponse
    {
        public string FlexeraOrganizationId { get; set; }
    }
}
