namespace SplatDev.Umbraco.Plugins.CacheManager.Constants
{
    public static class CacheConstants
    {
        public static class CacheExpires
        {
            public static readonly double MINUTE = 60 * 1000;
            public static readonly double HOUR = MINUTE * 60;
            public static readonly double DAY = HOUR * 24;
            public static readonly double WEEK = DAY * 7;
            public static readonly double MONTH = WEEK * 4;
            public static readonly double THREE_MONTHS = MONTH * 3;
        }
        public static class CacheRefresh
        {
            public static readonly TimeSpan MINUTE = TimeSpan.FromMinutes(1);
            public static readonly TimeSpan HOUR = TimeSpan.FromHours(1);
            public static readonly TimeSpan DAY = TimeSpan.FromHours(24);
            public static readonly TimeSpan WEEK = TimeSpan.FromDays(7);
            public static readonly TimeSpan MONTH = TimeSpan.FromDays(30);
            public static readonly TimeSpan THREE_MONTHS = TimeSpan.FromDays(90);
        }

        public static class SlidingCacheExpiration
        {
            public const int ONE_HOUR = 60 * 60;
            public const int ONE_DAY = 60 * 60 * 24 * 1;
            public const int SEVEN_DAYS = 60 * 60 * 24 * 7;
        }
    }
}
