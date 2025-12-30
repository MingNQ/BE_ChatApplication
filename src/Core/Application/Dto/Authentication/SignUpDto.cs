namespace Application.Dto.Authentication;

public class InitiateSignUpRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Contact { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class InitiateSignUpResponse
{
    public long VerificationId { get; set; }
    public bool IsContactExists { get; set; }
    public string Contact { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public UserProfileDto? User { get; set; }
}

public class VerifySignUpContactRequest
{
    public string Contact { get; set; } = string.Empty;
    public string VerificationCode { get; set; } = string.Empty;
}

public class VerifySignUpContactResponse
{
    public long UserId { get; set; }
    public bool IsVerified { get; set; }
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public UserProfileDto? User { get; set; }
}

public class ResendSignUpOtpRequest
{
    public string Contact { get; set; } = string.Empty;
    public long VerificationId { get; set; }
}

public class ResendSignUpOtpResponse
{
    public bool IsSent { get; set; }
    public long VerificationId { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class UserProfileDto
{
    public long Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsEmailVerified { get; set; }
    public bool IsPhoneVerified { get; set; }
}