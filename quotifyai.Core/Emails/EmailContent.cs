namespace quotifyai.Core.Emails;

public sealed record EmailContent(
    string Sender,
    string Body,
    DateTimeOffset SentDate,
    IReadOnlyList<byte[]> Images);