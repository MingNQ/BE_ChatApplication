using Application.Common.Persistence;
using Application.Common.Responses;
using Application.Cqrs.Identity.Users.Specs;
using Application.Dto.Persistence.Catalog.User;
using Domain.Entities.Identity;
using Mapster;
using MediatR;

namespace Application.Cqrs.Identity.Users.Queries;

public class GetUserByRoleQuery : IRequest<ResponseBase<List<UserDto>>>
{
    public long RoleId { get; set; }
}

public class GetUserByRoleQueryHandler(
    IReadRepository<User> userRepository) : IRequestHandler<GetUserByRoleQuery, ResponseBase<List<UserDto>>>
{
    public async Task<ResponseBase<List<UserDto>>> Handle(GetUserByRoleQuery request, CancellationToken cancellationToken)
    {
        var spec = new UserByRoleSpec(request.RoleId);

        var users = await userRepository.ListAsync(spec, cancellationToken: cancellationToken);

        var result = users.Adapt<List<UserDto>>();

        return new ResponseBase<List<UserDto>>(result);
    }
}