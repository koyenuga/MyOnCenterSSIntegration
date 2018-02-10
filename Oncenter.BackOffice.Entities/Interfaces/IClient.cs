using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace Oncenter.BackOffice.Entities.Interfaces
{
    public interface IClient
    {
        string UserName { get; set; }
        string Password { get; set; }
        string Endpoint { get; set; }
        string ProcessRequest(string action, HttpMethod methodType, object parameter, string version);
        string ProcessRequest(string action, HttpMethod methodType, string version);
    }

    public enum HttpMethod
    {
        GET,
        POST,
        PUT,
        DELETE
    }
}
