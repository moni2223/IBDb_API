using AutoMapper;
using IBDb.Data;
using IBDb.Dto;
using IBDb.Helpers;
using IBDb.Interfaces;
using IBDb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IBDb.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IConfiguration _configuration;
        private readonly HelperFunctions _helperFunctions;
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public UserController(IUserRepository userRepository,IRoleRepository roleRepository,IConfiguration configuration ,HelperFunctions helperFunctions,DataContext context, IMapper mapper)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _configuration = configuration;
            _helperFunctions = helperFunctions;
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("list")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<User>))]
        public IActionResult GetUsers()
        {
            var users = _mapper.Map<List<UserListDto>>(_userRepository.GetUsers());

            foreach (var user in users) { 
                var role = _roleRepository.GetRole(user.RoleId);
                if (role == null) return BadRequest(ModelState);
                user.Role = role;
            }
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(users);
        }

        [HttpGet("search/{query}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<User>))]
        [ProducesResponseType(400)]
        public IActionResult GetUsersByQuery(string query)
        {
            var users = _mapper.Map<List<BookDto>>(_userRepository.GetUsersByQuery(query));
            if (!ModelState.IsValid) return BadRequest(ModelState);

            return Ok(users);
        }

        [HttpGet("email/{email}")]
        [ProducesResponseType(200, Type = typeof(Book))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetUserByEmail(string email)
        {
            var user = _mapper.Map<User>(_userRepository.GetUserByEmail(email));
            if (!ModelState.IsValid) return BadRequest(ModelState);

            return Ok(user);
        }

        [HttpPost("register")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateUser([FromBody] UserCreateDto userCreateDto)
        {
            if (userCreateDto == null)
                return BadRequest(ModelState);

            var users = _userRepository.GetUsers()
                .Where(c => c.UserName.Trim().ToUpper() == userCreateDto.UserName.TrimEnd().ToUpper())
                .FirstOrDefault();

            if (users != null)
            {
                ModelState.AddModelError("", "Owner already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var role = _context.Roles.SingleOrDefault(r => r.RoleName == userCreateDto.RoleName);
            if (role == null)
            {
                return BadRequest("Role does not exist.");
            }

            var userMap = _mapper.Map<User>(userCreateDto);
            userMap.PasswordHash = _helperFunctions.HashPassword(userCreateDto.Password);
            userMap.Role = role;

            if (!_userRepository.CreateUser(userMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }

        [HttpPost("login")]
        public IActionResult Login(LoginDto model)
        {
            var user = _userRepository.GetUserByEmail(model.Email);
            if (user == null || !VerifyPasswordHash(model.Password, user.PasswordHash))
                return Unauthorized();

            var token = GenerateJwtToken(user);

            var userDto = _mapper.Map<UserSuccessfullLoginDto>(user);
            var roleDto = _mapper.Map<RoleUpdateDto>(user.Role);

            userDto.RoleName = roleDto.RoleName;

            return Ok(new { User = userDto, Token = token});
        }

        private string GenerateJwtToken(User user)
        {
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:SecretKey"]);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                }),
                Expires = DateTime.UtcNow.AddHours(Convert.ToDouble(_configuration["JwtSettings:ExpirationHours"])),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private bool VerifyPasswordHash(string password, string storedHash)
        {
            return _helperFunctions.VerifyPasswordHash(password, storedHash);
        }

    }
}
