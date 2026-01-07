using Application.Common.Specification;
using Application.Cqrs.Identity.Users.Params;
using Application.Dto.Persistence.Catalog.User;
using Domain.Entities.Identity;

namespace Application.Cqrs.Identity.Users.Specs;

public class UserExternalByConditionSpec(SearchUserParam param) : BaseSpec<User, SortUserInfo>(param);