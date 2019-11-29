using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ORM.Dapper;
using ORM.Domain;
using ORM.LogProvider;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ORM.Controllers
{
    public class DpController : Controller
    {
        private string RunGroupName = Guid.NewGuid().ToString();

        public IActionResult Insert()
        {
            try
            {
                var seed = new Seed();
               
                var authors = new List<Author>().Populate(seed.Authors, EntryLimit);
                var tags = new List<Tag>().Populate(seed.Tags, EntryLimit);

                SwAuthor.Start();
                _authorRepository.Insert(authors);
           
                SwAuthor.Stop();

                authors = _authorRepository.GetAll();
                var books = new List<Book>().Populate(a => seed.Books(authors, a), EntryLimit);

                SwTags.Start();
                _tagRepository.Insert(tags);
                SwTags.Stop();

                SwBooks.Start();
                _bookRepository.Insert(books);
                SwBooks.Stop();

                SwBookTags.Start();
                books = _bookRepository.GetAll();
                tags = _tagRepository.GetAll();
                SwBookTags.Stop();

                var bookTags = new List<BookTag>().Populate(bt => seed.BookTags(books, tags, bt), EntryLimit);

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
                var watch = e;
             _logger.Error( methodName: "Insert", message: "Insert failed from Dapper", programName: "Logger", runGroup: RunGroupName, keyValue: "125-67-4567-045", exception: e, additionalData: new []{e.StackTrace, e.Message, e.Source});
             return Json(new
             {
               
             });
            }
        }

        public IActionResult BooksOnly()
        {
            SwBooks.Start();
            var books = _bookRepository.GetAll();
            SwBooks.Stop();

            _logger.Performance(
                methodName: "BooksOnly",
                elapsedTime: SwBooks.Elapsed.Seconds.ToString(),
                message: $"Dapper Retrieves 1000000 books in {SwBooks.Elapsed.Seconds.ToString()} seconds",
                runGroup: RunGroupName,
                keyValue: "125-67-4567-045",
                programName: "Logger"
            );

            return Json(new
            {
                total = books.Count(),
                time = SwBooks.Elapsed.TotalSeconds,
                books = books
            });
        }

        public IActionResult AuthorWithId(int id)
        {
            SwAuthor.Start();

            var testAuthor = _authorRepository.GetById(3770181);
            _logger.Info(
                methodName: "AuthorWithId",
                message: "Obtaining Author by id using Dapper",
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

        public IActionResult BookWithId(int id)
        {
            SwBooks.Start();

            var testBook = _bookRepository.GetById(1185167);
            _logger.Debug(
                methodName: "BookWithId",
                message: "Obtaining book by id using Dapper",
                runGroup: RunGroupName,
                keyValue: "125-67-4567-045",
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

        public IActionResult BooksWithAuthor()
        {
            SwBooks.Start();
            var json = (from book in _bookRepository.GetAll()
                        join author in _authorRepository.GetAll() on book.AuthorId equals author.Id
                        select new
                        { 
                            book = book.Name,
                            date = book.PublishDate.ToShortDateString(),
                            author = author.Name
                        }).ToList();

            SwBooks.Stop();

            return Json(new
            {
                total = json.Count(),
                time = SwBooks.Elapsed.TotalSeconds,
                json = json
            });
        }

        public IActionResult SpBooksOnly()
        {
            SwBooks.Start();
            var books = _bookRepository.SpGetAll();
            SwBooks.Stop();

            _logger.Info(
                methodName: "SpBooksOnly",
                message: "Stored Procedure in Dapper to get all books",
                runGroup: RunGroupName,
                keyValue: "125-67-4567-045",
                programName: "Logger",
                additionalData: new List<object> { books }
            );

            return Json(new
            {
                total = books.Count(),
                time = SwBooks.Elapsed.TotalSeconds,
                books = books
            });
        }

        public IActionResult SpBookWithId()
        {

            SwBooks.Start();
            var testBook = _bookRepository.SpGetById(673775);
            SwBooks.Stop();

            _logger.Info(
                methodName: "SpBookWithId",
                message: "Stored Procedure in Dapper to get book with Id",
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

        public IActionResult SpBooksWithAuthor()
        {
            SwBooks.Start();
            var json = _bookRepository.WithAuthor();
            SwBooks.Stop();

            return Json(new
            {
                total = json.Count(),
                time = SwBooks.Elapsed.TotalSeconds,
                json = json
            });
        }

        public IActionResult SpBooksWithTags()
        {
            SwBooks.Start();
            var json = _bookRepository.WithTags();
            SwBooks.Stop();

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

            var json = _bookRepository.GetAll().Select(book => new
            {
                book = book.Name,
                date = book.PublishDate.ToShortDateString(),
                author = book.Author.Name
            }).ToList();
            SwBooks.Stop();

            return Json(new
            {
                total = json.Count(),
                time = SwBooks.Elapsed.TotalSeconds,
                json = json
            });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _tagRepository.Dispose();
                _authorRepository.Dispose();
                _bookRepository.Dispose();
                _bookTagRepository.Dispose();
            }
            base.Dispose(disposing);
        }

        private readonly IAuthorRepository _authorRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IBookTagRepository _bookTagRepository;
        private readonly ILogger<DBLogger> _logger;
        private Stopwatch _swAuthor;
        private Stopwatch _swTags;
        private Stopwatch _swBookTags;
        private Stopwatch _swBooks;
        private const int EntryLimit = 1000000;

        public Stopwatch SwAuthor => _swAuthor ?? (_swAuthor = new Stopwatch());
        public Stopwatch SwTags => _swTags ?? (_swTags = new Stopwatch());
        public Stopwatch SwBookTags => _swBookTags ?? (_swBookTags = new Stopwatch());
        public Stopwatch SwBooks => _swBooks ?? (_swBooks = new Stopwatch());

        public DpController(
               ILogger<DBLogger> logger,
               IAuthorRepository authorRepository,
               ITagRepository tagRepository,
               IBookRepository bookRepository,
               IBookTagRepository bookTagRepository)
        {
            _logger = logger;
            _authorRepository = authorRepository;
            _tagRepository = tagRepository;
            _bookRepository = bookRepository;
            _bookTagRepository = bookTagRepository;
        }
    }
}