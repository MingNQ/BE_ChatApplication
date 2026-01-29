using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Repositories;
using Application.Common.Services;
using Application.Common.UnitOfWork;
using Application.Dto.Feed;
using Domain.Entities.Feed;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Constants;

namespace Application.Cqrs.Feed.PostReactions.Commands;

public class CreateReactionCommand : BaseReactionCommand, IRequest<PostDto>;

public class CreateReactionCommandHandler(
    IUnitOfWork unitOfWork,
    IFilePathService filePathService,
    ICurrentUser currentUser)
    : IRequestHandler<CreateReactionCommand, PostDto>
{
    private readonly IWriteRepository<Post> _postRepository = unitOfWork.GetRepository<Post>();

    public async Task<PostDto> Handle(CreateReactionCommand request, CancellationToken cancellationToken)
    {
        var post = await _postRepository.GetFirstOrDefaultAsync(
            predicate: x => x.Id == request.PostId,
            include: x => x.Include(p => p.Comments)
                        .ThenInclude(c => c.User)
                    .Include(p => p.Reactions)
                    .Include(p => p.Author!)
                    .Include(p => p.Attachments)
                        .ThenInclude(a => a.Attachment!),
            disableTracking: false)
            ?? throw new NotFoundException(MessageCommon.SetEntityNotFound(nameof(Post), request.PostId));
        var postReaction = PostReaction.Create(post.Id, currentUser.UserId, request.Type);

        post.AddReaction(postReaction);

        _postRepository.Update(post);
        await unitOfWork.SaveChangesAsync();

        var postDto = post.Adapt<PostDto>();

        if (postDto.Attachments.Count > 0)
        {
            var attachments = postDto.Attachments.Select(a => a.Attachment!).ToList();
            filePathService.BindFullPaths(attachments);
        }
        return postDto;
    }
}