using Domain.Common.Contracts;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Identity;

public class RefreshToken : BaseEntity<long>, IAggregateRoot
{
    public long UserId { get; protected set; }
    public int Type { get; protected set; }

    [MaxLength(256)]
    public string Token { get; set; } = string.Empty;

    public DateTimeOffset ExpiredDate { get; set; }

    public static RefreshToken Create(long userId, string refreshToken, DateTimeOffset expiredDate)
    {
        return new RefreshToken
        {
            UserId = userId,
            Token = refreshToken,
            ExpiredDate = expiredDate
        };
    }
}