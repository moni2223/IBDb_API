using AutoMapper;
using IBDb.Dto;
using IBDb.Interfaces;
using IBDb.Models;
using IBDb.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IBDb.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    [Authorize]
    public class GenreController : Controller
    {
        private readonly IGenreRepository _genreRepository;
        private readonly IMapper _mapper;

        public GenreController(IGenreRepository genreRepository, IMapper mapper)
        {
            _genreRepository = genreRepository;
            _mapper = mapper;
        }

        [HttpGet("list")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Genre>))]
        public IActionResult GetGenres()
        {
            var genres = _mapper.Map<List<Genre>>(_genreRepository.GetGenres().ToList());
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(genres);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Genre))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetGenre(int id)
        {
            if (!_genreRepository.GenreExists(id)) return NotFound();
            var genre = _mapper.Map<Genre>(_genreRepository.GetGenre(id));
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(genre);
        }

        [HttpPost("create")]
        [ProducesResponseType(200, Type = typeof(Genre))]
        [ProducesResponseType(400)]
        public IActionResult CreateGenre([FromBody] GenreCreateDto genreCreateDto)
        {
            if (genreCreateDto == null)
                return BadRequest(ModelState);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var genre = _mapper.Map<Genre>(genreCreateDto);

            if (!_genreRepository.CreateGenre(genre))
            {
                ModelState.AddModelError("", "Something went wrong while creating the role");
                return StatusCode(500, ModelState);
            }

            return Ok(new { success = true, payload = genre });
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult DeleteGenre(int id)
        {
            if (!_genreRepository.GenreExists(id))
                 return NotFound();
            
            var role = _genreRepository.GetGenre(id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            if (!_genreRepository.DeleteGenre(role))
            {
                ModelState.AddModelError("", "Something went wrong while deleting the role");
                return StatusCode(500, ModelState);
            }

            return Ok(new {success = true });
        }
    }
}
