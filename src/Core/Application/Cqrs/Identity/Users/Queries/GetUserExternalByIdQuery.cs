using Application.Common.Exceptions;
using Application.Common.Persistence;
using Application.Cqrs.Identity.Users.Specs;
using Application.Dto.Persistence.Catalog.User;
using Domain.Entities.Identity;
using MediatR;
using Shared.Constants;

namespace Application.Cqrs.Identity.Users.Queries;

public class GetUserExternalByIdQuery : IRequest<SortUserInfo>
{
    public long Id { get; set; }
}

public class GetUserExternalByIdQueryHandler(IReadRepository<User> userRepository)
    : IRequestHandler<GetUserExternalByIdQuery, SortUserInfo>
{
    public async Task<SortUserInfo> Handle(GetUserExternalByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.FirstOrDefaultAsync(new UserExternalByIdSpec(request.Id), cancellationToken)
            ?? throw new NotFoundException(MessageCommon.SetEntityNotFound(nameof(User), request.Id));

        return user;
    }
}