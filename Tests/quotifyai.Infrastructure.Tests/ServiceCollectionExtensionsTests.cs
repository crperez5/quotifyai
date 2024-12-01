using Amazon.DynamoDBv2;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using quotifyai.Core.Common;
using quotifyai.Core.Emails;
using quotifyai.Core.Quotes;
using quotifyai.Infrastructure.Databases;
using quotifyai.Infrastructure.Email;

namespace quotifyai.Infrastructure.Tests;


[TestFixture]
public class ServiceCollectionExtensionsTests
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
    public void AddInfrastructure_ShouldAdd_DateTimeServiceAsSingleton()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddInfrastructure();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var dateTimeService = serviceProvider.GetService<IDateTimeService>();
        dateTimeService.Should().NotBeNull();
        dateTimeService.Should().BeOfType<DateTimeService>();
    }

    [Test]
    public void AddQuotesService_ShouldAdd_QuotesServiceWithCorrectDependencies()
    {
        // Arrange
        var services = new ServiceCollection();
       
        // Act
        services.AddQuotesService();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var quotesService = serviceProvider.GetService<IQuotesService>();
        quotesService.Should().NotBeNull();
        quotesService.Should().BeOfType<DynamoDBQuotesService>();

        var dynamoDbClient = serviceProvider.GetService<IAmazonDynamoDB>();
        var dynamoDbTableFactory = serviceProvider.GetService<IDynamoDBTableFactory>();
        var dateTimeService = serviceProvider.GetService<IDateTimeService>();

        dynamoDbClient.Should().NotBeNull();
        dynamoDbClient.Should().BeOfType<AmazonDynamoDBClient>();

        dynamoDbTableFactory.Should().NotBeNull();
        dynamoDbTableFactory.Should().BeOfType<DynamoDBTableFactory>();
        
        dateTimeService.Should().NotBeNull();
        dateTimeService.Should().BeOfType<DateTimeService>();
    }

    [Test]
    public void AddQuotesService_ShouldThrow_Exception_WhenAwsRegionEnvironmentVariableIsMissing()
    {
        // Arrange
        Environment.SetEnvironmentVariable("AWS_REGION", null);
        var services = new ServiceCollection();

        // Act & Assert
        Action act = () => services.AddQuotesService();
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("AWS_REGION environment variable is required.");
    }

    [Test]
    public void AddQuotesService_ShouldThrow_Exception_WhenDynamoDbEndpointEnvironmentVariableIsMissing()
    {
        // Arrange
        Environment.SetEnvironmentVariable("DYNAMODB_ENDPOINT", null);
        var services = new ServiceCollection();

        // Act & Assert
        Action act = () => services.AddQuotesService();
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("DYNAMODB_ENDPOINT environment variable is required.");
    }

    [Test]
    public void AddEmailService_ShouldAdd_EmailServiceWithCorrectDependencies()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddEmailService();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var emailService = serviceProvider.GetService<IEmailService>();
        emailService.Should().NotBeNull();
        emailService.Should().BeOfType<EmailService>();

        var imapClientFactory = serviceProvider.GetService<IImapClientFactory>();
        imapClientFactory.Should().NotBeNull();
    }    

    [Test]
    public void AddEmailService_ShouldThrow_Exception_WhenImapHostEnvironmentVariableIsMissing()
    {
        // Arrange
        Environment.SetEnvironmentVariable("IMAP_HOST", null);
        var services = new ServiceCollection();

        // Act & Assert
        Action act = () => services.AddEmailService();
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("IMAP_HOST environment variable is required");
    }

    [Test]
    public void AddEmailService_ShouldThrow_Exception_WhenImapPortEnvironmentVariableIsMissing()
    {
        // Arrange
        Environment.SetEnvironmentVariable("IMAP_PORT", null);
        var services = new ServiceCollection();

        // Act & Assert
        Action act = () => services.AddEmailService();
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("IMAP_PORT environment variable is required");
    }

    [Test]
    public void AddEmailService_ShouldThrow_Exception_WhenSmtpHostEnvironmentVariableIsMissing()
    {
        // Arrange
        Environment.SetEnvironmentVariable("SMTP_HOST", null);
        var services = new ServiceCollection();

        // Act & Assert
        Action act = () => services.AddEmailService();
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("SMTP_HOST environment variable is required");
    }        

    [Test]
    public void AddEmailService_ShouldThrow_Exception_WhenSmtpPortEnvironmentVariableIsMissing()
    {
        // Arrange
        Environment.SetEnvironmentVariable("SMTP_PORT", null);
        var services = new ServiceCollection();

        // Act & Assert
        Action act = () => services.AddEmailService();
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("SMTP_PORT environment variable is required");
    }  

    [Test]
    public void AddEmailService_ShouldThrow_Exception_WhenEmailUsernameEnvironmentVariableIsMissing()
    {
        // Arrange
        Environment.SetEnvironmentVariable("EMAIL_USERNAME", null);
        var services = new ServiceCollection();

        // Act & Assert
        Action act = () => services.AddEmailService();
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("EMAIL_USERNAME environment variable is required");
    }    

    [Test]
    public void AddEmailService_ShouldThrow_Exception_WhenEmailPasswordEnvironmentVariableIsMissing()
    {
        // Arrange
        Environment.SetEnvironmentVariable("EMAIL_PASSWORD", null);
        var services = new ServiceCollection();

        // Act & Assert
        Action act = () => services.AddEmailService();
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("EMAIL_PASSWORD environment variable is required");
    }  

    [Test]
    public void AddEmailService_ShouldSetUseSsl_ToFalse_WhenUseSslEnvironmentVariableIsMissing()
    {
        // Arrange
        Environment.SetEnvironmentVariable("USE_SSL", null);
        var services = new ServiceCollection();

        // Act 
        services.AddEmailService();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var emailService = serviceProvider.GetService<IEmailService>();
        emailService.As<EmailService>().Options.UseSsl.Should().BeFalse();
    }            
}