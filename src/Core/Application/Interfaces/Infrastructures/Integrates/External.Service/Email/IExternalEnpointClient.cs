using Application.Dto.Integrate.Emails;

namespace Application.Interfaces.Infrastructures.Integrates.External.Service.Email;

public interface IExternalEnpointClient
{
    Task<EmailResponseDto> SendEmails(EmailRequestDto request);

    Task SendSmsNotification(string phoneNumber, string message);

    Task SendEmailNotification(string toAddress, string subject, string body);
}