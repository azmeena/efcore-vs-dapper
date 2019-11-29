using System.Collections.Generic;

namespace ORM.Domain
{
    public class Author: BaseEntity
    {
        public string Name { get; set; }

        public List<Book> Books { get; set; }
    }
}