using AutoMapper;
using IBDb.Dto;
using IBDb.Interfaces;
using IBDb.Models;
using IBDb.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IBDb.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    [Authorize]
    public class BookController : Controller
    {
        private readonly ILogger<BookController> _logger;
        private readonly IBookRepository _bookRepository;
        private readonly IUserRepository _userRepository;
        private readonly IGenreRepository _genreRepository;
        private readonly IBookGenreRepository _bookGenreRepository;
        private readonly IMapper _mapper;

        public BookController(ILogger<BookController> logger, IBookRepository bookRepository,IUserRepository userRepository,IGenreRepository genreRepository,IBookGenreRepository bookGenreRepository,IMapper mapper) {
            _logger = logger;
            _bookRepository = bookRepository;
            _userRepository = userRepository;
            _genreRepository = genreRepository;
            _bookGenreRepository = bookGenreRepository;
            _mapper = mapper;
        }

        [HttpPost("list")]
        [ProducesResponseType(200, Type = typeof(PaginatedResponse<BookDto>))]
        public IActionResult GetBooks([FromBody] PaginationDto paginationDto)
        {
            List<Book> books;
            bool hasNextPage = false;

            if (paginationDto.noPagination)
                books = _bookRepository.GetAllBooks().ToList();
            else
            {
                books = _bookRepository.GetBooks(paginationDto.PageNumber, paginationDto.PageSize).ToList();
                var totalBooks = _bookRepository.GetBooksCount();
                hasNextPage = paginationDto.PageNumber * paginationDto.PageSize < totalBooks;
            }

            var booksMap = _mapper.Map<List<BookDto>>(books);

            foreach (var book in booksMap)
            {
                try
                {
                    var genres = _bookGenreRepository.GetGenresForBook(book.Id);
                    book.Genres = genres.ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching genres for book with ID {book.Id}: {ex.Message}");
                }
            }

            var response = new PaginatedResponse<BookDto>
            {
                Docs = booksMap,
                Page = paginationDto.noPagination ? 1 : paginationDto.PageNumber,
                Limit = paginationDto.noPagination ? books.Count : paginationDto.PageSize,
                HasNextPage = !paginationDto.noPagination && hasNextPage
            };

            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(response);
        }

        [HttpGet("personal-list/{userId}")]
        [ProducesResponseType(200, Type = typeof(PaginatedResponse<BookDto>))]
        public IActionResult GetBooksForUser(int userId)
        {
            List<Book> books;

            if (userId == null)
                return BadRequest(ModelState);

            books = _bookRepository.GetBooksForUser(userId).ToList();

            var booksMap = _mapper.Map<List<BookDto>>(books);

            foreach (var book in booksMap)
            {
                try
                {
                    var genres = _bookGenreRepository.GetGenresForBook(book.Id);
                    book.Genres = genres.ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching genres for book with ID {book.Id}: {ex.Message}");
                }
            }

            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(booksMap);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(BookDto))]
        [ProducesResponseType(400)]
        public IActionResult getBook(int id)
        {
            if (!_bookRepository.BookExists(id)) return NotFound();

            var book = _bookRepository.GetBook(id);
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var genres = _bookGenreRepository.GetGenresForBook(id);
            book.Genres = genres.ToList();

            var bookDto = _mapper.Map<BookDto>(book);

            return Ok(bookDto);
        }

        [HttpGet("search/{name}")]
        [ProducesResponseType(200, Type = typeof(BookDto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetBookByName(string name)
        {
            var book = _mapper.Map<Book>(_bookRepository.GetBook(name));
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var genres = _bookGenreRepository.GetGenresForBook(book.Id);
            book.Genres = genres.ToList();

            return Ok(book);
        }

        [HttpGet("find/{query}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<BookDto>))]
        [ProducesResponseType(400)]
        public IActionResult GetBooksByQuery(string query)
        {
            var books = _bookRepository.GetBooksByQuery(query);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var booksDto = _mapper.Map<List<BookDto>>(books);

            foreach (var book in booksDto)
            {
                try
                {
                    var genres = _bookGenreRepository.GetGenresForBook(book.Id);
                    book.Genres = genres.ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching genres for book with ID {book.Id}: {ex.Message}");
                }
            }

            return Ok(booksDto);
        }

        [HttpPost("create")]
        [ProducesResponseType(201, Type = typeof(BookDto))]
        [ProducesResponseType(400)]
        public IActionResult CreateBook([FromBody] BookCreateDto createBookDto)
        {
            try
            {
                _logger.LogInformation("Starting book creation process...");

                if (createBookDto == null)
                {
                    ModelState.AddModelError("", "Book data is missing.");
                    return BadRequest(ModelState);
                }

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var publisher = _userRepository.GetUser(createBookDto.PublisherId.Value);
                if (publisher == null)
                {
                    ModelState.AddModelError("", "Publisher not found.");
                    return BadRequest(ModelState);
                }

                var genres = _genreRepository.GetGenresById(createBookDto.GenreIds);
                if (genres.Count != createBookDto.GenreIds.Count)
                {
                    ModelState.AddModelError("", "One or more genres not found.");
                    return BadRequest(ModelState);
                }

                var book = _mapper.Map<Book>(createBookDto);
                book.Publisher = publisher;

                foreach (var genre in genres)
                    book.BookGenres.Add(new BookGenre { GenreId = genre.Id });
                

                if (!_bookRepository.CreateBook(book))
                {
                    ModelState.AddModelError("", "Something went wrong while saving the book.");
                    return StatusCode(500, new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors) });
                }

                var bookDto = _mapper.Map<BookDto>(book);

                return Ok(new { success = true, payload = bookDto });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during book creation.");
                return StatusCode(500, "Something went wrong while saving the book.");
            }
        }

        [HttpPut("update/{id}")]
        [ProducesResponseType(200, Type = typeof(Book))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult UpdateBook(int id, [FromBody] BookUpdateDto bookUpdateDto)
        {
            if (!_bookRepository.BookExists(id))
                return NotFound("Book not found.");
            try
            {
                var existingBook = _bookRepository.GetBook(id);
                if (existingBook == null)
                    return NotFound("Book not found.");

                existingBook.Title = bookUpdateDto.Title;
                existingBook.Price = bookUpdateDto.Price;
                existingBook.Description = bookUpdateDto.Description;
                existingBook.PublisherId = bookUpdateDto.PublisherId;
                existingBook.Cover = bookUpdateDto.Cover;
                _bookRepository.UpdateBook(existingBook);

                return Ok(existingBook);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the book: {ex.Message}");
            }
        }

        [HttpDelete("delete/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult DeleteBook(int id)
        {
            if (!_bookRepository.BookExists(id))
                return NotFound("Book not found.");

            try
            {
                var existingBook = _bookRepository.GetBook(id);
                if (existingBook == null)
                    return NotFound("Book not found.");

                _bookRepository.DeleteBook(existingBook);
                return Ok("Book deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while deleting the book: {ex.Message}");
            }
        }
    }
}
