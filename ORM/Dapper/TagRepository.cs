using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Extensions.Logging;
using ORM.Domain;
using ORM.LogProvider;

namespace ORM.Dapper
{
    public class TagRepository : DpRepository<Tag>, ITagRepository
    {
        private ILogger<DBLogger> _logger;
        private string RunGroupName = Guid.NewGuid().ToString();

        public TagRepository(ILogger<DBLogger> logger) : base(logger)
        {
            _logger = logger;
        }

        public bool Insert(IEnumerable<Tag> entities)
        {
            const string sql = "INSERT INTO [dbo].[Tag]([Name]) VALUES(@Name)";
            _logger.Debug(
                methodName: "Insert",
                message: $"sql query when Inserting Author entities{sql}",
                runGroup: RunGroupName,
                keyValue: "125-345435-457-045",
                programName: "Logger",
                additionalData: new[] { sql }
            );


            return base.Insert(entities, sql);
        }

        public bool Update(Tag entity)
        {
            const string sql = "UPDATE [dbo].[Tag] SET [Name] = @Name, WHERE Id=@Id";
            return base.Update(entity, sql);
        }
    }
}