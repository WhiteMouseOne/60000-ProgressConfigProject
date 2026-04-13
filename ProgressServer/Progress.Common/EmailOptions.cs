namespace Progress.Common
{
    public class EmailOptions
    {
        public const string SectionName = "Email";
        public string SmtpHost { get; set; } = "localhost";
        public int SmtpPort { get; set; } = 25;
        public string? User { get; set; }
        public string? Password { get; set; }
        public string From { get; set; } = "noreply@localhost";
        public bool UseSsl { get; set; }
        public string RepairRecipients { get; set; } = "";
        public string AlertRecipients { get; set; } = "";
    }
}
