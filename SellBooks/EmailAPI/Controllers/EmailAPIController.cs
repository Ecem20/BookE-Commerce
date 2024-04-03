using Azure;
using EmailAPI.Models.Dto;
using EmailAPI.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EmailAPI.Data;
using System.Net.Mail;

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
            MailAddress mailAddress = new MailAddress(request.ReceiverEmail);

            if(request.Subject=="string" || request.Subject=="" || request.Body=="string" || request.Body == "")
            {
                throw new Exception("Subject or body not entered");
            }

            _emailService.SendEmail(request.ReceiverEmail, request.Subject, request.Body);
            return Ok("Email sent successfully.");
        }
    }
}