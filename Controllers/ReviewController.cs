using AutoMapper;
using IBDb.Data;
using IBDb.Dto;
using IBDb.Helpers;
using IBDb.Interfaces;
using IBDb.Models;
using IBDb.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Reflection.Metadata.BlobBuilder;

namespace IBDb.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    [Authorize]
    public class ReviewController : Controller
    {
        private readonly ILogger<BookController> _logger;
        private readonly IReviewRepository _reviewRepository;
        private readonly IUserRepository _userRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IConfiguration _configuration;
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public ReviewController(ILogger<ReviewController> logger, IReviewRepository reviewRepository, IUserRepository userRepository,IBookRepository bookRepository, IConfiguration configuration,  DataContext context, IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _userRepository = userRepository;
            _bookRepository = bookRepository;
            _configuration = configuration;
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("book/{id}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewDto>))]
        public IActionResult GetBookReviews(int id)
        {
            var reviews = _reviewRepository.GetBookReviews(id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewsDto = _mapper.Map<List<ReviewDto>>(reviews);

            return Ok(reviewsDto);
        }

        [HttpPost("create")]
        [ProducesResponseType(200, Type = typeof(Review))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult CreateReview([FromBody] ReviewCreateDto reviewCreateDto)
        {
            try
            {
                if (reviewCreateDto == null)
                {
                    ModelState.AddModelError("", "Review data is missing.");
                    return BadRequest(ModelState);
                }

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = _userRepository.GetUser(reviewCreateDto.UserID);
                if (user == null)
                {
                    ModelState.AddModelError("", "User not found.");
                    return BadRequest(ModelState);
                }

                var book = _bookRepository.GetBook(reviewCreateDto.BookID);
                if (book == null)
                {
                    ModelState.AddModelError("", "Book not found.");
                    return BadRequest(ModelState);
                }

                var review = _mapper.Map<Review>(reviewCreateDto);
                review.User = user;
                review.Book = book;

                if (!_reviewRepository.CreateReview(review))
                {
                    ModelState.AddModelError("", "Something went wrong while saving the review.");
                    return StatusCode(500, new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors) });
                }

                var reviewDto = _mapper.Map<ReviewDto>(review);

                return Ok(new { success = true, payload = reviewDto });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during review creation.");
                return StatusCode(500, "Something went wrong while saving the review.");
            }
        }

        [HttpDelete("delete/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult DeleteReview(int id)
        {
            try
            {
                var existingReview = _reviewRepository.GetReview(id);
                if (existingReview == null)
                    return NotFound("Review not found.");

                _reviewRepository.DeleteReview(existingReview);
                return Ok("Review deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while deleting the book: {ex.Message}");
            }
        }
    }
    
}
