using quotifyai.Core.Shared;

namespace quotifyai.Infrastructure;

public sealed class DateTimeService : IDateTimeService
{
    public DateTime GetCurrentDateTimeUtc()
    {
        return DateTime.UtcNow;
    }
}
