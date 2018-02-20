using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oncenter.BackOffice.Entities.Interfaces
{
    public interface ICommand<T>
    {
        void Execute(T request);
        bool Rollback(T request);
    }
    public interface ICommand<in TRequest, out TResult>
    {
        TResult Execute(TRequest request, IClient client);
        bool Rollback(TRequest request, IClient client);
    }

}
