namespace quotifyai.Infrastructure.Email;

internal record EmailServiceOptions(
    string ImapHost,
    int ImapPort,
    string SmtpHost,
    int SmtpPort,
    string Username,
    string Password,
    bool UseSsl)
{
    public static EmailServiceOptions Create()
    {
        string imapHost = Environment.GetEnvironmentVariable("IMAP_HOST")
            ?? throw new InvalidOperationException("IMAP_HOST environment variable is required");
        int imapPort = int.TryParse(Environment.GetEnvironmentVariable("IMAP_PORT"), out var imapPortVal)
            ? imapPortVal
            : throw new InvalidOperationException("IMAP_PORT environment variable is required");
        string smtpHost = Environment.GetEnvironmentVariable("SMTP_HOST")
            ?? throw new InvalidOperationException("SMTP_HOST environment variable is required");
        int smtpPort = int.TryParse(Environment.GetEnvironmentVariable("SMTP_PORT"), out var smtpPortVal)
            ? smtpPortVal
            : throw new InvalidOperationException("SMTP_PORT environment variable is required");
        string username = Environment.GetEnvironmentVariable("EMAIL_USERNAME")
            ?? throw new InvalidOperationException("EMAIL_USERNAME environment variable is required");
        string password = Environment.GetEnvironmentVariable("EMAIL_PASSWORD")
            ?? throw new InvalidOperationException("EMAIL_PASSWORD environment variable is required");
        bool useSsl = bool.TryParse(Environment.GetEnvironmentVariable("USE_SSL"), out var useSslVal)
            && useSslVal;

        return new EmailServiceOptions(
            imapHost,
            imapPort,
            smtpHost,
            smtpPort,
            username,
            password,
            useSsl);
    }
}
