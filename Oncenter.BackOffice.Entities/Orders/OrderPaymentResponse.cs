using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oncenter.BackOffice.Entities.Orders
{
    public class OrderPaymentResponse
    {
        
        public string PaymentId { get; set; }
        public bool Sucessful { get; set; }
        public List<string> Errors { get; set; }
    }
}
