using MailKit;
using MailKit.Search;
using MimeKit;
using quotifyai.Core.Common;
using quotifyai.Core.Emails;

namespace quotifyai.Infrastructure.Email;

internal sealed class EmailService(EmailServiceOptions options, IImapClientFactory clientFactory) : IEmailService
{
    internal EmailServiceOptions Options => options;

    public async Task<Result<List<EmailContent>, string>> GetEmailsAsync(DateTime fromUtcDate)
    {
        var emails = new List<EmailContent>();

        using var client = clientFactory.CreateClient();
        try
        {
            await client.ConnectAsync(Options.ImapHost, Options.ImapPort, Options.UseSsl);
            await client.AuthenticateAsync(Options.Username, Options.Password);

            var inbox = client.Inbox;
            await inbox.OpenAsync(FolderAccess.ReadOnly);

            var query = SearchQuery.DeliveredAfter(fromUtcDate.AddDays(-1));
            var uids = await inbox.SearchAsync(query);

            foreach (var uid in uids)
            {
                var message = await inbox.GetMessageAsync(uid);

                if (message.Date.UtcDateTime < fromUtcDate)
                {
                    continue;                
                }

                string body = message.TextBody ?? message.HtmlBody ?? string.Empty;
                
                DateTimeOffset sentDate = message.Date;
                
                var sender = (message.From.FirstOrDefault() as MailboxAddress)?.Address ?? string.Empty;

                var images = new List<byte[]>();
                foreach (var attachment in message.Attachments)
                {
                    if (attachment is MimePart mimePart && mimePart.ContentType.MimeType.StartsWith("image/"))
                    {
                        using var memoryStream = new MemoryStream();
                        await mimePart.Content.DecodeToAsync(memoryStream);
                        images.Add(memoryStream.ToArray());
                    }
                }

                emails.Add(new EmailContent(
                    sender,
                    body,
                    sentDate,
                    images));
            }

            await client.DisconnectAsync(quit: true);
        }
        catch (Exception ex)
        {
            return $"Error fetching emails: {ex.Message}";
        }

        return emails;
    }
}