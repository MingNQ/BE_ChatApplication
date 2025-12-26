namespace Application.Common.Dto;

public abstract class BaseDto<T>
{
    public T Id { get; set; } = default!;
}

public class BasicAuditableDto<T> : BaseDto<T>
{
    public long CreatedBy { get; set; }
    public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.UtcNow;

    public long LastModifiedBy { get; set; }
    public DateTimeOffset? LastModifiedOn { get; set; } = DateTimeOffset.UtcNow;
}