using Refit;
using TravelBlogs.Core.Application.Dto.Integrates.Notifications;

namespace External.Service.ExternalEndpointClients.RefitInterfaces;

public interface INotificationServiceClient
{
    [Post("/email/send")]
    Task<string> SendEmail(EmailNotificationRequest request);

    [Post("/sms/send")]
    Task<string> SendSms(SmsNotificationRequest request);
}