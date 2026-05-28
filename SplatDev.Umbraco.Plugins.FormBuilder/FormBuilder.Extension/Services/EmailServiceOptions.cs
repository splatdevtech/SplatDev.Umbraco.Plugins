namespace FormBuilder.Extension.Services;

public class EmailServiceOptions
{
    public const string Section = "FormBuilder:Email";

    public string SmtpHost { get; set; } = "localhost";
    public int SmtpPort { get; set; } = 25;
    public string SmtpUsername { get; set; } = string.Empty;
    public string SmtpPassword { get; set; } = string.Empty;
    public bool EnableSsl { get; set; }
    public string DefaultFromAddress { get; set; } = "noreply@example.com";
    public string DefaultFromName { get; set; } = "FormBuilder";
}
