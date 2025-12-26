using Application.Common.Repositories;
using Application.Common.Services;
using Application.Common.UnitOfWork;
using Application.Dto.Persistence.Catalog.WebSettings;
using Domain.Entities.Catalog;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class WebSettingService(IUnitOfWork unitOfWork, IFilePathService filePathService) : IWebSettingService
{
    private readonly IWriteRepository<WebSetting> _webSettingRepository = unitOfWork.GetRepository<WebSetting>();

    private WebSettingDto? _webSettingDto;

    public async Task<WebSettingDto?> GetWebSettingAsync(CancellationToken cancellationToken = default)
    {
        if (_webSettingDto != null)
        {
            return _webSettingDto;
        }

        var webSetting = await _webSettingRepository.GetFirstOrDefaultAsync(include: x => x
            .Include(o => o.Logo)
            .Include(o => o.HeroBackground)
            .Include(o => o.HeroBackgroundMobile)
            .Include(o => o.PaymentMethod)
            .Include(o => o.Favicon!));
        if (webSetting == null)
        {
            return null;
        }

        var webSettingDto = webSetting.Adapt<WebSettingDto>();

        filePathService.BindFullPaths(webSettingDto.Logo);
        filePathService.BindFullPaths(webSettingDto.Favicon);
        filePathService.BindFullPaths(webSettingDto.HeroBackground);
        filePathService.BindFullPaths(webSettingDto.HeroBackgroundMobile);
        filePathService.BindFullPaths(webSettingDto.PaymentMethod);

        _webSettingDto = webSettingDto;

        return webSettingDto;
    }

    public void SetWebSettingCacheAsync(WebSettingDto webSettingDto)
    {
        _webSettingDto = webSettingDto;
    }
}