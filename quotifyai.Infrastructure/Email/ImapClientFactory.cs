using MailKit.Net.Imap;

namespace quotifyai.Infrastructure.Email;

internal sealed class ImapClientFactory : IImapClientFactory
{
    public IImapClient CreateClient()
    {
        return new ImapClient();
    }
}