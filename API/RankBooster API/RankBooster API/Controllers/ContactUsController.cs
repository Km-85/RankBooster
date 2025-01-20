using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using RankBooster_API.Models;
using MailKit.Net.Smtp;

namespace RankBooster_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactUsController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        // Constructor to inject configuration settings (like SMTP server settings)
        public ContactUsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // POST api/contact
        [HttpPost]
        public IActionResult Post([FromBody] ContactForm form)
        {
            if (form == null || string.IsNullOrEmpty(form.Name) || string.IsNullOrEmpty(form.Email) || string.IsNullOrEmpty(form.Message))
            {
                return BadRequest("Invalid form data.");
            }

            // Send the email
            try
            {
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress("RankBoosters", _configuration["EmailSettings:FromEmail"]));
                emailMessage.To.Add(new MailboxAddress("", _configuration["EmailSettings:ToEmail"]));
                emailMessage.Subject = "New Contact Us Message";

                // Create the services section from the array
                string services = string.Join(", ", form.Services);

                var bodyBuilder = new BodyBuilder
                {
                    TextBody = $"Name: {form.Name}\nEmail: {form.Email}\nPhone: {form.Phone}\n Website: {form.Website}\nServices: {services}\nMessage: {form.Message}"
                };

                emailMessage.Body = bodyBuilder.ToMessageBody();

                using (var smtpClient = new SmtpClient())
                {
                    // Connect to SMTP server
                    smtpClient.Connect(_configuration["EmailSettings:SmtpServer"], int.Parse(_configuration["EmailSettings:SmtpPort"]), false);

                    // Authenticate with SMTP server
                    smtpClient.Authenticate(_configuration["EmailSettings:FromEmail"], _configuration["EmailSettings:EmailPassword"]);

                    // Send the email
                    smtpClient.Send(emailMessage);

                    // Disconnect and quit
                    smtpClient.Disconnect(true);
                }

                return Ok(new { message = "Your message has been sent successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }
    }
}
