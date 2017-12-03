using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Oncenter.BackOffice.Entities.Interfaces
{
    public interface IStorage
    {
        void Save<T>(T data);
        T Get<T>(string id);
        List<T> Get<T>();
        List<T> Get<T, TSelector>(Expression<Func<TSelector>> selector);
    }
}
