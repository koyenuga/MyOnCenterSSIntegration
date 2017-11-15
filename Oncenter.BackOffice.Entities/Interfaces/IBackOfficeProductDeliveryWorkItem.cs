using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oncenter.BackOffice.Common;


namespace Oncenter.BackOffice.Entities.Interfaces
{
    public interface IBackOfficeProductDeliveryWorkItem
    {
        ProductDeliveryActivity Activity { get; set; }
        string QueueName { get; set; }
        string EntityName { get; set; }
        string EntityId { get; set; }
       
    }
}
