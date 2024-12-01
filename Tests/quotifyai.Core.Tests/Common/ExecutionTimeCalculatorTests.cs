using NUnit.Framework;
using FluentAssertions;
using quotifyai.Core.Common;

namespace quotifyai.Core.Tests.Common;

[TestFixture]
public class ExecutionTimeCalculatorTests
{
    [Test]
    public void GetLastExecutionUtcDateTimeRelativeTo_ShouldSubtract13Hours_WhenItIsExactly9AM()
    {
        // Arrange
        var fixedUtcNow = new DateTime(2024, 12, 1, 9, 0, 0, DateTimeKind.Utc); // Exactly 9 AM UTC

        // Act
        var result = ExecutionTimeCalculator.GetLastExecutionUtcDateTimeRelativeTo(fixedUtcNow);

        // Assert
        result.Should().Be(fixedUtcNow.AddHours(-13)); // 9 AM - 13 hours = 8 PM previous day
    }

    [Test]
    public void GetLastExecutionUtcDateTimeRelativeTo_ShouldSubtract1Hour_WhenItIsNot9AM()
    {
        // Arrange
        var fixedUtcNow = new DateTime(2024, 12, 1, 14, 30, 0, DateTimeKind.Utc); // 2:30 PM UTC

        // Act
        var result = ExecutionTimeCalculator.GetLastExecutionUtcDateTimeRelativeTo(fixedUtcNow);

        // Assert
        result.Should().Be(fixedUtcNow.AddHours(-1)); // 2:30 PM - 1 hour = 1:30 PM
    }

    [Test]
    public void GetLastExecutionUtcDateTimeRelativeTo_ShouldReturnCorrectTime_WhenTimeIsOneMinuteBefore9AM()
    {
        // Arrange
        var fixedUtcNow = new DateTime(2024, 12, 1, 8, 59, 0, DateTimeKind.Utc); // 8:59 AM UTC

        // Act
        var result = ExecutionTimeCalculator.GetLastExecutionUtcDateTimeRelativeTo(fixedUtcNow);

        // Assert
        result.Should().Be(fixedUtcNow.AddHours(-1)); // 8:59 AM - 1 hour = 7:59 AM
    }

    [Test]
    public void GetLastExecutionUtcDateTimeRelativeTo_ShouldReturnCorrectTime_WhenTimeIsOneMinuteAfter9AM()
    {
        // Arrange
        var fixedUtcNow = new DateTime(2024, 12, 1, 9, 1, 0, DateTimeKind.Utc); // 9:01 AM UTC

        // Act
        var result = ExecutionTimeCalculator.GetLastExecutionUtcDateTimeRelativeTo(fixedUtcNow);

        // Assert
        result.Should().Be(fixedUtcNow.AddHours(-1)); // 9:01 AM - 1 hour = 8:01 AM
    }

    [Test]
    public void GetLastExecutionUtcDateTimeRelativeTo_ShouldHandleMidnightTransitionCorrectly()
    {
        // Arrange
        var fixedUtcNow = new DateTime(2024, 12, 1, 0, 0, 0, DateTimeKind.Utc); // Midnight UTC

        // Act
        var result = ExecutionTimeCalculator.GetLastExecutionUtcDateTimeRelativeTo(fixedUtcNow);

        // Assert
        result.Should().Be(fixedUtcNow.AddHours(-1)); // Midnight - 1 hour = 11:00 PM previous day
    }
}
