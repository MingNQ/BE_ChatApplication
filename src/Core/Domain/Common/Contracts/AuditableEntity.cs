using System.ComponentModel.DataAnnotations;

namespace Domain.Common.Contracts;

public class AuditableEntity<T> : BaseEntity<T>, ISoftDelete
{
    public long CreatedBy { get; set; }
    public DateTimeOffset CreatedOn { get; private set; } = DateTimeOffset.UtcNow;
    public long LastModifiedBy { get; set; }
    public DateTimeOffset? LastModifiedOn { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? DeletedOn { get; set; }
    public long? DeletedBy { get; set; }
}

public abstract class SeoEntity<T> : BaseEntity<T>, IAuditableEntity, ISoftDelete, ISeoEntity
{
    public long CreatedBy { get; set; }
    public DateTimeOffset CreatedOn { get; private set; } = DateTimeOffset.UtcNow;
    public long LastModifiedBy { get; set; }
    public DateTimeOffset? LastModifiedOn { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? DeletedOn { get; set; }
    public long? DeletedBy { get; set; }

    /// <summary>
    /// Prefer 60
    /// </summary>
    [MaxLength(255)]
    public string? MetaTitle { get; set; }

    /// <summary>
    /// Prefer 160
    /// </summary>
    [MaxLength(1000)]
    public string? MetaDescription { get; set; }

    /// <summary>
    /// Prefer 255
    /// </summary>
    [MaxLength(1000)]
    public string? MetaKeywords { get; set; }

    /// <summary>
    /// Prefer 100
    /// </summary>
    [MaxLength(1000)]
    public string UrlSlug { get; set; } = string.Empty;

    public bool? NoIndex { get; set; }
    public bool? NoFollow { get; set; }

    [MaxLength(2083)]
    public string? CanonicalUrl { get; set; }

    /// <summary>
    /// Prefer 60
    /// </summary>
    [MaxLength(255)]
    public string? OgTitle { get; set; }

    /// <summary>
    /// Prefer 160
    /// </summary>
    [MaxLength(1000)]
    public string? OgDescription { get; set; }

    [MaxLength(2083)]
    public string? OgImageUrl { get; set; }

    [MaxLength(255)]
    public string? Author { get; set; }

    [MaxLength(255)]
    public string? Viewport { get; set; } = "idth=device-width, initial-scale=1.0";

    [MaxLength(255)]
    public string? Hreflang { get; set; }
}