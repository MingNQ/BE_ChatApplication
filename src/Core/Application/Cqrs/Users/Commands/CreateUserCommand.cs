using Application.Common.Repositories;
using Application.Common.Responses;
using Application.Common.UnitOfWork;
using Application.Dto.Persistence.Catalog.User;
using Application.Utility;
using Domain.Entities.Identity;
using Mapster;
using MediatR;
using Shared.Constants;

namespace Application.Cqrs.Users.Commands;

public class CreateUserCommand : UserBaseCommand, IRequest<ResponseBase<UserDto>>
{
    public string Password { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
}

public class CreateUserCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateUserCommand, ResponseBase<UserDto>>
{
    private readonly IWriteRepository<User> _userRepository = unitOfWork.GetRepository<User>();

    public async Task<ResponseBase<UserDto>> Handle(
        CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        var user = new User(
            request.UserName,
            request.Email,
            request.FirstName,
            request.LastName
        );

        SetPassword(user, request.Password);

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

        // Add roles using domain method
        if (request.RoleIds.Any())
        {
            user.AddRoles(request.RoleIds);
        }

        var newUser = await _userRepository
            .InsertAsync(user, cancellationToken);

        await unitOfWork.SaveChangesAsync();

        return new ResponseBase<UserDto>(newUser.Adapt<UserDto>(), MessageCommon.CreateSuccess);
    }

    private void SetPassword(User user, string password)
    {
        string hashPassword = Utils.ComputeHash(password);
        user.SetPassword(hashPassword);
    }
}