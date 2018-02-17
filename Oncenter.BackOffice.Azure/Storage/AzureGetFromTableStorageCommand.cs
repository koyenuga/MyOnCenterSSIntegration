using Microsoft.WindowsAzure.Storage.Table;
using Oncenter.BackOffice.Entities;
using Oncenter.BackOffice.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oncenter.BackOffice.Azure.Storage
{
    public class AzureGetFromTableStorageCommand<T> : AzureTableStorageCommand, ICommand<string, List<AzureStorageEntity<T>>>
    {

        public AzureGetFromTableStorageCommand(string containerName):base(containerName)
        {

        }
        public List<AzureStorageEntity<T>> Execute(string accountNumber, string subscriptionNumber)
        {
            TableQuery<AzureStorageEntity<T>> query = new TableQuery<AzureStorageEntity<T>>().Where(

                TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, accountNumber),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, subscriptionNumber)));

            return table.ExecuteQuery<AzureStorageEntity<T>>(query).ToList();
        }

       

        public bool Rollback(string request, IClient client)
        {
            throw new NotImplementedException();
        }

        List<AzureStorageEntity<T>> ICommand<string, List<AzureStorageEntity<T>>>.Execute(string request, IClient client)
        {
            throw new NotImplementedException();
        }

        bool ICommand<string, List<AzureStorageEntity<T>>>.Rollback(string request, IClient client)
        {
            throw new NotImplementedException();
        }
    }
}
