namespace Application.Dto.Integrate.Emails;

public class EmailResponseDto
{
    public int? StatusCode { get; set; }
    public string? StatusMessage { get; set; }
    public long? MailId { get; set; }
}