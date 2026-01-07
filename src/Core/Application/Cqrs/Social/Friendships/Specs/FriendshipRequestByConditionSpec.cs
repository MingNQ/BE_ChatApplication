using Application.Common.Specification;
using Application.Cqrs.Social.Friendships.Params;
using Application.Dto.Social;
using Domain.Entities.Social;

namespace Application.Cqrs.Social.Friendships.Specs;

public sealed class FriendshipRequestByConditionSpec(SearchFriendshipRequestParams param) : BaseSpec<FriendshipRequest, FriendshipRequestDto>(param);