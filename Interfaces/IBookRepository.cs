using IBDb.Dto;
using IBDb.Models;

namespace IBDb.Interfaces
{
    public interface IBookRepository
    {
        ICollection<Book> GetBooks(int pageNumber, int pageSize);
        ICollection<Book> GetAllBooks();
        ICollection<Book> GetBooksByQuery(string query);
        ICollection<Book> GetBooksForUser(int userId);
        int GetBooksCount();
        Book GetBook(int id);
        Book GetBook(string title);
        bool BookExists(int id);
        bool CreateBook(Book book);
        bool UpdateBook(Book book);
        bool DeleteBook(Book book);
        bool Save();
    }
}
