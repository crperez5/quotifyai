using System.Text;
using FluentAssertions;
using FluentAssertions.Common;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;
using Moq;
using NUnit.Framework;
using quotifyai.Infrastructure.Email;

namespace quotifyai.Infrastructure.Tests.Email;

[TestFixture]
public class EmailServiceTests
{
    private static readonly DateTime _FromDate = new(2024, 11, 30, 15, 0, 0, DateTimeKind.Utc);

    private Mock<IImapClientFactory> _clientFactoryMock;
    private Mock<IImapClient> _imapClientMock;
    private Mock<IMailFolder> _mailFolderMock;
    private EmailService _sut;
    private EmailServiceOptions _options;

    [SetUp]
    public void SetUp()
    {
        // Arrange - Mock dependencies
        _clientFactoryMock = new Mock<IImapClientFactory>();
        _imapClientMock = new Mock<IImapClient>();
        _mailFolderMock = new Mock<IMailFolder>();
        _options = new EmailServiceOptions(
            "imap.example.com",
            993,
            "smtp.example.com",
            587,
            "user@example.com",
            "password",
            true);

        _clientFactoryMock.Setup(x => x.CreateClient()).Returns(_imapClientMock.Object);
        _imapClientMock.Setup(x => x.Inbox).Returns(_mailFolderMock.Object);

        _sut = new EmailService(_options, _clientFactoryMock.Object);
    }

    [Test]
    public async Task GetEmailsAsync_ShouldReturnEmails_WhenSuccessful()
    {
        // Arrange
        var uniqueId = new UniqueId();
        MimeMessage message = GetMessage();

        _mailFolderMock.Setup(x => x.SearchAsync(It.IsAny<SearchQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([uniqueId])
            .Callback<SearchQuery, CancellationToken>((query, _) =>
            {
                query.Should().BeOfType<DateSearchQuery>();
                query.As<DateSearchQuery>().Date.Should().Be(_FromDate.AddDays(-1));
                query.As<DateSearchQuery>().Term.Should().Be(SearchTerm.DeliveredAfter);
            });           

        _mailFolderMock.Setup(x => x.GetMessageAsync(uniqueId, It.IsAny<CancellationToken>(), It.IsAny<ITransferProgress>()))
            .ReturnsAsync(message);

        // Act
        var result = await _sut.GetEmailsAsync(_FromDate);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var emails = result.ToSuccess();
        emails.Should().HaveCount(1);
        emails[0].Sender.Should().Be("sender@example.com");
        emails[0].Body.Should().Be("This is a test email body.");
        emails[0].SentDate.Should().BeAfter(_FromDate);
        emails[0].Images.Should().HaveCount(1);
    }

    [Test]
    public async Task GetEmailsAsync_ShouldFilterEmailsByTime_WhenSuccessful()
    {
        // Arrange
        var uniqueId1 = new UniqueId(1);
        var uniqueId2 = new UniqueId(2);
        MimeMessage message1 = GetMessage(dateSentInRange: true);
        MimeMessage message2 = GetMessage(dateSentInRange: false);

        _mailFolderMock.Setup(x => x.SearchAsync(It.IsAny<SearchQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([uniqueId1, uniqueId2]);           

        _mailFolderMock.Setup(x => x.GetMessageAsync(uniqueId1, It.IsAny<CancellationToken>(), It.IsAny<ITransferProgress>()))
            .ReturnsAsync(message1);
        _mailFolderMock.Setup(x => x.GetMessageAsync(uniqueId2, It.IsAny<CancellationToken>(), It.IsAny<ITransferProgress>()))
            .ReturnsAsync(message2);            

        // Act
        var result = await _sut.GetEmailsAsync(_FromDate);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var emails = result.ToSuccess();
        emails.Should().HaveCount(1);
        emails[0].Sender.Should().Be("sender@example.com");
        emails[0].Body.Should().Be("This is a test email body.");
        emails[0].SentDate.Should().BeAfter(_FromDate);
        emails[0].Images.Should().HaveCount(1);
    }    

    [Test]
    public async Task GetEmailsAsync_ShouldReturnErrorMessage_WhenExceptionOccurs()
    {
        // Arrange
        var fromDate = DateTime.UtcNow.AddDays(-1);
        var expectedErrorMessage = "Error fetching emails: Some error";

        _imapClientMock.Setup(x => x.ConnectAsync(
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<bool>(),
            It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Some error"));

        // Act
        var result = await _sut.GetEmailsAsync(fromDate);

        // Assert
        result.IsError.Should().BeTrue();
        result.ToError().Should().Be(expectedErrorMessage);
    }

    [Test]
    public async Task GetEmailsAsync_ShouldReturnEmptyList_WhenNoEmailsFound()
    {
        // Arrange
        var fromDate = DateTime.UtcNow.AddDays(-1);

        _mailFolderMock.Setup(x => x.SearchAsync(It.IsAny<SearchQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var result = await _sut.GetEmailsAsync(fromDate);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.ToSuccess().Should().BeEmpty();
    }

    [Test]
    public async Task GetEmailsAsync_ShouldHandleEmptyAttachmentsGracefully()
    {
        // Arrange
        var uniqueId = new UniqueId();
        var fromDate = DateTime.UtcNow.AddDays(-1);
        var message = GetMessage(withEmptyAttachments: true);

        _mailFolderMock.Setup(x => x.SearchAsync(It.IsAny<SearchQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([uniqueId]);

        _mailFolderMock.Setup(x => x.GetMessageAsync(uniqueId, It.IsAny<CancellationToken>(), It.IsAny<ITransferProgress>()))
            .ReturnsAsync(message);

        // Act
        var result = await _sut.GetEmailsAsync(fromDate);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var emails = result.ToSuccess();
        emails.Should().HaveCount(1);
        emails[0].Images.Should().BeEmpty(); 
    }

    private static MimeMessage GetMessage(bool dateSentInRange = true, bool withEmptyAttachments = false)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Sender", "sender@example.com"));
        message.To.Add(new MailboxAddress("Recipient", "recipient@example.com"));
        message.Subject = "Test Email";
        if (dateSentInRange)
        {
            message.Date = new DateTimeOffset(_FromDate.AddHours(1), TimeSpan.Zero);
        } 
        else 
        {
            message.Date = new DateTimeOffset(_FromDate.AddHours(-1), TimeSpan.Zero);
        }
        var textPart = new TextPart("plain") { Text = "This is a test email body." };
        message.Body = textPart;

        if (!withEmptyAttachments)
        {
            var memoryStream = new MemoryStream();
            var dummyData = Encoding.UTF8.GetBytes("dummy image data that does not need to be a real image");
            memoryStream.Write(dummyData, 0, dummyData.Length);
            memoryStream.Seek(0, SeekOrigin.Begin);
            var imageAttachment = new MimePart("image", "jpeg")
            {
                Content = new MimeContent(memoryStream),
                FileName = "dummy-image.jpg"
            };

            var multipart = new Multipart("mixed")
            {
                textPart,
                imageAttachment
            };

            message.Body = multipart;
        }

        return message;
    }
}