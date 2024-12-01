using MailKit.Net.Imap;

namespace quotifyai.Infrastructure.Email;

internal interface IImapClientFactory
{
    IImapClient CreateClient();
}