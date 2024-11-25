using System.Net;
using System.Net.Mail;
using System.Text;
using JobHunterApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Expressions;

namespace JobHunterApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {

        [HttpPost("send")]
        public IActionResult SendEmail([FromBody] EmailModel emaildata)
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

                mailMessage.To.Add(emaildata.To);
                smtpClient.Send(mailMessage);

                return Ok(new { message = "Email sent successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error sending email", error = ex.Message });
            }
        }

    }


}


