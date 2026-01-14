namespace Application.Cqrs.Feed.PostComments.Commands;

public class BaseCommentCommand
{
    public long Id { get; set; }
    public string Content { get; set; } = string.Empty;
}