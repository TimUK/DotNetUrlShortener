using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using URLShortener.Data;
using URLShortener.Interfaces;
using URLShortener.Models;
using URLShortener.Services;

namespace URLShortener.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ITokenCreationService _tokenCreationService;
        private readonly LinkAPIContext _linkAPIContext;

        public UsersController(ITokenCreationService tokenCreationService, UserManager<IdentityUser> userManager, LinkAPIContext linkAPIContext)
        {
            _tokenCreationService = tokenCreationService;
            _userManager = userManager;
            _linkAPIContext = linkAPIContext;
        }

        /// <summary>
        /// Administrative/test endpoint to create aspnetcore identity users
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userManager.CreateAsync(
                new IdentityUser() { UserName = user.UserName, Email = user.Email },
                user.Password
            );

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            user.Password = null;
            return CreatedAtAction("GetUser", new { username = user.UserName }, user);
        }

        /// <summary>
        /// Lookup aspnetcore identity user
        /// </summary>
        [HttpGet("{username}")]
        public async Task<ActionResult<User>> GetUser(string username)
        {
            IdentityUser user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return NotFound();
            }

            return new User
            {
                UserName = user.UserName,
                Email = user.Email
            };
        }

        /// <summary>
        /// Create JWT for specific user
        /// </summary>
        [AllowAnonymous]
        [HttpPost("BearerToken")]
        public async Task<ActionResult<AuthenticationResponse>> CreateToken(AuthenticationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Bad credentials");
            }

            var user = await _userManager.FindByNameAsync(request.UserName);

            if (user == null)
            {
                return BadRequest("Bad credentials");
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);

            if (!isPasswordValid)
            {
                return BadRequest("Bad credentials");
            }

            var token = _tokenCreationService.CreateToken(user);

            return Ok(token);
        }
    }
}
