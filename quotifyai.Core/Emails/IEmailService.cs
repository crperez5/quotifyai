using quotifyai.Core.Common;

namespace quotifyai.Core.Emails;

public interface IEmailService
{
    Task<Result<List<EmailContent>, string>> GetEmailsAsync(DateTime fromDate);
}