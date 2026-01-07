using Application.Common.Models;
using Application.Common.Persistence;
using Application.Common.Services;
using Application.Cqrs.Identity.Users.Params;
using Application.Cqrs.Identity.Users.Specs;
using Application.Dto.Persistence.Catalog.User;
using Domain.Entities.Identity;
using MediatR;

namespace Application.Cqrs.Identity.Users.Queries;

public class GetUserExternalByConditionQuery : SearchUserParam, IRequest<PaginationResponse<SortUserInfo>>;

public class GetUserExternalByConditionQueryHandler(
    IReadRepository<User> userRepository,
    IPaginationService paginationService,
    IFilePathService filePathService)
    : IRequestHandler<GetUserExternalByConditionQuery, PaginationResponse<SortUserInfo>>
{
    public async Task<PaginationResponse<SortUserInfo>> Handle(GetUserExternalByConditionQuery request, CancellationToken cancellationToken)
    {
        var spec = new UserExternalByConditionSpec(request);
        var users = await paginationService.PaginatedListAsync(
            userRepository,
            spec,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        filePathService.BindFullPaths(users.Data);

        return users;
    }
}