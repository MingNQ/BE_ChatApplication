using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Persistence;
using Application.Cqrs.Identity.Users.Specs;
using Application.Dto.Persistence.Catalog.User;
using Domain.Entities.Identity;
using Mapster;
using MediatR;
using Shared.Constants;

namespace Application.Cqrs.Identity.Users.Queries;

public class GetCurrentUserQuery : IRequest<SortUserInfo>;

public class GetCurrentUserQueryHandler(
    IReadRepository<User> userRepository,
    ICurrentUser currentUser)
    : IRequestHandler<GetCurrentUserQuery, SortUserInfo>
{
    public async Task<SortUserInfo> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var spec = new UserByIdSpec(currentUser.UserId);
        var user = await userRepository.FirstOrDefaultAsync(spec, cancellationToken);

        if (user == null)
        {
            throw new NotFoundException(MessageCommon.SetEntityNotFound(nameof(User), currentUser.UserId));
        }

        return user.Adapt<SortUserInfo>();
    }
}