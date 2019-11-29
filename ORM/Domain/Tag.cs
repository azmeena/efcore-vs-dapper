using System.Collections.Generic;

namespace ORM.Domain
{
    public class Tag : BaseEntity
    {
        public string Name { get; set; }
        public List<BookTag> BookTags { get; set; }
    }
}
