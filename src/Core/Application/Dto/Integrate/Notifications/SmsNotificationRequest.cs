namespace TravelBlogs.Core.Application.Dto.Integrates.Notifications;

public class SmsNotificationRequest
{
    public string PhoneNumber { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string CountryName { get; set; } = string.Empty;
}