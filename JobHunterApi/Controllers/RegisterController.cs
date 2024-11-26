using JobHunterApi.Database;
using JobHunterApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class RegisterController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;

    private readonly CompaniesDbContext _context;

    public RegisterController(UserManager<IdentityUser> userManager,
                            CompaniesDbContext context)
    {
        _userManager = userManager;
        _context= context;
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

        if (!await _userManager.IsInRoleAsync(user, "admin") && user.UserName=="Hitesh")
        {
            await _userManager.AddToRoleAsync(user, "admin");
        }

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }
        

        return Ok(new { Message="Registered Successfully! Please login!" });
    }


}


