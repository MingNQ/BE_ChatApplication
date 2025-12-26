using TimeZoneConverter;

namespace Application.Helpers;

public static class DateTimeHelper
{
    private static readonly TimeZoneInfo VnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

    public static DateTime ToVietnamDate(DateTimeOffset utcDateTimeOffset)
    {
        return TimeZoneInfo.ConvertTime(utcDateTimeOffset, VnTimeZone).Date;
    }

    public static DateTime GetTodayVietnam(DateTime? input = null)
    {
        return TimeZoneInfo.ConvertTimeFromUtc(input ?? DateTime.UtcNow, VnTimeZone).Date;
    }

    public static bool IsSameVietnamDate(DateTimeOffset utc, DateTime targetDate)
    {
        return ToVietnamDate(utc) == targetDate;
    }

    public static bool IsSameVietnamMonth(DateTimeOffset utc, DateTime targetDate)
    {
        var vn = ToVietnamDate(utc);
        return vn.Year == targetDate.Year && vn.Month == targetDate.Month;
    }

    public static bool IsSameVietnamYearAndMonth(DateTimeOffset utc, int year, int month)
    {
        var vn = ToVietnamDate(utc);
        return vn.Year == year && vn.Month == month;
    }

    public static long GenUnixTime()
    {
        var currentTime = DateTimeOffset.UtcNow;
        return currentTime.ToUnixTimeMilliseconds();
    }

    public static TimeSpan GetGmtOffsetByTimeZone(string timeZone)
    {
        var timeZoneId = TZConvert.GetTimeZoneInfo(timeZone);

        // Get the GMT offset
        var gmtOffset = timeZoneId.BaseUtcOffset;

        // Determine if the time zone is currently observing daylight saving time
        bool isDaylightSavingTime = timeZoneId.IsDaylightSavingTime(DateTime.UtcNow);

        // If observing daylight saving time, adjust the offset
        if (isDaylightSavingTime)
        {
            gmtOffset += new TimeSpan(1, 0, 0);
        }

        return gmtOffset;
    }
}