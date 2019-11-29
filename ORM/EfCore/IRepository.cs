using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ORM.Domain;

namespace ORM.EfCore
{
    public interface IEfRepository<TEntity> where TEntity : BaseEntity
    {
        #region Methods

        TEntity GetById(object id);
        TEntity GetById(int id);
        void Insert(TEntity entity);
        void Insert(IEnumerable<TEntity> entities);
        void Update(TEntity entity);
        void Update(IEnumerable<TEntity> entities);
        void Delete(TEntity entity);
        void Delete(IEnumerable<TEntity> entities);
        #endregion

        #region Properties
        IQueryable<TEntity> Table { get; }
        IQueryable<TEntity> AsNoTracking { get; }
        #endregion

        TEntity SpGetById(int id);
        IEnumerable<TEntity> SpGetAll();
        IEfRepository<TEntity> Including<TProp>(Expression<Func<TEntity, TProp>> include) where TProp : class;
        IEnumerable<TEntity> SpQuery(string sql);
    }
}