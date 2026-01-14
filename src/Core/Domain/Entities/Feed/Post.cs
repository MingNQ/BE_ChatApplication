using Domain.Common.Contracts;
using Domain.Common.Enums;
using Domain.Entities.Identity;

namespace Domain.Entities.Feed;

public class Post : AuditableEntity<long>, IAggregateRoot
{
    public string Content { get; private set; } = string.Empty;
    public long AuthorId { get; private set; }
    public PostVisibilityEnum Visibility { get; private set; }
    public virtual User? Author { get; private set; }
    private readonly List<PostAttachment> _attachments = [];
    public virtual IReadOnlyCollection<PostAttachment> Attachments => _attachments.AsReadOnly();
    private readonly List<PostReaction> _reactions = [];
    public virtual IReadOnlyCollection<PostReaction> Reactions => _reactions.AsReadOnly();
    private readonly List<PostComment> _comments = [];
    public virtual IReadOnlyCollection<PostComment> Comments => _comments.AsReadOnly();

    public static Post Create(string content, long authorId, PostVisibilityEnum visibility)
    {
        return new Post
        {
            Content = content,
            AuthorId = authorId,
            Visibility = visibility
        };
    }

    public void Update(string content, PostVisibilityEnum visibility)
    {
        Content = content;
        Visibility = visibility;
    }

    public void AddAttachment(PostAttachment attachment)
    {
        _attachments.Add(attachment);
    }

    public void AddAttachments(IEnumerable<PostAttachment> attachments)
    {
        _attachments.AddRange(attachments);
    }

    public void AddReaction(PostReaction reaction)
    {
        _reactions.Add(reaction);
    }

    public void AddComment(PostComment comment)
    {
        _comments.Add(comment);
    }

    public void RemoveComment(PostComment comment)
    {
        _comments.Remove(comment);
    }

    public void RemoveReaction(PostReaction reaction)
    {
        _reactions.Remove(reaction);
    }

    public void RemoveAttachment(PostAttachment attachment)
    {
        _attachments.Remove(attachment);
    }

    public void RemoveAttachments(IEnumerable<PostAttachment> attachments)
    {
        foreach (var attachment in attachments)
        {
            _attachments.Remove(attachment);
        }
    }
}