using Oncenter.BackOffice.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oncenter.BackOffice.Zuora
{
    public class ZuoraCommand
    {
        protected string _version = "211.0";
        protected ILogger _logger;

        public ZuoraCommand() { }
        public ZuoraCommand(string version)
        {
            _version = version;
        }

        public ZuoraCommand(string version, ILogger logger)
        {
            _version = version;
        }

        protected void LogError(dynamic errors)
        {
            foreach (var error in errors)
                _logger.Log(LogMessageType.Error, error.Message, error.Code);
        }
    }
}
