using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalCluster.CommunicationServer.Database
{
    public interface IRepository<T> where T : class
    {
        DbContext DbContext { get; }
        IQueryable<T> GetAll();
        T GetById(int id);
        void Add(T entiity);
        void Update(T entity);
        void Delete(T entity);
        void Delete(int id);
    }
}
