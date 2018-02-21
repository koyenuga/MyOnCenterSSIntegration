using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace Oncenter.BackOffice.Entities
{
    public class AzureStorageEntity : TableEntity
    {
        public AzureStorageEntity(string rowKey, string partitionKey, string data)
        {
            this.PartitionKey = partitionKey;
            this.RowKey = rowKey;
            Data = data;
        }

        public AzureStorageEntity()
        {
          
        }
        public string Data { get; set; }


    }
}
