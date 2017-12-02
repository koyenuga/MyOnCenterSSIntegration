using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oncenter.BackOffice.Entities;
using Oncenter.BackOffice.Entities.Orders;
namespace OnCenter.BackOffice.Repository.Interfaces
{
    public interface IRepository<T>
    {
        void Create(T data);
        void Create(T data, string id);
        List<T> Get();
        List<T> Get<T2>(T2 data);
        T Get(object id);

        T Update(T data);

        void Delete(object id);

    }
}
