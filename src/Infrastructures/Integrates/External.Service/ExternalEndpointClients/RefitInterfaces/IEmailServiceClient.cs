using Application.Dto.Integrate.Emails;
using Refit;

namespace External.Service.ExternalEndpointClients.RefitInterfaces;

public interface IEmailServiceClient
{
    [Post("/Mails/CreateMail")]
    Task<EmailResponseDto> SendEmails(EmailRequestDto request);
}