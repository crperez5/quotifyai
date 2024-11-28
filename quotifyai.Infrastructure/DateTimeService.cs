using quotifyai.Core.Shared;

namespace quotifyai.Infrastructure;

public class DateTimeService : IDateTimeService
{
    public DateTime GetCurrentDateTimeUtc()
    {
        return DateTime.UtcNow;
    }
}
