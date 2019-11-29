using System.Collections.Generic;
using ORM.Domain;

namespace ORM.Dapper
{
    public interface IAuthorRepository : IDpRepository<Author>
    {
        bool Insert(IEnumerable<Author> entities);
        bool Update(Author entity);
    }
}