using Application.Common.Specification;
using Application.Cqrs.Identity.Users.Params;
using Application.Dto.Persistence.Catalog.User;
using Ardalis.Specification;
using Domain.Entities.Identity;

namespace Application.Cqrs.Identity.Users.Specs;

public sealed class UserByConditionSpec : BaseSpec<User, UserDto>
{
    public UserByConditionSpec(SearchUserParam param) : base(param)
    {
        Query.Include(x => x.UserRoles).ThenInclude(x => x.Role);
    }
}

public sealed class UserByIdSpec : Specification<User, UserDto>
{
    public UserByIdSpec(long id)
    {
        Query.Where(x => x.Id == id);

        Query.Include(x => x.UserRoles).ThenInclude(x => x.Role);
    }
}