namespace IBDb.Models
{
    public class Review
    {
        public int ReviewID { get; set; }
        public int BookID { get; set; }
        public Book Book { get; set; }
        public int UserID { get; set; }
        public User User { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
