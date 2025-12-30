using Ardalis.Specification;
using Domain.Entities.Identity;

namespace Application.Cqrs.Identity.Users.Specs;

public sealed class UserByRoleSpec : Specification<User>
{
    public UserByRoleSpec(long roleId)
    {
        Query.Where(x => x.UserRoles.Any(ur => ur.RoleId == roleId));
    }
}