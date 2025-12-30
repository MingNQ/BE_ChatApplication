using Application.Common.Events;
using Application.Common.Repositories;
using Application.Common.UnitOfWork;
using Application.Dto.Authorization.Verification;
using Application.Interfaces.Infrastructures.Integrates.External.Service.Email;
using Domain.Entities.Identity;
using Domain.Events.Verification;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Constants;

namespace Application.Cqrs.Authentication.Events;

public class VerificationCreatedEventHandler(
    IEmailService emailService,
    IUnitOfWork unitOfWork,
    ILogger<VerificationCreatedEventHandler> logger)
    : INotificationHandler<EventNotification<VerificationCreatedEvent>>
{
    private readonly IWriteRepository<User> _userRepository = unitOfWork.GetRepository<User>();

    public async Task Handle(EventNotification<VerificationCreatedEvent> notification, CancellationToken cancellationToken)
    {
        var eventData = notification.Event;

        try
        {
            if (eventData.ContactInfo.IsEmail)
            {
                await SendEmailVerification(eventData);
            }
            else
            {
                // TO-DO: Send Phone Verification
            }

            logger.LogInformation("Verification code sent for user");
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
        }
    }

    private async Task SendEmailVerification(VerificationCreatedEvent notification)
    {
        var user = await _userRepository.GetFirstOrDefaultAsync(
            predicate: x => x.Id == notification.UserId,
            disableTracking: true);

        var data = new SendVerificationByEmailInput
        {
            Locale = "en",
            Mode = ConvertToUserVerificationMode(notification.Mode),
            Code = notification.Code.Value,
            Email = notification.ContactInfo.Value,
            UserName = user?.UserName ?? "User"
        };

        await emailService.SendVerificationEmailVerify(data, "en");
    }

    private string ConvertToUserVerificationMode(VerificationMode mode)
    {
        return mode switch
        {
            VerificationMode.SignUp => UserVerificationMode.VerificationForSignUp,
            VerificationMode.SignIn => UserVerificationMode.VerificationForSignin,
            VerificationMode.ForgotPassword => UserVerificationMode.ForgotPassword,
            VerificationMode.EmailVerification => UserVerificationMode.VerifyCurrentUserEmailByLinkOnly,
            VerificationMode.PhoneVerification => UserVerificationMode.PhoneVerificationForSignUp,
            _ => UserVerificationMode.VerificationForSignUp
        };
    }
}