using Microsoft.WindowsAzure.Storage.Table;
using Oncenter.BackOffice.Entities;
using Oncenter.BackOffice.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oncenter.BackOffice.Common.Extension;

namespace Oncenter.BackOffice.Azure.Storage
{
    public class AzureGetAccountFromTableStorageCommand<T> : AzureTableStorageCommand, ICommand<Tuple<string, string>, T>
    {

        public AzureGetAccountFromTableStorageCommand(string containerName):base(containerName)
        {

        }
        public T Execute(Tuple<string, string> accountInfo, IClient client = null)
        {
            TableQuery<AzureStorageEntity> query = new TableQuery<AzureStorageEntity>().Where(
                TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, accountInfo.Item1),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, accountInfo.Item2)));

            var result = table.ExecuteQuery(query);
            var account = result.FirstOrDefault();
            var obj = (T)Activator.CreateInstance(typeof(T));
            if (account != null)
            {
                return obj.LoadFromJsonString(account.Data);
            }
            else
            {
                throw new Exception("Item not found");
            }

        }


        public bool Rollback(Tuple<string, string> request, IClient client =null)
        {
            throw new NotImplementedException();
        }
    }
}
