﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oncenter.BackOffice.Entities.Interfaces;

namespace Oncenter.BackOffice.Entities.Accounts
{
    public class OncenterAccountRequest : IAccountRequest
    {
        public string AccountNumber { get; set; }
        public string CompanyName { get; set; }
        public int BillCycleDay { get; set; } = 1;
        public string Currency { get; set; } = "USD";
        public string Batch { get; set; } = "Batch1";
        public string PaymentTerm { get; set; } = "Due Upon Receipt";
        public string Status { get; set; } = "Draft";
        public string DefaultPaymentMethodId { get; set; }
        public string BillToId { get; set; }
    }
}
