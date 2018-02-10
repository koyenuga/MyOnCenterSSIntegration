using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oncenter.BackOffice.Entities.Interfaces
{
    public interface ILogger
    {
        void Log(LogMessageType messageType, string message);
        void Log(LogMessageType messageType, string message, string messageCode);
    }

    public enum LogMessageType
    {
        Information,
        Warning,
        Error
    }
}
