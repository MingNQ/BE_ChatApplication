using Application.Common.Interfaces;
using Application.Dto.Authorization.Role;
using Application.Dto.Persistence.Catalog.FileStorages;
using Application.Dto.Social;

namespace Application.Dto.Persistence.Catalog.User;

public class UserDto : SortUserInfo
{
    public DateTime? LastLogin { get; set; }
    public bool Active { get; set; }

    public bool? IsVerifiedPhone { get; set; }

    public bool? IsVerifiedEmail { get; set; }
    public string? RegisterProvider { get; set; }
    public List<UserRoleDto> UserRoles { get; set; } = new();
}

public class SortUserInfo : IDto
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;

    public string FullName => $"{FirstName} {LastName}";

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public FileStorageDto? Avatar { get; set; }
}

public class UserRoleDto
{
    public RoleDto? Role { get; set; }
}

public class UpdateUserPasswordInput
{
    public string OldPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string RePassword { get; set; } = string.Empty;
}

public class UserPresenceDto : SortUserInfo
{
    public PresenceDto? Presence { get; set; }
}