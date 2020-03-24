using System;
using System.Collections.Generic;
using System.Text;

namespace Relevamiento.Repository
{
    public interface IGenericRepository<T> where T : class
    {
        bool Insert(T obj);
        bool Update(T obj);
        bool Delete(T obj);
        T GetById(int id);
        IEnumerable<T> GetAll();
    }
}
