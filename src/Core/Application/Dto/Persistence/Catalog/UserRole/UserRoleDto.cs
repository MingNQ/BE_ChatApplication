using Application.Dto.Authorization.Role;
using Application.Dto.Persistence.Catalog.User;

namespace Application.Dto.Persistence.Catalog.UserRole;

public class UserRoleDto
{
    public int UserId { get; set; }

    public int RoleId { get; set; }

    public UserDto User { get; set; } = new();

    public RoleDto Role { get; set; } = new();
}