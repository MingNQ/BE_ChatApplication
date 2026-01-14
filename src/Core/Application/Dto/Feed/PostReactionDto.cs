using Application.Common.Interfaces;
using Domain.Common.Enums;

namespace Application.Dto.Feed;

public class PostReactionDto : IDto
{
    public long Id { get; set; }
    public long PostId { get; set; }
    public long UserId { get; set; }
    public ReactionTypeEnum Type { get; set; }
}