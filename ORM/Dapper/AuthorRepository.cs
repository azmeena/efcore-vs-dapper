using System;
using ORM.Domain;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using ORM.LogProvider;

namespace ORM.Dapper
{
    public class AuthorRepository : DpRepository<Author>, IAuthorRepository
    {
        private ILogger<DBLogger> _logger;
        private string RunGroupName = Guid.NewGuid().ToString();

        public AuthorRepository(ILogger<DBLogger> logger) : base(logger)
        {
            _logger = logger;
        }

        public bool Insert(IEnumerable<Author> entities)
        {
            const string sql = "INSERT INTO [dbo].[Author]([Name]) VALUES(@Name)";

            _logger.Debug(
                methodName: "Insert",
                message: $"sql query when Inserting Author entities {sql}",
                runGroup: RunGroupName,
                keyValue: "125-345435-457-045",
                programName: "Logger",
                additionalData: new []{ sql }
            );

            return base.Insert(entities, sql);
        }

        public bool Update(Author entity)
        {
            const string sql = "UPDATE [dbo].[Author] SET [Name] = @Name WHERE Id=@Id";

            _logger.Debug(
                methodName: "Update",
                message: "sql query to update Author entity",
                runGroup: RunGroupName,
                keyValue: "125-345435-457-045",
                programName: "Logger",
                additionalData: new[] { sql }
            );


            return base.Update(entity, sql);
        }
    }
}