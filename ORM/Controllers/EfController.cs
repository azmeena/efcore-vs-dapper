using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ORM.Domain;
using ORM.EfCore;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.Logging;
using ORM.LogProvider;

namespace ORM.Controllers
{
    public class EfController : Controller
    {
        private string RunGroupName = Guid.NewGuid().ToString();

        public IActionResult Insert()
        {
            try
            {
                var seed = new Seed();
                
                var authors = new List<Author>();
                var tags = new List<Tag>();
                var books = new List<Book>();

                authors.Populate(seed.Authors, 1000000);
                tags.Populate(seed.Tags, 1000000);

                SwAuthor.Start();
                _authorRepository.Insert(authors);
            
                SwAuthor.Stop();

                SwTags.Start();
                _tagRepository.Insert(tags);
                SwTags.Stop();

                books.Populate(a => seed.Books(authors, a), 1000000);

                SwBooks.Start();
                _bookRepository.Insert(books);
                SwBooks.Stop();

                SwBookTags.Start();
                books = _bookRepository.Table.ToList();
                tags = _tagRepository.Table.ToList();
                SwBookTags.Stop();

                var bookTags = new List<BookTag>().Populate(bt => seed.BookTags(books, tags, bt), 1000000);

                bookTags = bookTags.DistinctBy(x => new { x.BookId, x.TagId }).ToList();

                SwBookTags.Start();
                _bookTagRepository.Insert(bookTags);
                SwBookTags.Stop();

                return Json(new
                {
                    Author = SwAuthor.Elapsed.TotalSeconds,
                    Book = SwBooks.Elapsed.TotalSeconds,
                    tags = SwTags.Elapsed.TotalSeconds,
                    BookTags = SwBookTags.Elapsed.TotalSeconds

                });
            }
            catch (System.Exception e)
            {
                _logger.Error(methodName: "Insert", message: "Insert failed from EntityFramework", programName: "Logger", runGroup: RunGroupName, keyValue: "125-67-546567-045", exception: e, additionalData: new[] { e.StackTrace, e.Message, e.Source });
                return Json(new
                {

                });
            }
        }

        #region StoredProcedure
        public IActionResult SpBookWithId()
        {
            SwBooks.Start();
            var testBook = _bookRepository.SpGetById(673775);  
            SwBooks.Stop();

            _logger.Info(
                methodName: "SpBookWithId",
                message: "Stored Procedure in EF Core to get book with Id",
                runGroup: RunGroupName,
                keyValue: "125-67-4567-045",
                programName: "Logger",
                additionalData: new List<object> { testBook }
            );

            return Json(new
            {
                BookObject = testBook,
                time = SwBooks.Elapsed.TotalSeconds
            });
        }

        public IActionResult SpBooksOnly()
        {
            SwBooks.Start();
            var books = _bookRepository.SpGetAll();
            SwBooks.Stop();

            _logger.Info(
                methodName: "SpBooksOnly",
                message: "Stored Procedure in EF Core to get all books",
                runGroup: RunGroupName,
                keyValue: "125-67-4567-045",
                programName: "Logger",
                additionalData: new List<object> { books }
            );

            return Json(new
            {
                total = books.Count(),
                time = SwBooks.Elapsed.TotalSeconds,
                books
            });
        }

        public IActionResult SpBooksWithAuthorAndTag()
        {
            SwBooks.Start();
            var json = _bookRepository.Including(x => x.BookTags)
                                      .Including(x=>x.Author)
                                      .SpGetAll().ToList();
            SwBooks.Stop();
            return Json(new
            {
                total = json.Count(),
                time = SwBooks.Elapsed.TotalSeconds,
                json
            });
        }

        #endregion

        #region EfBasic
        public IActionResult BooksOnly()
        {
            SwBooks.Start();
            var books = _bookRepository.AsNoTracking.ToList();   
            SwBooks.Stop();

            _logger.Performance(
                methodName: "BooksOnly",
                elapsedTime: SwBooks.Elapsed.Seconds.ToString(),
                message: $"EF Core Retrieves 1000000 books in {SwBooks.Elapsed.Seconds.ToString()} seconds",
                runGroup: RunGroupName,
                keyValue: "125-67-546567-045",
                programName: "Logger"
            );

            return Json(new
            {
                total = books.Count(),
                time = SwBooks.Elapsed.TotalSeconds,
                books
            });
        }

        public IActionResult AuthorWithId()
        {
            SwAuthor.Start();

            var testAuthor = _authorRepository.GetById(3770181);
            _logger.Info(
                methodName: "AuthorWithId",
                message: "Obtaining Author by id using EF Core",
                runGroup: RunGroupName,
                keyValue: "125-67-4567-045",
                programName: "Logger",
                additionalData: new List<object> { testAuthor }
            );

            SwAuthor.Stop();
            return Json(new
            {
                AuthorObject = testAuthor,
                time = SwAuthor.Elapsed.TotalSeconds
            });
        }

        public IActionResult BookWithId()
        {

            SwBooks.Start();
            var testBook = _bookRepository.GetById(1185167);
            _logger.Debug(
                methodName: "BookWithId",
                message: "Obtaining book by id using EF core",
                runGroup: RunGroupName,
                keyValue: "125-67-546567-045",
                programName: "Logger",
                additionalData: new List<object> { "" }
            );

            SwBooks.Stop();
            return Json(new
            {
                BookObject = testBook,
                time = SwBooks.Elapsed.TotalSeconds
            });
        }

        public IActionResult BookIncludingAuthor()
        {
            SwBooks.Start();

            var testBook = _bookRepository.AsNoTracking.Include(b => b.Author).ToList();

            SwBooks.Stop();
            return Json(new
            {
                BookObject = testBook,
                time = SwBooks.Elapsed.TotalSeconds
            });
        }

        public IActionResult BooksWithAuthor()
        {
            SwBooks.Start();
            var json = (from book in _bookRepository.AsNoTracking
                        join author in _authorRepository.AsNoTracking on book.AuthorId equals author.Id
                        select new
                        {
                            book = book.Name,
                            date = book.PublishDate.ToShortDateString(),
                            author = author.Name
                        }).ToList();

            _logger.Info(
                methodName: "BooksWithAuthor",
                message: "Join operation to get books with the same Author Id",
                runGroup: RunGroupName,
                keyValue: "125-67-4567-045",
                programName: "Logger",
                additionalData: new List<object> { json }
            );

            SwBooks.Stop();

            _logger.Performance(
                methodName: "BooksWithAuthor",
                elapsedTime: SwBooks.Elapsed.Seconds.ToString(),
                message: $"EF Core Retrieves books with Join operation in {SwBooks.Elapsed.Seconds.ToString()} seconds",
                runGroup: RunGroupName,
                keyValue: "125-67-546567-045",
                programName: "Logger" 
            );

            return Json(new
            {
                total = json.Count(),
                time = SwBooks.Elapsed.TotalSeconds,
                json = json
            });
        }

        public IActionResult BooksWithAuthorEagerLoad()
        {
            SwBooks.Start();

            var json = _bookRepository.Table.Include(b=>b.Author).Select(book => new
            {
                book = book.Name,
                date = book.PublishDate.ToShortDateString(),
                author = book.Author.Name
            }).ToList();

            //var json = _bookRepository.Table.Select(book => new
            //{
            //    book = book.Name,
            //    date = book.PublishDate.ToShortDateString(),
            //    author = book.Author.Name
            //}).ToList();

            _logger.Info(
                methodName: "BooksWithAuthorEagerLoad",
                message: "Eager Load operation to get books with the Author",
                runGroup: RunGroupName,
                keyValue: "125-67-4567-045",
                programName: "Logger",
                additionalData: new List<object> { json }
            );

            SwBooks.Stop();

            _logger.Performance(
                methodName: "BooksWithAuthorEagerLoad",
                elapsedTime: SwBooks.Elapsed.Seconds.ToString(),
                message: $"EF Core Retrieves books with Eager Load operation in {SwBooks.Elapsed.Seconds.ToString()} seconds",
                runGroup: RunGroupName,
                keyValue: "125-67-546567-045",
                programName: "Logger"
            );

            return Json(new
            {
                total = json.Count(),
                time = SwBooks.Elapsed.TotalSeconds,
                json = json
            });

        }

        public IActionResult Index()
        {
            return View();
        }
        #endregion

        #region Props

        private readonly ILogger<DBLogger> _logger;
        private readonly IEfRepository<Author> _authorRepository;
        private readonly IEfRepository<Tag> _tagRepository;
        private readonly IEfRepository<Book> _bookRepository;
        private readonly IEfRepository<BookTag> _bookTagRepository; 
        #endregion

        #region Fields
        private Stopwatch _swAuthor;
        private Stopwatch _swTags;
        private Stopwatch _swBookTags;
        private Stopwatch _swBooks;

        public Stopwatch SwAuthor => _swAuthor ?? (_swAuthor = new Stopwatch());
        public Stopwatch SwTags => _swTags ?? (_swTags = new Stopwatch());
        public Stopwatch SwBookTags => _swBookTags ?? (_swBookTags = new Stopwatch());
        public Stopwatch SwBooks => _swBooks ?? (_swBooks = new Stopwatch());
        #endregion

        #region Ctor
        public EfController(
           ILogger<DBLogger> logger,
           IEfRepository<Book> bookRepository,
           IEfRepository<Author> authorRepository,
           IEfRepository<Tag> tagRepository,
           IEfRepository<BookTag> bookTagRepository)
        {
            _logger = logger;
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
            _tagRepository = tagRepository;
            _bookTagRepository = bookTagRepository;
        } 
        #endregion
    }
}