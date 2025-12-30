using Application.Cqrs.Authentication.SignIn.Commands;
using Application.Cqrs.Authentication.SignUp.Commands;
using Application.Dto.Authentication;
using Host.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace Host.Controllers.Client;

[ControllerName("auth")]
[Tags("Client|Authentication")]
public class AuthenticationController : BaseNoAuthController
{
    #region Sign In

    [HttpPost("sign-in/initiate")]
    public async Task<IActionResult> InitiateSignIn(InitiateSignInRequest request)
    {
        var command = new InitiateSignInCommand
        {
            Contact = request.Contact,
            Password = request.Password,
            RememberMe = request.RememberMe,
        };
        var result = await Mediator.Send(command);
        return Ok(result, result.Message);
    }

    [HttpPost("sign-in/verify-otp")]
    public async Task<IActionResult> VerifySignInOtp(VerifySignInOtpRequest request)
    {
        var command = new VerifySignInOtpCommand
        {
            Contact = request.Contact,
            VerificationCode = request.VerificationCode,
            RememberMe = request.RememberMe,
            IpAddress = GetIpAddress() ?? string.Empty,
        };
        var result = await Mediator.Send(command);
        return Ok(result, result.Message);
    }

    [HttpPost("sign-in/resend-otp")]
    public async Task<IActionResult> ResendSignInOtp(ResendSignInOtpRequest request)
    {
        var command = new ResendSignInOtpCommand
        {
            Contact = request.Contact,
            VerificationId = request.VerificationId,
        };
        var result = await Mediator.Send(command);
        return Ok(result, result.Message);
    }

    #endregion Sign In

    #region Sign Up

    [HttpPost("sign-up/initiate")]
    public async Task<IActionResult> InitiateSignUp(InitiateSignUpRequest request)
    {
        var command = new InitiateSignUpCommand
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Contact = request.Contact,
            Password = request.Password,
            ConfirmPassword = request.ConfirmPassword,
        };
        var result = await Mediator.Send(command);
        return Ok(result, result.Message);
    }

    [HttpPost("sign-up/verify-contact")]
    public async Task<IActionResult> VerifySignUpContact(VerifySignUpContactRequest request)
    {
        var command = new VerifySignUpContactCommand
        {
            Contact = request.Contact,
            VerificationCode = request.VerificationCode,
            IpAddress = GetIpAddress(),
        };
        var result = await Mediator.Send(command);
        return Ok(result, result.Message);
    }

    [HttpPost("sign-up/resend-otp")]
    public async Task<IActionResult> ResendSignUpOtp(ResendSignUpOtpRequest request)
    {
        var command = new ResendSignUpOtpCommand
        {
            Contact = request.Contact,
            VerificationId = request.VerificationId,
        };
        var result = await Mediator.Send(command);
        return Ok(result, result.Message);
    }

    #endregion Sign Up

    private string? GetIpAddress() =>
        Request.Headers.ContainsKey("X-Forwarded-For")
            ? Request.Headers["X-Forwarded-For"]
            : HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "N/A";
}