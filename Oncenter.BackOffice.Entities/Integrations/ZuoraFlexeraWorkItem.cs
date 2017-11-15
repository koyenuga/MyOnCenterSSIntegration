using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oncenter.BackOffice.Common;
using Oncenter.BackOffice.Entities.Interfaces;

namespace Oncenter.BackOffice.Entities.Integrations
{
    public class ZuoraFlexeraWorkItem : IBackOfficeProductDeliveryWorkItem
    {
        public ProductDeliveryActivity Activity { get; set; }
        public string QueueName { get; set; }
        public string EntityName { get; set; }
        public string EntityId { get; set; }

        public ZuoraFlexeraWorkItem(ProductDeliveryActivity activity, string queueName, string entityName, string entityId)
        {
            Activity = activity;
            QueueName = queueName;
            EntityName = entityName;
            EntityId = entityId;
        }
    }
}
