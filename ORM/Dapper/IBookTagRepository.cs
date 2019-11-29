using System.Collections.Generic;
using ORM.Domain;

namespace ORM.Dapper
{
    public interface IBookTagRepository: IDpRepository<BookTag>
    {
        bool Insert(IEnumerable<BookTag> entities);
        bool Update(BookTag entity);
    }
}