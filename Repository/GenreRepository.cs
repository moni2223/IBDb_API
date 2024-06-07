using IBDb.Data;
using IBDb.Interfaces;
using IBDb.Models;

namespace IBDb.Repository
{
    public class GenreRepository : IGenreRepository
    {
        private readonly DataContext _context;

        public GenreRepository(DataContext context)
        {
            _context = context;
        }

        public Genre GetGenre(int id)
        {
            return _context.Genres.Where(g => g.Id == id).FirstOrDefault();
        }

        public bool GenreExists(int id)
        {
            return _context.Genres.Any(g => g.Id == id);
        }

        ICollection<Genre> IGenreRepository.GetGenres()
        {
            return _context.Genres.OrderBy(g => g.Id).ToList();
        }

        ICollection<Genre> IGenreRepository.GetGenresById(List<int> Ids)
        {
            return _context.Genres.Where(g => Ids.Contains(g.Id)).ToList();
        }

        public bool CreateGenre(Genre genre)
        {
            _context.Add(genre);
            return Save();
        }

        public bool DeleteGenre(Genre genre)
        {
            _context.Remove(genre);
            return Save();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0;
        }
    }
}
