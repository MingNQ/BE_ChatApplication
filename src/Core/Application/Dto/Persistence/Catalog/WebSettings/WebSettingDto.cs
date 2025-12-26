using Application.Dto.Persistence.Catalog.Common;
using Application.Dto.Persistence.Catalog.FileStorages;

namespace Application.Dto.Persistence.Catalog.WebSettings;

public class WebSettingDto : SeoDto
{
    public FileStorageDto? Favicon { get; set; }
    public FileStorageDto? Logo { get; set; }
    public FileStorageDto? HeroBackground { get; set; }
    public FileStorageDto? HeroBackgroundMobile { get; set; }
    public FileStorageDto? PaymentMethod { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Slogan { get; set; } = string.Empty;
    public string Fax { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Facebook { get; set; } = string.Empty;
    public string Youtube { get; set; } = string.Empty;
    public string Linkedin { get; set; } = string.Empty;

    public string Twitter { get; set; } = string.Empty;

    public string Tiktok { get; set; } = string.Empty;
    public string Instagram { get; set; } = string.Empty;
    public string Copyright { get; set; } = string.Empty;
    public string UrlBooking { get; set; } = string.Empty;
    public string UrlWhatsapp { get; set; } = string.Empty;
}