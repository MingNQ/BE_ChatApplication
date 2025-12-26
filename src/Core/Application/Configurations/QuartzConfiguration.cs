namespace Application.Configurations;

public class QuartzConfiguration
{
    public bool Enabled { get; set; }
    public Optional ChangeReportStatus { get; set; } = null!;
    public Optional HandleDisposeFoundReport { get; set; } = null!;
    public Optional CleanupOrphanedFiles { get; set; } = null!;
}

public class Optional
{
    public bool Enabled { get; set; }
    public bool EnabledLog { get; set; }
    public string Scheduled { get; set; } = null!;
}