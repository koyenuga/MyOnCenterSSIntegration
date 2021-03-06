﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oncenter.BackOffice.Entities.Orders;

namespace Oncenter.BackOffice.Entities.Interfaces
{
    public interface ISubscription
    {
        dynamic Create(FulfillOrderRequest request);
        dynamic Amend(FulfillOrderRequest request, dynamic existingSubscription);

        dynamic Get(string accountNumber);
        dynamic MakePayment(OrderPaymentRequest request);
        dynamic GetProductRatePlanCharge(string id);

    }
}
