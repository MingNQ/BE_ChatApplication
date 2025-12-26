namespace Application.Dto.Persistence.Catalog.UserRole;

public class UserRolesDtoInput
{
    public int UserId { get; set; }
    public List<int> RoleIds { get; set; } = new();
}