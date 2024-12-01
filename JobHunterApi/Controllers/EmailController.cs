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
        [HttpPost("send")]
        public async Task<IActionResult> SendEmail([FromBody] EmailModel emaildata)
        {
            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("hiteshlakshmaiahdinesh@gmail.com", "qetc iqkk bysg cenu"),
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


