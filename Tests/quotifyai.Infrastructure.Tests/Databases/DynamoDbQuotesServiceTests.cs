using System.Globalization;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Moq;
using NUnit.Framework;
using quotifyai.Core.Shared;
using quotifyai.Infrastructure.Databases;

namespace quotifyai.Infrastructure.Tests.Databases;

[TestFixture]
public class DynamoDBQuotesServiceTests
{
    private Mock<IDateTimeService> _mockDataTimeService;
    private Mock<IAmazonDynamoDB> _mockDynamoDBClient;
    private Mock<IDynamoDBTableFactory> _mockTableFactory;
    private Mock<IDynamoDBTable> _mockTable;
    private DynamoDBQuotesService _sut;

    [SetUp]
    public void Setup()
    {
        _mockDataTimeService = new Mock<IDateTimeService>();
        _mockDynamoDBClient = new Mock<IAmazonDynamoDB>();
        _mockTableFactory = new Mock<IDynamoDBTableFactory>();
        _mockTable = new Mock<IDynamoDBTable>();
        _mockTableFactory
            .Setup(factory => factory.LoadTable(It.IsAny<IAmazonDynamoDB>(), It.IsAny<string>()))
            .Returns(_mockTable.Object);        
        _sut = new DynamoDBQuotesService(
            _mockDataTimeService.Object,
            _mockDynamoDBClient.Object,
            _mockTableFactory.Object,
            "Quotes");
    }

    [Test]
    public async Task SaveQuoteAsync_ShouldCallPutItemAsync_WithCorrectDocument()
    {
        // Arrange
        var expectedDateTime = DateTime.UtcNow;
        var expectedDateTimeString = expectedDateTime.ToString(Constants.QuoteDateTimeFormat);
        _mockDataTimeService.Setup(s => s.GetCurrentDateTimeUtc()).Returns(expectedDateTime);
        var expectedDocumentData = "Test Quote";
        _mockTable
            .Setup(table => table.PutItemAsync(It.IsAny<Document>(), default))
            .ReturnsAsync([]);

        // Act
        await _sut.SaveQuoteAsync(expectedDocumentData);

        // Assert
        _mockTable.Verify(t => t.PutItemAsync(It.Is<Document>(s =>
            s.ContainsKey("quoteId") && IsValidGuid(s["quoteId"]) &&
            s.ContainsKey("createdDate") && IsValidDateTime(s["createdDate"]) && s["createdDate"] == expectedDateTimeString &&
            s.Contains("documentData") && s["documentData"] == expectedDocumentData
        ), 
        default), Times.Once);
    }

    [Test]
    public async Task SaveQuoteAsync_ShouldCallTableFactoryWithCorrectParameters()
    {
        // Arrange
        var expectedDateTime = DateTime.UtcNow;
        var expectedDateTimeString = expectedDateTime.ToString(Constants.QuoteDateTimeFormat);
        _mockDataTimeService.Setup(s => s.GetCurrentDateTimeUtc()).Returns(expectedDateTime);
        var expectedDocumentData = "Test Quote";
        _mockTable
            .Setup(table => table.PutItemAsync(It.IsAny<Document>(), default))
            .ReturnsAsync([]);

        // Act
        await _sut.SaveQuoteAsync(expectedDocumentData);

        // Assert
        _mockTableFactory.Verify(factory => factory.LoadTable(_mockDynamoDBClient.Object, "Quotes"), Times.Once);
    }

    private static bool IsValidGuid(string value) => Guid.TryParse(value, out _);

    private static bool IsValidDateTime(string value) => 
        DateTime.TryParseExact(
            value,
            Constants.QuoteDateTimeFormat,
            null,
            DateTimeStyles.AssumeUniversal,
            out _);
}