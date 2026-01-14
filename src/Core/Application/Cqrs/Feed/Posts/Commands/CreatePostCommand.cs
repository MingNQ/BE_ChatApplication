using Application.Common.Interfaces;
using Application.Common.Repositories;
using Application.Common.UnitOfWork;
using Application.Dto.Feed;
using Domain.Common.Enums;
using Domain.Entities.Feed;
using Mapster;
using MediatR;

namespace Application.Cqrs.Feed.Posts.Commands;

public class CreatePostCommand : IRequest<PostDto>
{
    public string Content { get; set; } = string.Empty;
    public PostVisibilityEnum Visibility { get; set; }
    public List<long> AttachmentIds { get; set; } = [];
}

public class CreatePostCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser)
    : IRequestHandler<CreatePostCommand, PostDto>
{
    private readonly IWriteRepository<Post> _postRepository = unitOfWork.GetRepository<Post>();

    public async Task<PostDto> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        var post = Post.Create(request.Content, currentUser.UserId, request.Visibility);

        await _postRepository.InsertAsync(post, cancellationToken);
        await unitOfWork.SaveChangesAsync();

        var postAttachments = request.AttachmentIds
            .Select(attachmentId => PostAttachment.Create(post.Id, attachmentId))
            .ToList();

        post.AddAttachments(postAttachments);

        await unitOfWork.SaveChangesAsync();

        return post.Adapt<PostDto>();
    }
}