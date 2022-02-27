using System;
using System.Globalization;

namespace EveryeyeFeed
{
    public static class Helpers
    {
        public static string GetRssDate(DateTime date)
            => date.ToString("ddd, d MMM yyyy 00:00:00 +0000", new CultureInfo("en-us"));
    }
}
