using Microsoft.EntityFrameworkCore;
using ORM.Domain;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

namespace ORM.EfCore
{
    public class SqlContext : DbContext
    {
        public new DbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity
        {
            return base.Set<TEntity>();
        }

        public SqlContext(DbContextOptions<SqlContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Author>();
            modelBuilder.Entity<Book>();
            modelBuilder.Entity<Tag>();
            modelBuilder.Entity<Library>();

            modelBuilder.Entity<BookTag>()
                .Ignore(x=>x.Id)
                .HasIndex(x => new {x.BookId, x.TagId});

            modelBuilder.Entity<BookTag>()
                .HasKey(x => new {x.BookId, x.TagId});     
        }

        public IQueryable<TEntity> EntityFromSql<TEntity>(string sql, params object[] parameters) where TEntity : BaseEntity
        {
            return Set<TEntity>().FromSql(CreateSqlWithParameters(sql, parameters), parameters);
        }

        public IQueryable<TEntity> SpQuery<TEntity>(string sql) where TEntity : BaseEntity => Set<TEntity>().FromSql(sql);

        public int ExecuteSqlCommand(RawSqlString sql, bool doNotEnsureTransaction = false, int? timeout = null, params object[] parameters)
        {
            var previousTimeout = Database.GetCommandTimeout();
            Database.SetCommandTimeout(timeout);

            var result = 0;
            if (!doNotEnsureTransaction)
            {
                using (var transaction = Database.BeginTransaction())
                {
                    result = Database.ExecuteSqlCommand(sql, parameters);
                    transaction.Commit();
                }
            }
            else
                result = Database.ExecuteSqlCommand(sql, parameters);

            Database.SetCommandTimeout(previousTimeout);

            return result;
        }

        protected string CreateSqlWithParameters(string sql, params object[] parameters)
        {
            for (var i = 0; i <= (parameters?.Length ?? 0) - 1; i++)
            {
                if (!(parameters[i] is DbParameter parameter))
                    continue;

                sql = $"{sql}{(i > 0 ? "," : string.Empty)} @{parameter.ParameterName}";

                //whether parameter is output
                if (parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Output)
                    sql = $"{sql} output";
            }

            return sql;
        }

        public static DbParameter GetParameter(DbType dbType, string parameterName, object parameterValue)
        {
            return new SqlParameter
            {
                ParameterName = parameterName,
                Value = parameterValue,
                DbType = dbType
            };
        }
    }
}
