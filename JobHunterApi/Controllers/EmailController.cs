using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Text;
using JobHunterApi.Database;
using JobHunterApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Expressions;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

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


        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail( string redirectto,string data, string email)
        {
        
        
        var Email = await _context.PendingRegistrations
                                    .FirstOrDefaultAsync(c => c.Email == email);
        if (Email!=null && Email.VerificationToken==data)
            {
                Email.AccountVerificationStatus = "Verified";  
                await _context.SaveChangesAsync();
                return Redirect(redirectto);
            }
            return BadRequest("Email Couldnt be Verified!");
        }

        [HttpGet("oauth2callback")]
        public async Task<IActionResult> Callbackfunctn()
        {
            return Ok("success!");
        }
        [HttpPost("sendgmailapi")]
        public async Task<IActionResult> SendEmailAsync(EmailRequest emailRequest)
        {
            try
            {

                // Step 1: Authenticate the user and get the Gmail service instance
                var emailService = await GoogleOAuthHelper.AuthenticateAsync();
                
               

                // Step 2: Send an email
                if (string.IsNullOrEmpty(emailRequest.FromEmail) || string.IsNullOrEmpty(emailRequest.ToEmail))
                {
                    return BadRequest("FromEmail and ToEmail cannot be null or empty.");
                }

                // Send the email
                await GoogleOAuthHelper.SendEmailAsync(emailService, emailRequest.FromEmail, emailRequest.ToEmail, emailRequest.Subject, emailRequest.Body);
                    
                
                return Ok(new { message = "Email sent successfully!" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        [HttpPost("send")]
        public async Task<IActionResult> SendEmail([FromBody] EmailModel emaildata)
        {
            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("jobhuntertechteam@gmail.com", "vskw iwdj tskn mznt"),
                    EnableSsl = true,
                };
                
                
                var mailMessage = new MailMessage
                {
                    From = new MailAddress("jobhuntertechteam@gmail.com"),
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


