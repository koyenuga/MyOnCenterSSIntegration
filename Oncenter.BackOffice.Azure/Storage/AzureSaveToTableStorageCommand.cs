﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oncenter.BackOffice.Entities;
using Oncenter.BackOffice.Entities.Interfaces;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;


namespace Oncenter.BackOffice.Azure.Storage
{
    public class AzureSaveToTableStorageCommand : AzureTableStorageCommand, ICommand<AzureStorageEntity>
    {
        public AzureSaveToTableStorageCommand(string containerName):base(containerName)
        {
          
        }
        
        public void Execute(AzureStorageEntity request)
        {
            TableOperation insertOperation = TableOperation.InsertOrReplace(request);
            var result = table.Execute(insertOperation);
            if(result.HttpStatusCode == 204)
            {

            }
        }

        public bool Rollback(AzureStorageEntity request)
        {
            throw new NotImplementedException();
        }
    }
}
