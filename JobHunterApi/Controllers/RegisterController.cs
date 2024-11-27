using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JobHunterApi.Database;
using JobHunterApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

[Route("api/[controller]")]
[ApiController]
public class RegisterController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;

    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly CompaniesDbContext _context;
    private readonly IConfiguration _configuration;

    public RegisterController(UserManager<IdentityUser> userManager,
                            CompaniesDbContext context,
                            RoleManager<IdentityRole> roleManager,
                            IConfiguration configuration)
    {
        _userManager = userManager;
        _context= context;
        _roleManager=roleManager;
        _configuration = configuration;

    }
    [HttpGet("getpendingregistrations")]
    public async Task<IActionResult> GetPendingApplications()
    {
        var pendingregistrations = await _context.PendingRegistrations.ToListAsync(); 
        return Ok(pendingregistrations);
    }

    [HttpPost("pendingregistrations")]
    public async Task<IActionResult> GetPendingApplications([FromBody] RegisterModel user)
    {
        await _context.PendingRegistrations.AddAsync(user);
        await _context.SaveChangesAsync();

        return Ok(new {Message="user approval pending"});
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        // Check if user already exists
        var existingUser = await _userManager.FindByNameAsync(model.Username);
        if (existingUser != null)
        {
            return BadRequest(new { message = "Username is already taken" });
        }

        // Create a new user
        var user = new IdentityUser
        {
            UserName = model.Username,
            Email = model.Email
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }
        
        //added
        await _roleManager.CreateAsync(new IdentityRole("admin"));

        if (user.UserName=="Hitesh")
        {
            await _userManager.AddToRoleAsync(user, "admin");
        }

        var token =await GenerateJwtToken(user);
        return Ok(new { Message="Registered Successfully! Please login!" ,token});
    }


    private async Task<string> GenerateJwtToken(IdentityUser user)
    {

    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName ?? ""),
                new Claim(ClaimTypes.Email, user.Email ?? "")
            };
        var roles=await _userManager.GetRolesAsync(user);

        claims.AddRange(roles.Select(role=>new Claim(ClaimTypes.Role,role)));
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]??""));
        var creds= new SigningCredentials(key,SecurityAlgorithms.HmacSha256Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            SigningCredentials = creds
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);

    }

}


