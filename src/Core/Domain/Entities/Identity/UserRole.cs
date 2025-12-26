using Domain.Common.Contracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Identity;

[Table("UserRole")]
public class UserRole : AuditableEntity<long>
{
    public long UserId { get; set; }

    public long RoleId { get; set; }

    public virtual User? User { get; set; }
    public virtual Role? Role { get; set; }
}