using System.ComponentModel.DataAnnotations;
using Domain.Common.Contracts;

namespace EfCore.Persistence.Auditing;

public class Trail : BaseEntity
{
    public long UserId { get; set; }

    [MaxLength(10)]
    public string? Type { get; set; }

    [MaxLength(255)]
    public string? TableName { get; set; }

    public DateTimeOffset DateTime { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? AffectedColumns { get; set; }

    [MaxLength(255)]
    public string? PrimaryKey { get; set; }
}