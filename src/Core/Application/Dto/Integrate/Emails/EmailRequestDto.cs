namespace Application.Dto.Integrate.Emails;

public class EmailRequestDto
{
    public long MailId { get; set; }
    public long FromId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime PostedDate { get; set; }
    public List<UserInfoInput> CcUserId { get; set; } = new();
    public List<UserInfoInput> BccUserId { get; set; } = new();
    public long? CampusId { get; set; }
    public List<UserInfoInput> ReceiverId { get; set; } = new();
    public List<Attachment> Attachment { get; set; } = new();
    public long? BrandId { get; set; }
    public bool IsDraft { get; set; }
    public long FlagId { get; set; }
    public bool IsSaveDraftFlag { get; set; }
    public bool IsForward { get; set; }
    public bool IsConfidentialMail { get; set; }
    public DateTime? DateOfExpiry { get; set; }
    public bool IsRemoveAccess { get; set; }
    public bool IsSchedule { get; set; }
    public DateTime? ScheduleDate { get; set; }
    public bool? IsMoveToInbox { get; set; }
    public long? ParentMailId { get; set; }
    public List<Broadcast> Broadcasts { get; set; } = new();
    public bool IsReply { get; set; }
    public string? Authorized { get; set; }
}

public class UserInfoInput
{
    public long UserId { get; set; }
    public long BrandId { get; set; }
    public long CampusId { get; set; }
    public string Email { get; set; } = string.Empty;
}

public class Attachment
{
    public string FileName { get; set; } = string.Empty;
    public int FileSize { get; set; }
    public string FileUrl { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
}

public class Broadcast
{
    public int BroadcastId { get; set; }
    public bool IsSelectAllUser { get; set; }
    public List<UserInfoInput> SelectedUsers { get; set; } = new();
}