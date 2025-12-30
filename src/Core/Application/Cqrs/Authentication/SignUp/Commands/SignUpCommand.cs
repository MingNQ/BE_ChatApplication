using Application.Dto.Authentication;
using MediatR;
using System.Text.Json.Serialization;

namespace Application.Cqrs.Authentication.SignUp.Commands;

public class InitiateSignUpCommand : IRequest<InitiateSignUpResponse>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Contact { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class VerifySignUpContactCommand : IRequest<VerifySignUpContactResponse>
{
    public string Contact { get; set; } = string.Empty;
    public string VerificationCode { get; set; } = string.Empty;

    [JsonIgnore]
    public string? IpAddress { get; set; }
}

public class ResendSignUpOtpCommand : IRequest<ResendSignUpOtpResponse>
{
    public string Contact { get; set; } = string.Empty;
    public long VerificationId { get; set; }
}