namespace Application.Cqrs.Users.Commands;

public class UserBaseCommand
{
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public long? AvatarId { get; set; }
    public bool LockoutEnabled { get; set; } = true;
    public string? RegisterProvider { get; set; }
    public List<long> RoleIds { get; set; } = new();
}