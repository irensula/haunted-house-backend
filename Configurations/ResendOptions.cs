namespace GameBackend.Configuration;

public class ResendOptions
{
    public const string SectionName = "Resend";

    public string ApiKey { get; set; } = string.Empty;
    public string FromEmail { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;
    public string ApiUrl { get; set; } = string.Empty;
    public string VerificationUrl { get; set; } = string.Empty;
}