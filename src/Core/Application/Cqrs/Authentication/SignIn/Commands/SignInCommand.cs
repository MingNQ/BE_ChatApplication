using Application.Dto.Authentication;
using MediatR;

namespace Application.Cqrs.Authentication.SignIn.Commands;

public class InitiateSignInCommand : IRequest<InitiateSignInResponse>
{
    public string Contact { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; } = false;
}

public class VerifySignInOtpCommand : IRequest<VerifySignInOtpResponse>
{
    public string Contact { get; set; } = string.Empty;
    public string VerificationCode { get; set; } = string.Empty;
    public bool RememberMe { get; set; } = false;
    public string IpAddress { get; set; } = string.Empty;
}

public class ResendSignInOtpCommand : IRequest<ResendSignInOtpResponse>
{
    public string Contact { get; set; } = string.Empty;
    public long VerificationId { get; set; }
}