using System.Security.Claims;

namespace Application.Common.Interfaces;

public interface ICurrentUser
{
    string? Name { get; }

    long GetUserId();

    string? GetUserEmail();

    bool IsAuthenticated();

    bool IsInRole(string role);

    IEnumerable<Claim>? GetUserClaims();

    long UserId { get; }
    string Email { get; }
    string Avatar { get; }
}