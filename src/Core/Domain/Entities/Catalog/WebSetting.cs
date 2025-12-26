using Domain.Common.Contracts;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Catalog;

public class WebSetting : SeoEntity<long>, IAggregateRoot
{
    public long? LogoId { get; protected set; }

    public long? FaviconId { get; protected set; }
    public long? HeroBackgroundId { get; protected set; }
    public long? HeroBackgroundMobileId { get; protected set; }
    public long? PaymentMethodId { get; protected set; }

    [MaxLength(255)]
    public string Name { get; private set; } = string.Empty;

    [MaxLength(500)]
    public string Slogan { get; private set; } = string.Empty;

    [MaxLength(15)]
    public string PhoneNumber { get; private set; } = string.Empty;

    [MaxLength(255)]
    public string Fax { get; private set; } = string.Empty;

    [MaxLength(255)]
    public string Email { get; private set; } = string.Empty;

    [MaxLength(255)]
    public string Address { get; private set; } = string.Empty;

    [MaxLength(255)]
    public string Location { get; private set; } = string.Empty;

    [MaxLength(255)]
    public string Facebook { get; private set; } = string.Empty;

    [MaxLength(255)]
    public string Youtube { get; private set; } = string.Empty;

    [MaxLength(255)]
    public string Linkedin { get; private set; } = string.Empty;

    [MaxLength(255)]
    public string Twitter { get; private set; } = string.Empty;

    [MaxLength(255)]
    public string Tiktok { get; private set; } = string.Empty;

    [MaxLength(255)]
    public string Instagram { get; private set; } = string.Empty;

    [MaxLength(255)]
    public string Copyright { get; private set; } = string.Empty;

    [MaxLength(255)]
    public string UrlBooking { get; private set; } = string.Empty;

    [MaxLength(255)]
    public string UrlWhatsapp { get; private set; } = string.Empty;

    public virtual FileStorage? Favicon { get; set; }
    public virtual FileStorage? Logo { get; set; }
    public virtual FileStorage? HeroBackground { get; set; }
    public virtual FileStorage? HeroBackgroundMobile { get; set; }
    public virtual FileStorage? PaymentMethod { get; set; }

    public static WebSetting Create(long? logoId,
        long? faviconId,
        long? heroBackgroundId,
        long? heroBackgroundMobileId,
        long? paymentMethodId,
        string name,
        string slogan,
        string phoneNumber,
        string fax,
        string email,
        string address,
        string location,
        string facebook,
        string youtube,
        string linkedin,
        string twitter,
        string tiktok,
        string instagram,
        string copyright,
        string urlBooking,
        string urlWhatsapp)
    {
        return new WebSetting
        {
            Name = name,
            Slogan = slogan,
            LogoId = logoId,
            FaviconId = faviconId,
            HeroBackgroundId = heroBackgroundId,
            HeroBackgroundMobileId = heroBackgroundMobileId,
            PaymentMethodId = paymentMethodId,
            PhoneNumber = phoneNumber,
            Fax = fax,
            Email = email,
            Address = address,
            Location = location,
            Facebook = facebook,
            Youtube = youtube,
            Linkedin = linkedin,
            Twitter = twitter,
            Tiktok = tiktok,
            Instagram = instagram,
            Copyright = copyright,
            UrlBooking = urlBooking,
            UrlWhatsapp = urlWhatsapp,
        };
    }

    public void Update(long? logoId,
        long? faviconId,
        long? heroBackgroundId,
        long? heroBackgroundMobileId,
        long? paymentMethodId,
        string name,
        string slogan,
        string phoneNumber,
        string fax,
        string email,
        string address,
        string location,
        string facebook,
        string youtube,
        string linkedin,
        string twitter,
        string tiktok,
        string instagram,
        string copyright,
        string urlBooking,
        string urlWhatsapp)
    {
        Name = name;
        Slogan = slogan;
        LogoId = logoId;
        FaviconId = faviconId;
        HeroBackgroundId = heroBackgroundId;
        HeroBackgroundMobileId = heroBackgroundMobileId;
        PaymentMethodId = paymentMethodId;
        PhoneNumber = phoneNumber;
        Fax = fax;
        Email = email;
        Address = address;
        Location = location;
        Facebook = facebook;
        Youtube = youtube;
        Linkedin = linkedin;
        Twitter = twitter;
        Tiktok = tiktok;
        Instagram = instagram;
        Copyright = copyright;
        UrlBooking = urlBooking;
        UrlWhatsapp = urlWhatsapp;
    }
}