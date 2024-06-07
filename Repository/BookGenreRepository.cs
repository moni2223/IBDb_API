using IBDb.Data;
using IBDb.Interfaces;
using IBDb.Models;
using Microsoft.EntityFrameworkCore;

namespace IBDb.Repository
{
    public class BookGenreRepository : IBookGenreRepository
    {
        private readonly DataContext _context;

        public BookGenreRepository(DataContext context)
        {
            _context = context;
        }

        public IEnumerable<Genre> GetGenresForBook(int bookId)
        {
            var query = _context.BookGenres
               .Where(bg => bg.BookId == bookId)
               .Select(bg => bg.Genre);

            // Log the generated SQL query
            var sql = query.ToQueryString();
            Console.WriteLine(sql);

            var genres = query.ToList();
            return genres;
        }
    }
}
