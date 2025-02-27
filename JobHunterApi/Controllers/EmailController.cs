using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Text;
using JobHunterApi.Database;
using JobHunterApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Expressions;

namespace JobHunterApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly CompaniesDbContext _context;

        public EmailController(CompaniesDbContext context){
            _context = context;
        }

        // [HttpPost("sendverificationmail")]
        // public async Task<IActionResult> VerificationEmail([FromBody] EmailModel model)
        // {
        //     // Assuming user is successfully created
            
        // }


        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail( string data, string email)
        {
        
        
        var Email = await _context.PendingRegistrations
                                    .FirstOrDefaultAsync(c => c.Email == email);
        if (Email!=null && Email.VerificationToken==data)
            {
                Email.AccountVerificationStatus = "Verified";  
                await _context.SaveChangesAsync();
                return Redirect("http://localhost:4200/email-verification-complete");
            }
            return BadRequest("Email Couldnt be Verified!");
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendEmail([FromBody] EmailModel emaildata)
        {
            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("hiteshlakshmaiahdinesh@gmail.com", "zyzi pxzn bgvt wdri"),
                    EnableSsl = true,
                };
                
                




                
                var mailMessage = new MailMessage
                {
                    From = new MailAddress("hiteshlakshmaiahdinesh@gmail.com"),
                    Subject = emaildata.Subject ?? "No Subject",
                    Body = emaildata.Body, //.Replace("\n", "<pre>") ?? "No Body",
                    IsBodyHtml = true,
                };

                if(emaildata.To=="Recruiter")
                {
                     var Emails =  await _context.CompanyReferences
                                            .Where(u => u.Position == "Recruiter")
                                            .Select(u => u.Email).ToListAsync();
                    
                    foreach(var email in Emails){
                            mailMessage.To.Add(email);
                            smtpClient.Send(mailMessage);
                    }
                    return Ok(new { message = "All Emails sent successfully!" });
                }
                else if(emaildata.To=="Employees"){
                    var Emails =  await _context.CompanyReferences
                                            .Where(u => u.Position != "Recruiter")
                                            .Select(u => u.Email).ToListAsync();
                    foreach(var email in Emails){
                            mailMessage.To.Add(email);
                            smtpClient.Send(mailMessage);
                    }
                    return Ok(new { message = "All Emails sent successfully!" });
                }
                else{
                    mailMessage.To.Add(emaildata.To);
                    smtpClient.Send(mailMessage);
                }
                    
                return Ok(new { message = "Email sent successfully!" });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error sending email", error = ex.Message });
            }
        }

    }


}


