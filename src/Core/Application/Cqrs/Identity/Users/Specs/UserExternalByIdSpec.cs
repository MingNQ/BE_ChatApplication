using Application.Dto.Persistence.Catalog.User;
using Ardalis.Specification;
using Domain.Entities.Identity;

namespace Application.Cqrs.Identity.Users.Specs;

public class UserExternalByIdSpec : Specification<User, SortUserInfo>
{
    public UserExternalByIdSpec(long id)
    {
        Query.Where(x => x.Id == id);

        Query.Include(x => x.UserRoles).ThenInclude(x => x.Role);
    }
}