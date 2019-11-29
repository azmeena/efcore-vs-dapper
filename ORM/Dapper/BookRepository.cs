using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Microsoft.Extensions.Logging;
using ORM.Domain;
using ORM.LogProvider;

namespace ORM.Dapper
{
    public class BookRepository : DpRepository<Book>, IBookRepository
    {
        private ILogger<DBLogger> _logger;
        private string RunGroupName = Guid.NewGuid().ToString();

        private static readonly Dictionary<int, Book> bookDictionary = new Dictionary<int, Book>();

        private const string sp_book_with_author= "sp_get_book_with_author";
        private const string sp_book_with_tag= "sp_get_book_with_tag";

        public BookRepository(ILogger<DBLogger> logger) : base(logger)
        {
            _logger = logger;
        }
        public List<Book> WithAuthor()
        {

            return DbConn.Query<Book, Author, Book>(
                         sp_book_with_author,
                         MapAuthor,
                         splitOn: nameof(Book.AuthorId),
                         commandType: CommandType.StoredProcedure)
                        .Distinct()
                        .ToList();
        }

        public List<Book> WithTags()
        {
            return DbConn.Query<Book, BookTag, Book>(sp_book_with_tag, MapTags,
                splitOn: nameof(BookTag.BookId)).Distinct().ToList();

        }

        private static Book MapTags(Book book, BookTag bookTag)
        {
            if (!bookDictionary.TryGetValue(book.Id, out var bookEntry))
            {
                bookEntry = book;
                bookEntry.BookTags = new List<BookTag>();
                bookDictionary.Add(bookEntry.Id, bookEntry);
            }
            
            bookEntry.BookTags.Add(bookTag);
            return bookEntry;
        }

        private static Book MapAuthor(Book book, Author author)
        {
            book.Author = author;
          
            return book;
        }

        public bool Insert(Book entity)
        {
            const string sql = "INSERT INTO [dbo].[Book]([Name],[PublishDate],[AuthorId]) VALUES(@Name, @PublishDate,@AuthorId)";

            return base.Insert(entity, sql);
        }

        public bool Insert(IEnumerable<Book> entities)
        {
            const string sql = "INSERT INTO [dbo].[Book]([Name],[PublishDate],[AuthorId]) VALUES(@Name, @PublishDate,@AuthorId)";

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

        public bool Update(Book entity)
        {
            const string sql = "UPDATE [dbo].[Book] SET [Name] = @Name,[PublishDate] = @PublishDate,[AuthorId] = @AuthorId WHERE ID=@ID";
            return base.Update(entity, sql);
        }
    }
}