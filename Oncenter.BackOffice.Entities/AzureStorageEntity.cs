using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace Oncenter.BackOffice.Entities
{
    public class AzureStorageEntity<T> : TableEntity
    {
        public AzureStorageEntity(string rowKey, string partitionKey, T data)
        {
            this.PartitionKey = partitionKey;
            this.RowKey = rowKey;
            Data = data;
        }

        public T Data { get; }


    }
}
