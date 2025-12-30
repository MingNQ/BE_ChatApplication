namespace TravelBlogs.Core.Application.Dto.Integrates.Notifications;

public class EmailNotificationRequest
{
    public string ToAddress { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}