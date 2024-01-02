using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using WebAPI.Models.DTO;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly ITokenGenerator tokenGenerator;

        public AuthController(UserManager<IdentityUser> userManager, ITokenGenerator tokenRepository)
        {
            this.userManager = userManager;
            this.tokenGenerator = tokenRepository;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            var identityUser = new IdentityUser
            {
                UserName = registerRequestDto.Username,
                Email = registerRequestDto.Username
            };

            var identityResult = await userManager.CreateAsync(identityUser, registerRequestDto.Password);

            if(identityResult.Succeeded && registerRequestDto.Roles!.Any())
            {
                identityResult = await userManager.AddToRolesAsync(identityUser, registerRequestDto.Roles);

                if(identityResult.Succeeded)
                {
                    return Ok("User was registered! Please login.");
                }
            }

            return BadRequest("Something went wrong");
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var user = await userManager.FindByEmailAsync(loginRequestDto.Username);

            if(user is not null && await userManager.CheckPasswordAsync(user, loginRequestDto.Password))
            {
                var roles = (await userManager.GetRolesAsync(user)).ToList() ?? Enumerable.Empty<string>().ToList();

                var token = tokenGenerator.CreateJWTToken(user, roles);

                var response = new LoginResponseDto
                {
                    JwtToken = token.Value,
                    ExpirationTime = token.ExpirationTime
                };

                return Ok(response);
            }

            return BadRequest("Username or password incorrect");
        }
    }
}