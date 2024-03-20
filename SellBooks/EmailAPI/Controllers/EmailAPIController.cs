using Azure;
using EmailAPI.Models.Dto;
using EmailAPI.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EmailAPI.Data;

namespace EmailAPI.Controllers
{
    [Route("api/email")]
    [ApiController]
    public class EmailAPIController : ControllerBase
    {
        private readonly EmailService _emailService;

        public EmailAPIController(ApplicationDbContext dbContext)
        {
            _emailService = new EmailService("localhost", "email_queue1", dbContext);
        }

        [HttpPost("send")]
        public IActionResult SendEmail([FromBody] EmailDto request)
        {
            try
            {
                _emailService.SendEmail(request.ReceiverEmail, request.Subject, request.Body);
                return Ok("Email sent successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Failed to send email: {ex.Message}");
            }
        }

    }
}
