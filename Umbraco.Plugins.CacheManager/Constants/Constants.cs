namespace Umbraco.Plugins.CacheManager.Constants
{
    public static class CacheConstants
    {
        public static class CacheExpires
        {
#pragma warning disable CA2211 // Non-constant fields should not be visible
            public static double MINUTE = 60 * 1000;
            public static double HOUR = MINUTE * 60;
            public static double DAY = HOUR * 24;
            public static double WEEK = DAY * 7;
            public static double MONTH = WEEK * 4;
            public static double THREE_MONTHS = MONTH * 3;

#pragma warning restore CA2211 // Non-constant fields should not be visible
        }
        public static class CacheRefresh
        {
#pragma warning disable CA2211 // Non-constant fields should not be visible
            public static TimeSpan MINUTE = TimeSpan.FromMinutes(1);
            public static TimeSpan HOUR = TimeSpan.FromHours(1);
            public static TimeSpan DAY = TimeSpan.FromHours(24);
            public static TimeSpan WEEK = TimeSpan.FromDays(7);
            public static TimeSpan MONTH = TimeSpan.FromDays(30);
            public static TimeSpan THREE_MONTHS = TimeSpan.FromDays(90);
#pragma warning restore CA2211 // Non-constant fields should not be visible
        }

        public static class SlidingCacheExpiration
        {
            public const int ONE_HOUR = 60 * 60;
            public const int ONE_DAY = 60 * 60 * 24 * 1;
            public const int SEVEN_DAYS = 60 * 60 * 24 * 7;
        }
    }
}
