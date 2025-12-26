using Application.Dto.Persistence.Catalog.WebSettings;

namespace Application.Common.Services;

public interface IWebSettingService
{
    Task<WebSettingDto?> GetWebSettingAsync(CancellationToken cancellationToken = default);

    void SetWebSettingCacheAsync(WebSettingDto webSettingDto);
}