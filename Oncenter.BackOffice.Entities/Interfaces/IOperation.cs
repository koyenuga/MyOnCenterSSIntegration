using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oncenter.BackOffice.Entities.Interfaces
{
    public interface IOperation
    {
        string UserName { get; set; }
        string Password { get; set; }
        string EndPointUrl { get; set; }
        string Execute(Method requestType, object Parameter = null, string action="", string version = "");
        
    }
}
