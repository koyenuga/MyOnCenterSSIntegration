﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oncenter.BackOffice.Entities.Interfaces
{
    public interface IAccountRequest
    {
        string AccountNumber { get; set; }
        string CompanyName { get; set; }
        int BillCycleDay { get; set; }
        string Currency { get; set; }
        string Batch { get; set; }
        string PaymentTerm { get; set; }
        string Status { get; set; }
        bool InvoiceDeliveryByEmail { get; set; }
        bool InvoiceDeliveryByPrint { get; set; }
        bool IsTaxExempt { get; set; }
        string TaxCertificateID { get; set; }
        string CommunicationProfileId { get; set; }
        string NetsuiteIntegrationId { get; set; }
    }
}

