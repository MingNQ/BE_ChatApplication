using Application.Common.Exceptions;
using Application.Common.Persistence;
using Application.Common.Responses;
using Application.Common.Services;
using Application.Cqrs.Identity.Users.Specs;
using Application.Dto.Persistence.Catalog.User;
using Domain.Entities.Identity;
using MediatR;
using Shared.Constants;

namespace Application.Cqrs.Identity.Users.Queries;

public class GetUserByIdQuery : IRequest<ResponseBase<UserDto>>
{
    public long Id { get; set; }
}

public class GetUserByIdQueryHandler(
    IReadRepository<User> userRepository,
    IFilePathService filePathService)
    : IRequestHandler<GetUserByIdQuery, ResponseBase<UserDto>>
{
    public async Task<ResponseBase<UserDto>> Handle(
        GetUserByIdQuery request,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.FirstOrDefaultAsync(new UserByIdSpec(request.Id), cancellationToken)
                   ?? throw new NotFoundException(MessageCommon.SetEntityNotFound(nameof(User), request.Id));

        filePathService.BindFullPaths(user.Avatar);

        return new ResponseBase<UserDto>(user);
    }
}