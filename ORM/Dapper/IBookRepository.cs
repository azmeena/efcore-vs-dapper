using System.Collections.Generic;
using ORM.Domain;

namespace ORM.Dapper
{
    public interface IBookRepository :IDpRepository<Book>
    {
        bool Insert(Book entity);
        bool Insert(IEnumerable<Book> entities);
        bool Update(Book entity);
        List<Book> WithAuthor();
        List<Book> WithTags();
    }
}