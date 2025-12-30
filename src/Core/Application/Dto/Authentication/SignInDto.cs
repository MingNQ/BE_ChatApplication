namespace Application.Dto.Authentication;

public class InitiateSignInRequest
{
    public string Contact { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; } = false;
}

public class InitiateSignInResponse
{
    public long VerificationId { get; set; }
    public string Contact { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public UserProfileDto? User { get; set; }
}

public class VerifySignInOtpRequest
{
    public string Contact { get; set; } = string.Empty;
    public string VerificationCode { get; set; } = string.Empty;
    public bool RememberMe { get; set; } = false;
}

public class VerifySignInOtpResponse
{
    public bool IsAuthenticated { get; set; }
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; set; }
    public string Message { get; set; } = string.Empty;
    public UserProfileDto? User { get; set; }
}

public class ResendSignInOtpRequest
{
    public string Contact { get; set; } = string.Empty;
    public long VerificationId { get; set; }
}

public class ResendSignInOtpResponse
{
    public bool IsSent { get; set; }
    public long VerificationId { get; set; }
    public string Message { get; set; } = string.Empty;
}