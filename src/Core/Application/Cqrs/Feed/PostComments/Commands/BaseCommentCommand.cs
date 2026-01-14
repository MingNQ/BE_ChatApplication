namespace Application.Cqrs.Feed.PostComments.Commands;

public class BaseCommentCommand
{
    public long PostId { get; set; }
    public string Content { get; set; } = string.Empty;
}