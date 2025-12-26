using Application.Common.Dto;
using Application.Common.Interfaces;

namespace Application.Dto.Persistence.Catalog.Common;

public class SeoDto : BasicAuditableDto<long>, ISeoDto
{
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
    public string UrlSlug { get; set; } = string.Empty;
    public bool? NoIndex { get; set; }
    public bool? NoFollow { get; set; }
    public string? CanonicalUrl { get; set; }
    public string? OgTitle { get; set; }
    public string? OgDescription { get; set; }
    public string? OgImageUrl { get; set; }
    public string? Author { get; set; }
    public string? Viewport { get; set; }
}