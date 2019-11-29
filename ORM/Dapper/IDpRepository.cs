using System;
using System.Collections.Generic;
using ORM.Domain;

namespace ORM.Dapper
{
    public interface IDpRepository<TEntity> : IDisposable where TEntity : BaseEntity
    {
        bool Delete(int id);
        List<TEntity> GetAll();
        TEntity GetById(int id);
        bool Insert(IEnumerable<TEntity> entities, string sql);
        bool Insert(TEntity entity, string sql);
        bool Update(IEnumerable<TEntity> entities, string sql);
        bool Update(TEntity entity, string sql);

        List<TEntity> SpGetAll();
        TEntity SpGetById(int id);
        int SpDelete(int id);
    }
}
