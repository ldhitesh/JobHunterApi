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

    private readonly CompaniesDbContext _context;
    private readonly IConfiguration _configuration;

    public RegisterController(UserManager<IdentityUser> userManager,
                            CompaniesDbContext context,
                            IConfiguration configuration)
    {
        _userManager = userManager;
        _context= context;
        _configuration = configuration;

    }
    [HttpGet("getpendingregistrations")]
    public async Task<IActionResult> GetPendingApplications()
    {
        var pendingregistrations = await _context.PendingRegistrations.
                            Select(u => new { u.Username, u.Email })  
                            .ToListAsync(); 
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
    public async Task<IActionResult> Register([FromBody] RegisterUserModel userdetails)
    {
        if (userdetails==null)
        {
            return BadRequest(new { message = "Wrong User Details" });
        }
        var user = await _context.PendingRegistrations
                                    .FirstOrDefaultAsync(c => c.Username.ToLower() == userdetails.Username.ToLower());

        if(user==null){
            BadRequest("UserName not fount");
        }
        // Check if user already exists
        var existingUser = await _userManager.FindByNameAsync(userdetails.Username);
        if (existingUser != null)
        {
            return BadRequest(new { message = "Username is already taken" });
        }

        // Create a new user
        var userdata = new IdentityUser
        {
            UserName = userdetails.Username,
            Email = userdetails.Email,
        };

        var result = await _userManager.CreateAsync(userdata, user.Password);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }
        
        if(userdata.UserName=="Hitesh"){
            var addToRoleResult = await _userManager.AddToRoleAsync(userdata, "Admin");
            if (!addToRoleResult.Succeeded)
        {
            return BadRequest(new { message = "Failed to assign role" });
        }
        }
        else{
            var addToRoleResult = await _userManager.AddToRoleAsync(userdata, "User");
            if (!addToRoleResult.Succeeded)
        {
            return BadRequest(new { message = "Failed to assign role" });
        }
        }

        _context.PendingRegistrations.Remove(user);
        await _context.SaveChangesAsync();

        var token =GenerateJwtToken(userdata);

        return Ok(new { Message="User Registeration was Successfully!" ,token});
    }


    [HttpDelete("rejectregistration/{username}")]
    public async Task<IActionResult> DeleteCompany(string username)
    {
        
        if (username==null)
        {
            return BadRequest(new { message = "UserName is required." });
        }
        var user = await _context.PendingRegistrations
                                    .FirstOrDefaultAsync(c => c.Username.ToLower() == username.ToLower());
        if (user == null)
        {
            return NotFound(new { message = "User not found." });
        }
        _context.PendingRegistrations.Remove(user);
        await _context.SaveChangesAsync();

        return Ok(new { message = "User Approval Rejeted!" });
    }


    private string GenerateJwtToken(IdentityUser user)
    {

    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName ?? ""),
                new Claim(ClaimTypes.Email, user.Email ?? "")
            };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]??""));
        var creds= new SigningCredentials(key,SecurityAlgorithms.HmacSha256Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            SigningCredentials = creds
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token =  tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);

    }

}


