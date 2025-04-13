using AgroPlaner.DAL.Models;
using AgroPlaner.Services.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AgroPlaner.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(
                    new AuthResult
                    {
                        Success = false,
                        Errors = new List<string> { "Invalid payload" },
                    }
                );
            }

            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
            {
                return BadRequest(
                    new AuthResult
                    {
                        Success = false,
                        Errors = new List<string> { "Email already in use" },
                    }
                );
            }

            var newUser = new ApplicationUser
            {
                Email = model.Email,
                UserName = model.UserName,
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            var result = await _userManager.CreateAsync(newUser, model.Password);

            if (!result.Succeeded)
            {
                return BadRequest(
                    new AuthResult
                    {
                        Success = false,
                        Errors = result.Errors.Select(e => e.Description).ToList(),
                    }
                );
            }

            var token = await _tokenService.GenerateJwtToken(newUser);

            return Ok(new AuthResult { Success = true, Token = token });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(
                    new AuthResult
                    {
                        Success = false,
                        Errors = new List<string> { "Invalid payload" },
                    }
                );
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest(
                    new AuthResult
                    {
                        Success = false,
                        Errors = new List<string> { "Invalid email or password" },
                    }
                );
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
            {
                return BadRequest(
                    new AuthResult
                    {
                        Success = false,
                        Errors = new List<string> { "Invalid email or password" },
                    }
                );
            }

            var token = await _tokenService.GenerateJwtToken(user);

            return Ok(new AuthResult { Success = true, Token = token });
        }
    }
}
