using quotifyai.Core.Shared;

namespace quotifyai.Core.Emails;

public interface IEmailService
{
    Task<Result<List<EmailContent>, string>> GetEmailsAsync(DateTime fromDate);
}