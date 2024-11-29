using System.Reflection;
using Amazon.Lambda.Core;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using quotifyai.Core.Quotes.SaveQuote;

namespace quotifyai.Assistant.Tests;

[TestFixture]
public class FunctionTests
{
    [Test]
    public async Task FunctionHandler_CallsSaveQuoteAsync_WhenCalled()
    {
        // Arrange
        var mockQuotesService = new Mock<IQuotesService>();

        mockQuotesService
            .Setup(service => service.SaveQuoteAsync(It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        var sut = new Function(mockQuotesService.Object);

        var mockContext = new Mock<ILambdaContext>();

        // Act
        await sut.FunctionHandler(mockContext.Object);

        // Assert
        mockQuotesService.Verify(service => service.SaveQuoteAsync("data"), Times.Once);
    }
}