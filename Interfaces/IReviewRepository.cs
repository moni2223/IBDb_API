using IBDb.Models;

namespace IBDb.Interfaces
{
    public interface IReviewRepository
    {
        ICollection<Review> GetBookReviews(int id);
        Review GetReview(int id);
        bool CreateReview(Review review);
        bool DeleteReview(Review review);
        bool Save();
    }
}
