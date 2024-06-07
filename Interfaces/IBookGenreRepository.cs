using IBDb.Models;

namespace IBDb.Interfaces
{
    public interface IBookGenreRepository
    {
        IEnumerable<Genre> GetGenresForBook(int bookId);
    }
}
