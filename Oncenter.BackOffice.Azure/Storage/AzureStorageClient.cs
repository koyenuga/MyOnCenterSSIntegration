using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oncenter.BackOffice.Entities.Interfaces;
namespace Oncenter.BackOffice.Azure.Storage
{
    public class AzureStorageClient : IClient
    {
        public string UserName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Password { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Endpoint { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public string ProcessRequest(string action, HttpMethod methodType, object parameter, string version)
        {
            throw new NotImplementedException();
        }

        public string ProcessRequest(string action, HttpMethod methodType, string version)
        {
            throw new NotImplementedException();
        }
    }
}
