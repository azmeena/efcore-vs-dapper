using Microsoft.EntityFrameworkCore;
using ORM.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace ORM.EfCore
{
    public class EfRepository<TEntity> : IEfRepository<TEntity> where TEntity : BaseEntity
    {
        #region Fields

        private readonly SqlContext _context;
        private DbSet<TEntity> _entities;

        #endregion
        #region Ctor

        public EfRepository(SqlContext context)
        {
            this._context = context;
        }

        #endregion
        #region Utilities

        protected string GetFullErrorTextAndRollbackEntityChanges(DbUpdateException exception)
        {
            //rollback entity changes
            if (_context is DbContext dbContext)
            {
                var entries = dbContext.ChangeTracker.Entries()
                    .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified).ToList();

                entries.ForEach(entry =>
                {
                    try
                    {
                        entry.State = EntityState.Unchanged;
                    }
                    catch (InvalidOperationException)
                    {
                        // ignored
                    }
                });
            }

            _context.SaveChanges();
            return exception.ToString();
        }

        #endregion

        #region Methods

        public virtual TEntity GetById(object id)
        {
            return Entities.Find(id);
        }

        public virtual TEntity GetById(int id)
        {   
            return Entities.Find(id);
        }

        public virtual void Insert(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            try
            {
                Entities.Add(entity);
                _context.SaveChanges();
            }
            catch (DbUpdateException exception)
            {
                throw new Exception(GetFullErrorTextAndRollbackEntityChanges(exception), exception);
            }
        }

        public virtual void Insert(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            try
            {
                Entities.AddRange(entities);

                _context.SaveChanges();
            }
            catch (DbUpdateException exception)
            {
                throw new Exception(GetFullErrorTextAndRollbackEntityChanges(exception), exception);
            }
        }

        public virtual void Update(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            try
            {
                Entities.Update(entity);
                _context.SaveChanges();
            }
            catch (DbUpdateException exception)
            {
                throw new Exception(GetFullErrorTextAndRollbackEntityChanges(exception), exception);
            }
        }

        public virtual void Update(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            try
            {
                Entities.UpdateRange(entities);
                _context.SaveChanges();
            }
            catch (DbUpdateException exception)
            {
                throw new Exception(GetFullErrorTextAndRollbackEntityChanges(exception), exception);
            }
        }

        public virtual void Delete(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            try
            {
                Entities.Remove(entity);
                _context.SaveChanges();
            }
            catch (DbUpdateException exception)
            {
                throw new Exception(GetFullErrorTextAndRollbackEntityChanges(exception), exception);
            }
        }

        public virtual void Delete(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            try
            {
                Entities.RemoveRange(entities);
                _context.SaveChanges();
            }
            catch (DbUpdateException exception)
            {
                throw new Exception(GetFullErrorTextAndRollbackEntityChanges(exception), exception);
            }
        }

        #endregion

        #region StoredProcedure
        public virtual TEntity SpGetById(int id)
        {
            return _context.EntityFromSql<TEntity>($"sp_get_{typeof(TEntity).Name}", SqlContext.GetParameter(DbType.Int32, nameof(BaseEntity.Id), id))
                           .FirstOrDefault();
        }

        public virtual IEnumerable<TEntity> SpGetAll()
        {
            return _context.EntityFromSql<TEntity>($"sp_get_all_{typeof(TEntity).Name}");
        }

        public virtual IEfRepository<TEntity> Including<TProp>(Expression<Func<TEntity, TProp>> include) where TProp : class
        {
            var entities = _context.EntityFromSql<TEntity>($"sp_get_all_{typeof(TEntity).Name}");
            entities.Select(include).Load();
            return this;
        }
 
        public virtual IEnumerable<TEntity> SpQuery(string sql)
        {
            return _context.SpQuery<TEntity>(sql);
        }
        #endregion

        #region Properties

        public virtual IQueryable<TEntity> Table => Entities;

        public virtual IQueryable<TEntity> AsNoTracking => Entities.AsNoTracking();

        protected virtual DbSet<TEntity> Entities
        {
            get
            {
                if (_entities == null)
                    _entities = _context.Set<TEntity>();

                return _entities;
            }
        }

        #endregion
    }
}