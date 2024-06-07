using AutoMapper;
using IBDb.Dto;
using IBDb.Interfaces;
using IBDb.Models;
using IBDb.Repository;
using Microsoft.AspNetCore.Mvc;

namespace IBDb.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class RoleController : Controller
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;

        public RoleController(IRoleRepository roleRepository, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        [HttpGet("list")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Role>))]
        public IActionResult GetBooks()
        {
            var roles = _mapper.Map<List<Role>>(_roleRepository.GetRoles());
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(roles);
        }

        [HttpGet("{roleName}")]
        [ProducesResponseType(200, Type = typeof(Role))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetRole(string roleName)
        {
            var role = _mapper.Map<Role>(_roleRepository.GetRole(roleName));
            if (!ModelState.IsValid) return BadRequest(ModelState);

            return Ok(role);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Role))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GeRole(int id)
        {
            var role = _mapper.Map<Role>(_roleRepository.GetRole(id));
            if (!ModelState.IsValid) return BadRequest(ModelState);

            return Ok(role);
        }

        [HttpPost("create")]
        [ProducesResponseType(200, Type = typeof(Role))]
        [ProducesResponseType(400)]
        public IActionResult CreateRole([FromBody] RoleCreateDto roleCreateDto)
        {
            if (roleCreateDto == null)
                return BadRequest(ModelState);
            if (!ModelState.IsValid)
                 return BadRequest(ModelState);
            
            var role = _mapper.Map<Role>(roleCreateDto);

            if (!_roleRepository.CreateRole(role))
            {
                ModelState.AddModelError("", "Something went wrong while creating the role");
                return StatusCode(500, ModelState);
            }

            return Ok(new { success = true, payload = role });
        }
    }
}
