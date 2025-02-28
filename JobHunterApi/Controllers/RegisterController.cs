using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
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
        var pendingregistrations = await _context.PendingRegistrations
                    .Where(u => u.AccountVerificationStatus == "Verified")  
                    .Select(u => new { u.Username, u.Email })
                    .ToListAsync();
        return Ok(pendingregistrations);
    }


    [HttpPost("pendingregistrations")]
    public async Task<IActionResult> GetPendingApplications([FromBody] RequestBody requestbody)
    {
            var userdata=requestbody.RegisterData;
            var emaildata=requestbody.EmailVerificationData;

            var userEmailExist = await _context.PendingRegistrations
                                    .FirstOrDefaultAsync(c => c.Email.ToLower() == userdata!.Email.ToLower());

            if(userEmailExist!=null){
                return BadRequest(new { message = "Duplicate Email!"});
            }

            if(userdata.Email=="ld.hitesh00@gmail.com")
            {                
                var admindata = new IdentityUser
                {
                    UserName = userdata.Username,
                    Email = userdata.Email,
                };

                var admincreatedresult = await _userManager.CreateAsync(admindata, userdata!.Password);
                if (!admincreatedresult.Succeeded)
                {
                    return BadRequest(admincreatedresult.Errors);
                }
                var addToRoleResult = await _userManager.AddToRoleAsync(admindata, "Admin");
                if (!addToRoleResult.Succeeded)
                {
                    return BadRequest(new { message = "Failed to assign role" });
                }
                
                return Ok(new { message = "Success" });
 
            }


            userdata!.VerificationToken = Guid.NewGuid().ToString();
            await _context.PendingRegistrations.AddAsync(userdata);
            await _context.SaveChangesAsync();

         
            string verificationUrl =$"{requestbody.verificationUrl}&data={userdata.VerificationToken}&email={userdata.Email}";
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            var greetingName=textInfo.ToTitleCase(userdata.Username.ToLower());
            string emailBody = $@"
                                <html>
                                    <head>
                                        <style>
                                            @import url('https://fonts.googleapis.com/css2?family=Roboto:wght@400;500;700&display=swap');
                                            /* General reset for margin and padding */
                                            body, html {{
                                                margin: 0;
                                                padding: 0;
                                                width: 100%;
                                                height: 100%;
                                            }}

                                            body {{
                                                font-family: 'Roboto', Arial, sans-serif;
                                                font-size: 16px;
                                                line-height: 1.5;
                                                background-color: #f4f4f4; /* Light background color */
                                                padding: 20px;
                                                color: #333;
                                            }}

                                            .email-container {{
                                                max-width: 600px;
                                                margin: 0 auto; /* Center the email content */
                                                background-color: #ffffff; /* White background for email body */
                                                padding: 20px;
                                                border-radius: 8px;
                                                box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1); /* Soft shadow for better appearance */
                                            }}

                                            h1 {{
                                                color: #333;
                                                font-size: 24px;
                                            }}

                                            p {{
                                                color: #666;
                                                font-size: 16px;
                                                line-height: 1.5;
                                            }}

                                            a {{
                                                color: #1a73e8;
                                                text-decoration: none;
                                                font-weight: bold;
                                            }}

                                            .formatted-body {{
                                                font-family: 'Roboto', Arial, sans-serif;
                                                white-space: pre-wrap;
                                                word-wrap: break-word;
                                            }}
                                        </style>
                                    </head>
                                    <body>
                                        <div class='email-container'>
                                            <div class='formatted-body'>
                                                <h1>Dear {greetingName},</h1>
                                                <p>Thank you for registering with JobHunter! To complete the sign-up process and activate your account, please verify your email address by clicking the link below:</p>
                                                <p><a href='{verificationUrl}'>Verify My Email Address</a></p>
                                                <p> If you did not create an account with JobHunter, please ignore this email.Your email address will not be used for any further communication.</p>
                                                <p>For any questions or issues, feel free to contact our support team at <a href='mailto:hiteshlakshmaiahdinesh@gmail.com'>hiteshlakshmaiahdinesh@gmail.com</a>.</p>
                                                <p>Thank you for choosing JobHunter!</p>
                                                <p>Best regards,<br>The JobHunter Team</p>
                                            </div>
                                        </div>
                                    </body>
                                </html>";


            var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("hiteshlakshmaiahdinesh@gmail.com", "zyzi pxzn bgvt wdri"),
                    EnableSsl = true,
                };

            var mailMessage = new MailMessage
                {
                    From = new MailAddress("hiteshlakshmaiahdinesh@gmail.com"),
                    Subject = emaildata!.Subject,
                    Body = emailBody, 
                    IsBodyHtml = true,
                };
            mailMessage.To.Add(emaildata.To);
            Console.WriteLine(emaildata.To);
            smtpClient.Send(mailMessage);

            return Ok(new { message = "Success" });

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
            BadRequest("UserName not found");
        }

        var EmailVerificationStatus = await _context.PendingRegistrations
                                    .FirstOrDefaultAsync(c => c.Email == user!.Email);
            
        if(EmailVerificationStatus!.AccountVerificationStatus!="Verified"){
            return BadRequest(new { message = "User Email is not verified!" });
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

        var result = await _userManager.CreateAsync(userdata, user!.Password);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }
         
        var addToRoleResult = await _userManager.AddToRoleAsync(userdata, "User");
        if (!addToRoleResult.Succeeded)
        {
            return BadRequest(new { message = "Failed to assign role" });
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


