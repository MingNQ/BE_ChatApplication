using Application.Common.Specification;
using Application.Cqrs.Feed.Posts.Params;
using Application.Dto.Feed;
using Domain.Entities.Feed;

namespace Application.Cqrs.Feed.Posts.Specs;

public class SearchPostSpec(SearchPostParam param) : BaseSpec<Post, PostDto>(param);