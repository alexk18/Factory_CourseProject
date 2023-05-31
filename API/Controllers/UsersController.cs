using API.BLL.DTO.Request;
using API.BLL.DTO.Response;
using API.DAL.EF;
using API.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TestProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UsersController(
            ApplicationContext context,
            IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserResponse>>> GetUsers()
        {
            var userId = HttpContext.User.Identity!.Name;

            if (userId == null) { return BadRequest("HttpContext.User.Identity.Name is null"); }

            var users = await _context.Users
                .Where(x => x.Id.ToString() != userId)
                .Select(x => new UserResponse
                {
                    Id = x.Id,
                    Email = x.Email,
                    Firstname = x.Firstname,
                    Lastname = x.Lastname,
                    Role = x.Role,
                })
                .ToListAsync();

            return users;
        }

        [HttpGet("{id}/visits")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserVisits(Guid id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound("No user found");
            }

            return Ok(new
            {
                user.Id,
                user.Email,
                user.Firstname,
                user.Lastname,
                user.Role,
                Visits = await _context.Visits.Where(x => x.UserId == id).Select(x => new
                {
                    x.Id,
                    x.Created,
                    x.Room!.Number
                }).ToListAsync()
            });
        }

        [HttpPost("create")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserResponse>> PostUser(UserRequest model)
        {
            if (await _context.Users.AnyAsync(x => x.Email == model.Email))
            {
                return BadRequest("User with such Email exists");
            }

            var user = new User
            {
                Email = model.Email,
                Role = model.Role,
                Lastname = model.Lastname,
                Firstname = model.Firstname,
            };

            user.Password = _passwordHasher.HashPassword(user, model.Password);

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                Lastname = user.Lastname,
                Firstname = user.Firstname,
                Role = user.Role
            };
        }

        [HttpPut("edit/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutUser(Guid id, EditUserRequest model)
        {
            if (id != model.UserId) { return BadRequest("UserId is not same in model and Guid"); }

            var user = await _context.Users.FindAsync(model.UserId);

            if (user == null)
            {
                return NotFound("No user found");
            }

            user.Lastname = model.Lastname;
            user.Firstname = model.Firstname;
            user.Role = model.Role;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound("No user found");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
