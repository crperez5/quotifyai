using Amazon.Lambda.Core;
using Moq;
using NUnit.Framework;
using quotifyai.Core.Emails;
using quotifyai.Core.Quotes;

namespace quotifyai.Assistant.Tests;

[TestFixture]
public class FunctionTests
{
    [SetUp]
    public void Setup()
    {
        Environment.SetEnvironmentVariable("AWS_REGION", "us-west-2");
        Environment.SetEnvironmentVariable("DYNAMODB_ENDPOINT", "http://localhost:8000");
        Environment.SetEnvironmentVariable("IMAP_HOST", "test.local");
        Environment.SetEnvironmentVariable("IMAP_PORT", "143");
        Environment.SetEnvironmentVariable("SMTP_HOST", "test.local");
        Environment.SetEnvironmentVariable("SMTP_PORT", "25");
        Environment.SetEnvironmentVariable("EMAIL_USERNAME", "name");
        Environment.SetEnvironmentVariable("EMAIL_PASSWORD", "password");
        Environment.SetEnvironmentVariable("USE_SSL", "false");
    }
    
    [Test]
    public async Task FunctionHandler_CallsSaveQuoteAsync_WhenCalled()
    {
        // Arrange
        var mockQuotesService = new Mock<IQuotesService>();
        var mockEmailService = new Mock<IEmailService>();

        mockQuotesService
            .Setup(service => service.SaveQuoteAsync(It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        var sut = new Function(mockQuotesService.Object, mockEmailService.Object);

        var mockContext = new Mock<ILambdaContext>();

        // Act
        await sut.FunctionHandler(mockContext.Object);

        // Assert
        mockQuotesService.Verify(service => service.SaveQuoteAsync("data"), Times.Once);
    }
}