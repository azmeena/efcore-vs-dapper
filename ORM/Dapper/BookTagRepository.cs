using Microsoft.Extensions.Logging;
using ORM.Domain;
using ORM.LogProvider;
using System;
using System.Collections.Generic;

namespace ORM.Dapper
{
    public class BookTagRepository : DpRepository<BookTag>, IBookTagRepository
    {
        private ILogger<DBLogger> _logger;
        private string RunGroupName = Guid.NewGuid().ToString();

        public BookTagRepository(ILogger<DBLogger> logger) : base(logger)
        {
            _logger = logger;
        }

        public bool Insert(IEnumerable<BookTag> entities)
        {
            const string sql = "INSERT INTO [dbo].[BookTag]([BookId],[TagId]) VALUES(@BookId, @TagId)";

            _logger.Debug(
                methodName: "Insert",
                message: $"sql query when Inserting IEnumerable of Author entities {sql}",
                runGroup: RunGroupName,
                keyValue: "125-345435-457-045",
                programName: "Logger",
                additionalData: new[] { sql }
            );

            return base.Insert(entities, sql);
        }
     
        public bool Update(BookTag entity)
        {
            const string sql = "UPDATE [dbo].[BookTag] SET [BookId] = @BookId,[TagId] = @TagId WHERE Id=@Id";
            return base.Update(entity, sql);
        }
    }
}