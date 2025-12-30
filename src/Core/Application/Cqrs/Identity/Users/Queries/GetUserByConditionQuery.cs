using Application.Common.Models;
using Application.Common.Persistence;
using Application.Common.Responses;
using Application.Common.Services;
using Application.Cqrs.Identity.Users.Params;
using Application.Cqrs.Identity.Users.Specs;
using Application.Dto.Persistence.Catalog.User;
using Domain.Entities.Identity;
using MediatR;

namespace Application.Cqrs.Identity.Users.Queries;

public class GetUserByConditionQuery : SearchUserParam,
    IRequest<ResponseBase<PaginationResponse<UserDto>>>;

public class GetUserByConditionQueryHandler(
    IReadRepository<User> userRepository,
    IPaginationService paginationService,
    IFilePathService filePathService)
    : IRequestHandler<GetUserByConditionQuery, ResponseBase<PaginationResponse<UserDto>>>
{
    public async Task<ResponseBase<PaginationResponse<UserDto>>> Handle(
        GetUserByConditionQuery request, CancellationToken cancellationToken)
    {
        var spec = new UserByConditionSpec(request);
        var users = await paginationService.PaginatedListAsync(
            userRepository,
            spec,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        filePathService.BindFullPaths(users.Data);

        return new ResponseBase<PaginationResponse<UserDto>>(users);
    }
}