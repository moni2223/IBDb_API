using IBDb.Data;
using IBDb.Dto;
using IBDb.Interfaces;
using IBDb.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace IBDb.Repository
{
    public class BookRepository : IBookRepository
    {
        private readonly DataContext _context;

        public BookRepository(DataContext context) {
            _context = context;
        }
        public List<Genre> GetGenresByBookId(int bookId)
        {
            var genres = _context.Genres
                .FromSqlRaw("SELECT g.* FROM Genres g INNER JOIN BookGenres bg ON g.GenreID = bg.GenreId WHERE bg.BookId = {0}", bookId)
                .ToList();

            return genres;
        }
        public Book GetBook(int id)
        {
            var book = _context.Books
                .Include(b => b.Publisher)
                .FirstOrDefault(b => b.Id == id);
            return book;
        }
        public Book GetBook(string title) {
            var book = _context.Books
                 .Include(b => b.Publisher)
                 .FirstOrDefault(b => b.Title == title);
            return book;
        }
        public bool BookExists(int id)
        {
            return _context.Books.Any(p => p.Id == id);
        }
        public int GetBooksCount()
        {
            return _context.Books.Count();
        }
        public ICollection<Book> GetBooksByQuery(string query)
        {
            return _context.Books
                .Where(b => b.Title.Contains(query))
                .Include(b => b.Publisher)
                .OrderBy(b => b.CreatedAt)
                .ToList();
        }
        public ICollection<Book> GetAllBooks()  // ICollection you can add / remove but only to the end
        {
            return _context.Books.Include(b => b.Publisher).OrderBy(p => p.CreatedAt).ToList();
        }
        public ICollection<Book> GetBooks(int pageNumber, int pageSize)
        {
            return _context.Books
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Include(b => b.Publisher)
          .ToList();
        }
        public ICollection<Book> GetBooksForUser(int userId)
        {
            return _context.Books
            .Where(b => b.PublisherId == userId)
            .Include(b => b.Publisher)
            .ToList();
        }

        public bool CreateBook(Book book)
        {
            _context.Add(book);
            return Save();
        }
        public bool UpdateBook(Book book)
        {
            _context.Update(book);
            return Save();
            
        }
        public bool DeleteBook(Book book)
        {
            try
            {
                var bookGenres = _context.BookGenres.Where(bg => bg.BookId == book.Id);
                _context.BookGenres.RemoveRange(bookGenres);
                _context.Remove(book);

                return Save();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while deleting the book: {ex.Message}");
                return false;
            }
        }
        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0;
        }
    }
}
