using Dapper;
using ORM.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.Extensions.Logging;
using ORM.LogProvider;

namespace ORM.Dapper
{ 
    public class DpRepository<TEntity> : IDpRepository<TEntity> where TEntity : BaseEntity
    {

        protected readonly IDbConnection DbConn;

        private ILogger<DBLogger> _logger;

        private string RunGroupName = Guid.NewGuid().ToString();

        public DpRepository(ILogger<DBLogger> logger)
        {
            DbConn = new SqlConnection("Data Source=localhost\\SQLEXPRESS02;Initial Catalog=ELearn;Integrated Security=True;");
            _logger = logger;
        }

        #region SqlCommand
        public List<TEntity> GetAll()
        {
            var sql = $"Select * from {typeof(TEntity).Name}";

            _logger.Debug(
                methodName: "GetAll",
                message: $"sql query when retrieving all entities{sql}",
                runGroup: RunGroupName,
                keyValue: "125-345435-457-045",
                programName: "Logger",
                additionalData: new []{ sql}
            );

            return DbConn.Query<TEntity>(sql).ToList();
        }

        public TEntity GetById(int id)
        {
            var sql = $"Select * from {typeof(TEntity).Name} WHERE Id=@Id";

            _logger.Debug(
                methodName: "GetById",
                message: $"sql query when retrieving entities by Id {sql}",
                runGroup: RunGroupName,
                keyValue: "125-345435-457-045",
                programName: "Logger",
                additionalData: new[] { sql }
            );

            return DbConn.QueryFirstOrDefault<TEntity>(sql, new { Id = id });
        }

        public virtual bool Insert(TEntity entity, string sql)
        {
            try
            {
                var count = DbConn.Execute(sql, entity);
                return count > 0;
            }
            catch (Exception){return false;}
        }

        public virtual bool Update(TEntity entity, string sql)
        {
            return DbConn.Execute(sql, entity) > 0;
        }

        public virtual bool Insert(IEnumerable<TEntity> entities, string sql)
        {
            var affectedRows = 0;
            foreach (var entity in entities)
            {
                affectedRows = DbConn.Execute(sql, entity);
            }
            return affectedRows > 0;
        }

        public virtual bool Update(IEnumerable<TEntity> entities, string sql)
        {
            foreach (var entity in entities)
            {
                Update(entity, sql);
            }
            return true;
        }

        public bool Delete(int id)
        {
            var sql = $"DELETE FROM {typeof(TEntity).Name} WHERE ID = @Id";
            return DbConn.Execute(sql, new { Id = id }) > 0;
        }
        #endregion

        #region StoredProcedure 
        protected IEnumerable<TEntity> SpQuery(string name, object parms = null) =>
            DbConn.Query<TEntity>(name, parms, commandType: CommandType.StoredProcedure);

        protected int SpExecute(string name, object parms = null) =>
            DbConn.Execute(name, parms, commandType: CommandType.StoredProcedure);

        public List<TEntity> SpGetAll() =>
            SpQuery($"sp_get_all_{typeof(TEntity).Name}").ToList();

        public TEntity SpGetById(int id) =>
            SpQuery($"sp_get_{typeof(TEntity).Name}", new { Id = id }).FirstOrDefault();

        public int SpDelete(int id) =>
            SpExecute($"sp_delete_{typeof(TEntity).Name}", new { Id = id });

        #endregion

        public void Dispose()
        {
            DbConn.Dispose();
        }
    }
}