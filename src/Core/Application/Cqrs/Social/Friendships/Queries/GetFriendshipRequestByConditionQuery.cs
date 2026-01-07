using Application.Common.Models;
using Application.Common.Persistence;
using Application.Common.Services;
using Application.Cqrs.Social.Friendships.Params;
using Application.Cqrs.Social.Friendships.Specs;
using Application.Dto.Social;
using Domain.Entities.Social;
using MediatR;

namespace Application.Cqrs.Social.Friendships.Queries;

public class GetFriendshipRequestByConditionQuery : SearchFriendshipRequestParams, IRequest<PaginationResponse<FriendshipRequestDto>>;

public class GetFriendshipRequestByConditionQueryHandler(
    IReadRepository<FriendshipRequest> friendshipRequestRepository,
    IPaginationService paginationService)
    : IRequestHandler<GetFriendshipRequestByConditionQuery, PaginationResponse<FriendshipRequestDto>>
{
    public async Task<PaginationResponse<FriendshipRequestDto>> Handle(GetFriendshipRequestByConditionQuery request, CancellationToken cancellationToken)
    {
        var spec = new FriendshipRequestByConditionSpec(request);
        var result = await paginationService.PaginatedListAsync(
            friendshipRequestRepository,
            spec,
            request.PageNumber,
            request.PageSize);

        return result;
    }
}