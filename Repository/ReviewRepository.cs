using IBDb.Data;
using IBDb.Interfaces;
using IBDb.Models;
using Microsoft.EntityFrameworkCore;

namespace IBDb.Repository
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly DataContext _context;

        public ReviewRepository(DataContext context)
        {
            _context = context;
        }
        public ICollection<Review> GetBookReviews(int id)
        {
            return _context.Reviews.Where(r => r.BookID == id).Include(r => r.User).OrderBy(r => r.CreatedAt).ToList();
        }
        public Review GetReview(int id) {
            return _context.Reviews.Where(r => r.ReviewID == id).FirstOrDefault();
        }
        public bool CreateReview(Review review)
        {
            _context.Add(review);
            return Save();
        }
        public bool DeleteReview(Review review)
        {
            try
            {
                _context.Remove(review);
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
