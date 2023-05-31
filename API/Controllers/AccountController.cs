using API.BLL.DTO.Request;
using API.BLL.DTO.Response;
using API.BLL.Services.JWT;
using API.BLL.Services.JWT.Models;
using API.DAL.EF;
using API.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly IJwtService _jwtService;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AccountController(ApplicationContext context,
            IJwtService jwtService,
            IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _jwtService = jwtService;
            _passwordHasher = passwordHasher;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthorizeResponse>> Register(RegisterRequest model)
        {
            if (await _context.Users.AnyAsync(x => x.Email == model.Email))
            {
                return BadRequest("User with such Email exists");
            }

            var newUser = new User
            {
                Lastname = model.Lastname,
                Firstname = model.Firstname,
                Email = model.Email
            };

            newUser.Password = _passwordHasher.HashPassword(newUser, model.Password);

            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();

            var token = _jwtService.GetToken(new JwtUser { Id = newUser.Id, Role = newUser.Role });

            var response = new AuthorizeResponse
            {
                Role = newUser.Role.ToString(),
                UserId = newUser.Id,
                Token = token
            };

            return response;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthorizeResponse>> Authenticate(AuthenticateRequest model)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.Email == model.Email);

            if (user == null)
            {
                return NotFound("Email or password is incorrect");
            }

            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, model.Password);

            if (verificationResult == PasswordVerificationResult.Failed)
            {
                return BadRequest("Email or password is incorrect");
            }
            else if (verificationResult == PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.Password = _passwordHasher.HashPassword(user, model.Password);
                await _context.SaveChangesAsync();
            }

            var token = _jwtService.GetToken(new JwtUser { Id = user.Id, Role = user.Role });

            var response = new AuthorizeResponse
            {
                Role = user.Role.ToString(),
                UserId = user.Id,
                Token = token
            };

            return response;
        }

        [HttpGet("profileInfo")]
        [Authorize]
        public async Task<ActionResult<ProfileResponse>> GetProfile()
        {
            var userId = HttpContext.User.Identity!.Name;

            if (userId == null) { return BadRequest("HttpContext.User.Identity.Name is null"); }

            var user = await _context.Users
                .SingleOrDefaultAsync(x => x.Id.ToString() == userId);

            if (user == null)
            {
                return NotFound("No user found");
            }

            return new ProfileResponse
            {
                Email = user.Email,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Role = user.Role.ToString()
            };
        }

        [HttpPut("edit/profileInfo")]
        [Authorize]
        public async Task<IActionResult> PutUserProfile(ProfileRequest profile)
        {
            var userId = HttpContext.User.Identity!.Name;

            if (userId == null) { return BadRequest("HttpContext.User.Identity.Name is null"); }

            var user = await _context.Users.SingleOrDefaultAsync(x => x.Id.ToString() == userId);

            if (user == null)
            {
                return NotFound("No user found");
            }

            user.Lastname = profile.Lastname;
            user.Firstname = profile.Firstname;

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
