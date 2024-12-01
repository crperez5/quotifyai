using Amazon.Lambda.Core;
using Moq;
using NUnit.Framework;
using quotifyai.Core.Common;
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
        var utcNow = new DateTime(2024, 12, 1, 9, 0, 0, DateTimeKind.Utc);        
        var mockDateTimeService = new Mock<IDateTimeService>();
        var mockEmailService = new Mock<IEmailService>();
        var mockQuotesService = new Mock<IQuotesService>();

        mockDateTimeService
            .Setup(s => s.GetCurrentDateTimeUtc())
            .Returns(utcNow);

        mockEmailService
            .Setup(service => service.GetEmailsAsync(It.IsAny<DateTime>()))
            .ReturnsAsync(new List<EmailContent>());               

        mockQuotesService
            .Setup(service => service.SaveQuoteAsync(It.IsAny<string>()))
            .Returns(Task.CompletedTask);


        var sut = new Function(mockDateTimeService.Object, mockQuotesService.Object, mockEmailService.Object);

        var mockContext = new Mock<ILambdaContext>();

        // Act
        await sut.FunctionHandler(mockContext.Object);

        // Assert
        mockQuotesService.Verify(service => service.SaveQuoteAsync("data"), Times.Once);
    }

    [Test]
    public async Task FunctionHandler_CallsGetEmailsAsync_WhenCalled()
    {
        // Arrange
        var utcNow = new DateTime(2024, 12, 1, 9, 0, 0, DateTimeKind.Utc);
        var nextExecution = ExecutionTimeCalculator.GetLastExecutionUtcDateTimeRelativeTo(utcNow);

        var mockDateTimeService = new Mock<IDateTimeService>();
        var mockEmailService = new Mock<IEmailService>();
        var mockQuotesService = new Mock<IQuotesService>();

        mockDateTimeService
            .Setup(s => s.GetCurrentDateTimeUtc())
            .Returns(utcNow);

        mockEmailService
            .Setup(service => service.GetEmailsAsync(It.Is<DateTime>(s => s == nextExecution)))
            .ReturnsAsync(new List<EmailContent>());            

        mockQuotesService
            .Setup(service => service.SaveQuoteAsync(It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        var sut = new Function(
            mockDateTimeService.Object,
            mockQuotesService.Object,
            mockEmailService.Object);

        var mockContext = new Mock<ILambdaContext>();

        // Act
        await sut.FunctionHandler(mockContext.Object);

        // Assert
        mockEmailService.Verify(service => service.GetEmailsAsync(It.Is<DateTime>(s => s == nextExecution)), Times.Once);
    }    

    [Test]
    public async Task FunctionHandler_ShouldHandleEmailError_WhenEmailServiceFails()
    {
        // Arrange
        var utcNow = new DateTime(2024, 12, 1, 9, 0, 0, DateTimeKind.Utc);

        var mockDateTimeService = new Mock<IDateTimeService>();
        var mockEmailService = new Mock<IEmailService>();
        var mockQuotesService = new Mock<IQuotesService>();

        mockEmailService
            .Setup(service => service.GetEmailsAsync(It.IsAny<DateTime>()))
            .ReturnsAsync("random error");

        mockDateTimeService
            .Setup(s => s.GetCurrentDateTimeUtc())
            .Returns(utcNow);

        var sut = new Function(mockDateTimeService.Object, mockQuotesService.Object, mockEmailService.Object);

        var mockContext = new Mock<ILambdaContext>();
        mockContext.Setup(s => s.Logger).Returns(Mock.Of<ILambdaLogger>());

        // Act
        await sut.FunctionHandler(mockContext.Object);

        // Assert
        mockContext.Verify(context => context.Logger.LogError(It.Is<string>(s => s.Contains("random error"))), Times.Once);
    }
}