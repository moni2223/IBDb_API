namespace IBDb.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Cover { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int? PublisherId { get; set; } // we have the id and the user on different fields, because it will be easier to list all the books for current publisher.
        public User Publisher { get; set; }
        public ICollection<Genre> Genres { get; set; }
        public ICollection<BookGenre> BookGenres { get; set; } = new List<BookGenre>();
        public ICollection<Review> Reviews { get; set; }
    }
}
