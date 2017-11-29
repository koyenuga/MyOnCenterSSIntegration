using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Oncenter.BackOffice.Entities;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Oncenter.BackOffice.Clients.Azure
{
    public class AzureStorageTableClient
    {
        CloudStorageAccount storageAccount;
        CloudTableClient tableClient;
        
        public AzureStorageTableClient()
        {
           storageAccount = CloudStorageAccount.Parse(
               ConfigurationManager.ConnectionStrings["AzureStorage"].ConnectionString);
            tableClient = storageAccount.CreateCloudTableClient();
            
        }

        public void Save<T>(AzureStorageEntity<T> data, string containerName)
        {
            CloudTable table = tableClient.GetTableReference(containerName);
            TableOperation insertOperation = TableOperation.InsertOrReplace(data);
            var result = table.Execute(insertOperation);
            
        }

        public void CreateTableIfNotExist(string containerName)
        {
            CloudTable table = tableClient.GetTableReference(containerName);
            table.CreateIfNotExists();
        }
    }
}
