namespace quotifyai.Core.Common;

public static class ExecutionTimeCalculator
{
    private const int FirstPlannedExecutionHour = 9;
    private const int FirstLastHourDiff = 13;
    private const int ExecutionIntervalInHours = 1;

    public static DateTime GetLastExecutionUtcDateTimeRelativeTo(DateTime utcNow)
    {
        var truncatedUtcNow = new DateTime(
            utcNow.Year,
            utcNow.Month,
            utcNow.Day,
            utcNow.Hour,
            utcNow.Minute,
            0,
            DateTimeKind.Utc);

        var utcStart = new DateTime(truncatedUtcNow.Year, truncatedUtcNow.Month, truncatedUtcNow.Day, FirstPlannedExecutionHour, 0, 0, DateTimeKind.Utc);

        DateTime utcLast;
        if (truncatedUtcNow == utcStart)
        {
            utcLast = truncatedUtcNow.AddHours(-FirstLastHourDiff);
        }
        else
        {
            utcLast = truncatedUtcNow.AddHours(-ExecutionIntervalInHours);
        }

        return utcLast;
    }
}
