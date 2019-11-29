using GenFu;
using System.Collections.Generic;
using System.Linq;

namespace ORM.Domain
{
    public class Seed
    {
        public List<Author> Authors(int count = 20)
        {
            A.Configure<Author>()
                .Fill(a => a.Id, default(int))
                .Fill(a => a.Name).AsArticleTitle();

            return A.ListOf<Author>(count).ToList();
        }

        public List<Tag> Tags(int count = 20)
        {
            A.Configure<Tag>()
                .Fill(t => t.Id, default(int))
                .Fill(t => t.Name).AsLoremIpsumWords();

            return A.ListOf<Tag>(count).ToList();

        }

        public List<Book> Books(List<Author> authors, int count = 20)
        {
            A.Configure<Book>()
                .Fill(b => b.Name).AsMusicGenreName()
                .Fill(b => b.PublishDate).AsPastDate();

            var books = A.ListOf<Book>(count);

            foreach (var book in books)
            {
                var author = authors.RandomElement();
                book.Author=author;
                book.AuthorId =author.Id;
            }
            return books;
        }

        public List<BookTag> BookTags(List<Book> books, List<Tag> tags, int count = 20)
        {
            A.Configure<BookTag>();

            var bookTags = A.ListOf<BookTag>(count);
            foreach (var bookTag in bookTags)
            {
                var book= books.RandomElement();
                var tag = tags.RandomElement();
                bookTag.Book = book;
                bookTag.BookId = book.Id;
                bookTag.Tag = tag;
                bookTag.TagId = tag.Id;
            }

            return bookTags.DistinctBy(d => new { d.BookId, d.TagId }).ToList();
        }
    }
}