using Application.Dto.Authorization.Verification;
using Application.Interfaces.Infrastructures.Integrates.External.Service.Email;

namespace Infrastructure.Services.Integrates;

public class EmailService(IEmailTemplateProvider emailTemplateProvider, IExternalEnpointClient emailSender)
    : IEmailService
{
    public Task ChangePasswordSuccessfullyAsync(string emailAddress, string fullName, string language)
    {
        throw new NotImplementedException();
    }

    public Task SendVerificationCodeAsync(SendVerificationByEmailInput input, string language)
    {
        throw new NotImplementedException();
    }

    public Task SendVerificationCodeForUpdateProfileAsync(SendVerificationByEmailInput input, string language)
    {
        throw new NotImplementedException();
    }

    public async Task SendVerificationEmailVerify(SendVerificationByEmailInput input, string language)
    {
        string emailTemplate = await emailTemplateProvider.GetTemplateByNameAsync("verifyemail", language);

        if (!string.IsNullOrEmpty(input.Code))
        {
            emailTemplate = emailTemplate.Replace("{{UserName}}", input.UserName);
            emailTemplate = emailTemplate.Replace("{{OTP_CODE}}", input.Code);
            emailTemplate = emailTemplate.Replace("{{EMAIL}}", input.Email);
        }

        await ReplaceBodyAndSend(input.Email, "EMAIL_VERIFICATION", emailTemplate);
    }

    public Task SendVerificationEmailVerifyLinkOnly(SendVerificationByEmailInput input, string language)
    {
        throw new NotImplementedException();
    }

    public Task SendVerificationPasswordReset(SendVerificationByEmailInput input, string language)
    {
        throw new NotImplementedException();
    }

    private async Task ReplaceBodyAndSend(string emailAddress, string subject, string emailTemplate)
    {
        await emailSender.SendEmailNotification(emailAddress, subject, emailTemplate);
        await Task.CompletedTask;
    }
}