using JobHunterApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JobHunterApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public LoginController(UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] User model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return Unauthorized(new { message = "Invalid username or password." });
            }
            var role = await _userManager.GetRolesAsync(user);

            var token = GenerateJwtToken(user,role);
            return Ok(new
            {
                Token = token,
                UserDetails = new
                {
                    Username = user.UserName,
                    Role=role,
                }
            });
        }


        private string GenerateJwtToken(IdentityUser user, IList<string> role)
        {
            // Create claims for the user
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName ?? ""),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim(ClaimTypes.Role, role.FirstOrDefault() ?? "") // Ensure there's at least one role
            };

            // Get the JWT key from the configuration
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is missing in the configuration")));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            // Create the token descriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),               // Add claims to the identity
                Expires = DateTime.UtcNow.AddMinutes(30),           // Set expiration to 30 minutes
                Issuer = _configuration["Jwt:Issuer"],              // Set the issuer
                Audience = _configuration["Jwt:Audience"],          // Set the audience
                SigningCredentials = creds                          // Set signing credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor); // Create the token

            // Return the JWT token as a string
            return tokenHandler.WriteToken(token); 
        }

    }
}