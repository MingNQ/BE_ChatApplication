using System.Collections.Concurrent;
using System.Text;
using Application.Common.Services;
using Application.Configurations;
using Application.Interfaces.Infrastructures.Integrates.External.Service.Email;
using Microsoft.Extensions.Options;
using Shared.Constants.EmailTemplate;

namespace External.Service.Email;

public class EmailTemplateProvider(IOptions<AppSettings> applicationInfoConfiguration,
    IWebSettingService webSettingService)
    : IEmailTemplateProvider
{
    private const string NameSpace = "External.Service.Email.EmailTemplates";

    private readonly AppSettings _applicationInfoSettings = applicationInfoConfiguration.Value;
    private readonly ConcurrentDictionary<string, string> _defaultTemplates = new();

    public string GetTemplateByName(string name, string languageCode = EmailSupportLanguageConst.Vietnamese)
    {
        return _defaultTemplates.GetOrAdd($"{name}_{languageCode}", _ =>
        {
            var assembly = typeof(EmailTemplateProvider).Assembly;
            using var stream = assembly.GetManifestResourceStream($"{NameSpace}.{languageCode}.{name}.html");
            byte[] bytes;

            using (var streamReader = new MemoryStream())
            {
                stream?.CopyTo(streamReader);
                bytes = streamReader.ToArray();
            }

            string template = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            return GetApplicationInfoAsync(template).GetAwaiter().GetResult();
        });
    }

    public async Task<string> GetTemplateByNameAsync(string name, string languageCode = EmailSupportLanguageConst.Vietnamese)
    {
        var assembly = typeof(EmailTemplateProvider).Assembly;
        await using var stream = assembly.GetManifestResourceStream($"{NameSpace}.{languageCode}.{name}.html");
        byte[] bytes;

        using (var streamReader = new MemoryStream())
        {
            stream?.CopyTo(streamReader);
            bytes = streamReader.ToArray();
        }

        string template = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        return await GetApplicationInfoAsync(template);
    }

    private async Task<string> GetApplicationInfoAsync(string template)
    {
        template = template.Replace("{THIS_YEAR}", DateTime.Now.Year.ToString());
        template = template.Replace("{EMAIL_LOGO_URL}", string.Empty);

        // Get WebSetting data
        var webSetting = await webSettingService.GetWebSettingAsync();

        if (webSetting != null)
        {
            // Use WebSetting data
            template = template.Replace("{WEBSITE_NAME}", !string.IsNullOrEmpty(webSetting.Name) ? webSetting.Name : _applicationInfoSettings.Name);
            template = template.Replace("{CONTACT_EMAIL}", !string.IsNullOrEmpty(webSetting.Email) ? webSetting.Email : _applicationInfoSettings.Email);
            template = template.Replace("{WEBSITE_URL}", _applicationInfoSettings.ClientRootAddress);
            template = template.Replace("{ADDRESS}", !string.IsNullOrEmpty(webSetting.Address) ? webSetting.Address : _applicationInfoSettings.Address);
            template = template.Replace("{PHONE_NUMBER}", !string.IsNullOrEmpty(webSetting.PhoneNumber) ? webSetting.PhoneNumber : _applicationInfoSettings.Phone);
            template = template.Replace("{FACEBOOK_URL}", webSetting.Facebook);
            template = template.Replace("{INSTAGRAM_URL}", webSetting.Instagram);
            template = template.Replace("{YOUTUBE_URL}", webSetting.Youtube);
            template = template.Replace("{TIKTOK_URL}", webSetting.Tiktok);
            template = template.Replace("{LINKEDIN_URL}", webSetting.Linkedin);
            template = template.Replace("{TWITTER_URL}", webSetting.Twitter);
            template = template.Replace("{COPYRIGHT}", string.IsNullOrEmpty(webSetting.Copyright) ? $"© {DateTime.Now.Year} {webSetting.Name ?? _applicationInfoSettings.Name}. All Rights Reserved." : webSetting.Copyright);
            template = template.Replace("{LOGO_URL}", webSetting.Logo?.FullPathUrl ?? _applicationInfoSettings.Logo);
        }
        else
        {
            // Fallback to ApplicationInfoConfiguration
            template = template.Replace("{WEBSITE_NAME}", _applicationInfoSettings.Name);
            template = template.Replace("{CONTACT_EMAIL}", _applicationInfoSettings.Email);
            template = template.Replace("{WEBSITE_URL}", _applicationInfoSettings.ClientRootAddress);
            template = template.Replace("{ADDRESS}", _applicationInfoSettings.Address);
            template = template.Replace("{PHONE_NUMBER}", _applicationInfoSettings.Phone);
            template = template.Replace("{FACEBOOK_URL}", string.Empty);
            template = template.Replace("{INSTAGRAM_URL}", string.Empty);
            template = template.Replace("{YOUTUBE_URL}", string.Empty);
            template = template.Replace("{TIKTOK_URL}", string.Empty);
            template = template.Replace("{LINKEDIN_URL}", string.Empty);
            template = template.Replace("{TWITTER_URL}", string.Empty);
            template = template.Replace("{COPYRIGHT}", $"© {DateTime.Now.Year} {_applicationInfoSettings.Name}. All Rights Reserved.");
            template = template.Replace("{LOGO_URL}", _applicationInfoSettings.Logo);
        }

        // Keep existing application info replacements for backward compatibility
        template = template.Replace("{APPLICATION_SITE}", _applicationInfoSettings.ServerRootAddress);

        return template;
    }
}