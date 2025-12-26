using Application.Common.Exceptions;
using Application.Common.Repositories;
using Application.Common.Responses;
using Application.Common.UnitOfWork;
using Application.Dto.Persistence.Catalog.User;
using Domain.Entities.Identity;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Constants;
using System.Text.Json.Serialization;

namespace Application.Cqrs.Users.Commands;

public class UpdateUserCommand : UserBaseCommand, IRequest<ResponseBase<UserDto>>
{
    [JsonIgnore]
    public long Id { get; private set; }

    public UpdateUserCommand SetId(long id)
    {
        Id = id;
        return this;
    }
}

public class UpdateUserCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateUserCommand, ResponseBase<UserDto>>
{
    private readonly IWriteRepository<User> _userRepository = unitOfWork.GetRepository<User>();

    public async Task<ResponseBase<UserDto>> Handle(
        UpdateUserCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetFirstOrDefaultAsync(
            predicate: c => c.Id == request.Id,
            include: x => x.Include(o => o.UserRoles),
                disableTracking: false)
            ?? throw new NotFoundException(MessageCommon.SetEntityNotFound(nameof(User), request.Id));

        user.SetEmail(request.Email);
        user.UpdateName(request.FirstName, request.LastName);

        if (!string.IsNullOrEmpty(request.PhoneNumber))
        {
            user.SetPhoneNumber(request.PhoneNumber);
        }

        if (request.AvatarId.HasValue)
        {
            user.SetAvatar(request.AvatarId.Value);
        }

        if (!string.IsNullOrEmpty(request.RegisterProvider))
        {
            user.SetRegisterProvider(request.RegisterProvider);
        }

        // Update roles using domain method
        user.UpdateRoles(request.RoleIds);

        _userRepository.Update(user);
        await unitOfWork.SaveChangesAsync();

        return new ResponseBase<UserDto>(user.Adapt<UserDto>(), MessageCommon.UpdateSuccess);
    }
}