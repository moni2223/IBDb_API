using IBDb.Models;

namespace IBDb.Dto
{
    public class ReviewDto
    {
        public int ReviewID { get; set; }
        public int BookID { get; set; }
        public PublisherDto User { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class ReviewCreateDto
    {
        public int Rating { get; set; }
        public int BookID { get; set; }
        public int UserID { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
