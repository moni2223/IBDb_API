using IBDb.Models;
using Microsoft.EntityFrameworkCore;

namespace IBDb.Interfaces
{
    public interface IGenreRepository
    {
        ICollection<Genre> GetGenres();
        ICollection<Genre> GetGenresById(List<int> genreIds);
        Genre GetGenre(int id);
        public bool GenreExists(int id);
        bool CreateGenre(Genre genre);
        bool DeleteGenre(Genre genre);
        bool Save();

    }
}
