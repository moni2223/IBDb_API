using IBDb.Models;

namespace IBDb.Dto
{
    public class BookDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Cover { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int? PublisherId { get; set; } // we have the id and the user on different fields, because it will be easier to list all the books for current publisher.
        public PublisherDto Publisher { get; set; }
        public ICollection<Genre> Genres { get; set; }
        public ICollection<BookGenre> BookGenres { get; set; } = new List<BookGenre>();
    }

    public class BookCreateDto
    {
        public string Title { get; set; }
        public string Cover { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public int? PublisherId { get; set; }
        public List<int> GenreIds { get; set; }
    }
    public class BookUpdateDto
    {
        public string Title { get; set; }
        public string Cover { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public int? PublisherId { get; set; }
    }
    public class BookWithGenresDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public IEnumerable<string> Genres { get; set; }
    }
}
