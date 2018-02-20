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
    public class AzureGetAccountFromTableStorageCommand<T> : AzureTableStorageCommand, ICommand<Tuple<string, string>, AzureStorageEntity<T>>
    {

        public AzureGetAccountFromTableStorageCommand(string containerName):base(containerName)
        {

        }
        public AzureStorageEntity<T> Execute(Tuple<string, string> accountInfo, IClient client =null)
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<AzureStorageEntity<T>>(accountInfo.Item1, accountInfo.Item2);
            TableResult retrievedResult = table.Execute(retrieveOperation);
            if (retrievedResult.Result != null)
            {
                return (AzureStorageEntity<T>)retrievedResult.Result;
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
