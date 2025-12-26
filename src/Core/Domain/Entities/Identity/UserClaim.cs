using Domain.Common.Contracts;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Identity;

public class UserClaim : AuditableEntity<long>, IAggregateRoot
{
    public long UserId { get; set; }

    [MaxLength(255)]
    public string ClaimType { get; set; } = string.Empty;

    [MaxLength(255)]
    public string ClaimValue { get; set; } = string.Empty;

    public virtual User? User { get; set; }
}