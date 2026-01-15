using Domain.Common.Enums;

namespace Application.Cqrs.Feed.PostReactions.Commands;

public class BaseReactionCommand
{
    public long PostId { get; set; }
    public ReactionTypeEnum Type { get; set; }
}