using Relevamiento.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relevamiento.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class, new()
    {
        private readonly SQLiteConnection db;

        public GenericRepository()
        {
            db = new SQLiteConnection(App.RutaBD);
        }

        public bool Insert(T obj)
        {           
            var rs = db.Insert(obj) > 0 ? true : false;
            return rs;
        }

        public bool Delete(T obj)
        {
            var rs = db.Delete(obj) > 0 ? true : false;
            return rs;
        }

        public T GetById(int id)
        {
            var rs = db.Get<T>(id);
            return rs;
        }

        public IEnumerable<T> GetAll()
        {
            var rs = db.Table<T>();
            return rs;
        }

        public bool Update(T obj)
        {
            var rs = db.Update(obj) > 0 ? true : false;
            return rs;
        }
    }
}
