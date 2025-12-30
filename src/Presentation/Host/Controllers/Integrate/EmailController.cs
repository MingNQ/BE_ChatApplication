using Host.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;
using TravelBlogs.Core.Application.Dto.Integrates.Notifications;

namespace Host.Controllers.Integrate;

[ControllerName("email")]
[Tags("Integrate|Email")]
public class EmailController : BaseNoAuthController
{
    private readonly IConfiguration _config;

    public EmailController(IConfiguration config)
    {
        _config = config;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendEmailAsync(EmailNotificationRequest request)
    {
        if (string.IsNullOrEmpty(request.ToAddress) ||
                string.IsNullOrEmpty(request.Subject) ||
                string.IsNullOrEmpty(request.Body))
        {
            return BadRequest("Missing email fields.");
        }

        try
        {
            var smtpHost = _config["Smtp:Host"];
            var smtpPort = int.Parse(_config["Smtp:Port"]);
            var smtpUser = _config["Smtp:Username"];
            var smtpPass = _config["Smtp:Password"];
            var enableSsl = bool.Parse(_config["Smtp:EnableSsl"]);

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = enableSsl
            };

            var mail = new MailMessage
            {
                From = new MailAddress(smtpUser),
                Subject = request.Subject,
                Body = request.Body,
                IsBodyHtml = true
            };
            mail.To.Add(request.ToAddress);

            await client.SendMailAsync(mail);

            return Ok("Email sent successfully");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to send email: {ex.Message}");
        }
    }
}