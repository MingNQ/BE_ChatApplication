using Application.Dto.Integrate.Emails;
using Application.Interfaces.Infrastructures.Integrates.External.Service.Email;
using External.Service.ExternalEndpointClients.RefitInterfaces;
using Microsoft.Extensions.Logging;

namespace External.Service.ExternalEndpointClients;

public class ExternalEnpointClient(
    HttpClient httpClient,
    ILogger<ExternalEnpointClient> logger,
    IEmailServiceClient emailServiceClient,
    INotificationServiceClient notificationServiceClient)
    : IExternalEnpointClient
{
    public async Task SendEmailNotification(string toAddress, string subject, string body)
    {
        try
        {
            await notificationServiceClient.SendEmail(new()
            {
                ToAddress = toAddress,
                Subject = subject,
                Body = body
            });
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to send email notification to {ToAddress}", toAddress);
        }
    }

    public async Task<EmailResponseDto> SendEmails(EmailRequestDto request)
    {
        return await emailServiceClient.SendEmails(request);
    }

    public async Task SendSmsNotification(string phoneNumber, string message)
    {
        await notificationServiceClient.SendSms(new()
        {
            PhoneNumber = phoneNumber,
            Message = message
        });
    }
}