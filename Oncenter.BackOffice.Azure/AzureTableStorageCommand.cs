using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Configuration;

namespace Oncenter.BackOffice.Azure
{
    public class AzureTableStorageCommand
    {
        protected CloudStorageAccount storageAccount;
        protected CloudTableClient tableClient;
        protected string ContainerName;
        protected CloudTable table;
        public AzureTableStorageCommand(string containerName)
        {
            ContainerName = containerName;
            storageAccount = CloudStorageAccount.Parse(
                ConfigurationManager.ConnectionStrings["AzureStorage"].ConnectionString);
            tableClient = storageAccount.CreateCloudTableClient();
            table = tableClient.GetTableReference(ContainerName);
            table.CreateIfNotExists();

        }
    }
}
