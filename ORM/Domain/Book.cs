using System;
using System.Collections.Generic;

namespace ORM.Domain
{
    public class Book : BaseEntity
    {
        public string Name { get; set; }

        public DateTime PublishDate { get; set; }

        public List<BookTag> BookTags { get; set; }

        public Author Author { get; set; }

        public int  AuthorId { get; set; }
    }
}
