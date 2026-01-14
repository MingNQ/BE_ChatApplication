using Application.Common.Exceptions;
using Application.Common.Repositories;
using Application.Common.UnitOfWork;
using Domain.Entities.Feed;
using MediatR;
using Shared.Constants;
using System.Text.Json.Serialization;

namespace Application.Cqrs.Feed.Posts.Commands;

public class DeletePostCommand : IRequest<long>
{
    [JsonIgnore]
    public long Id { get; private set; }

    public void SetId(long id) => Id = id;
}

public class DeletePostCommandHandler(
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeletePostCommand, long>
{
    private readonly IWriteRepository<Post> _postRepository = unitOfWork.GetRepository<Post>();

    public async Task<long> Handle(DeletePostCommand request, CancellationToken cancellationToken)
    {
        var post = await _postRepository.GetFirstOrDefaultAsync(
            predicate: p => p.Id == request.Id,
            disableTracking: false)
            ?? throw new NotFoundException(MessageCommon.SetEntityNotFound(nameof(Post), request.Id));

        _postRepository.Delete(post);
        await unitOfWork.SaveChangesAsync();

        return post.Id;
    }
}