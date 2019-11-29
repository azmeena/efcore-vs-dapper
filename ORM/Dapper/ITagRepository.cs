using System.Collections.Generic;
using ORM.Domain;

namespace ORM.Dapper
{
    public interface ITagRepository: IDpRepository<Tag>
    {
        bool Insert(IEnumerable<Tag> entities);
        bool Update(Tag entity);
    }
}