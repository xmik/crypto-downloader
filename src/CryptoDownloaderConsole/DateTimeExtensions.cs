using System;

namespace CryptoDownloader
{
    /// <summary>
    /// Extension methods to faciliate operating on System.DateTime and NodaTime.Instant
    /// </summary>
    public static class DateTimeExtensions
    {
        public static DateTime ToDateTime(this NodaTime.Instant nodaTime)
        {
            return nodaTime.ToDateTimeUtc();
        }
        public static NodaTime.Instant ToNodaTime(this DateTime dateTime)
        {
            return NodaTime.Instant.FromDateTimeUtc(dateTime);
        }

        public static NodaTime.Instant CreateNodaTime(int year, int month, int day, int hour, int minute, int second)
        {
            return new DateTime(year,month,day,hour,minute,second, DateTimeKind.Utc).ToNodaTime();
        }
    }
}