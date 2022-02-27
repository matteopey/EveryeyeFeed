using System;
using System.Globalization;

namespace EveryeyeFeed.Library
{
    public static class Helpers
    {
        public static string GetRssDate(DateTime date)
            => date.ToString("ddd, d MMM yyyy hh:mm:00 +0000", new CultureInfo("en-us"));

        public static DateTime GetEveryeyeDate(string dateString)
        {
            var splitted = dateString.Split(",");

            if (splitted.Length == 2)
            {
                // News "11:42, 27 Febbraio 2022"
                var hourSplitted = splitted[0].Split(':');

                var dateSplitted = splitted[1].Trim().Split(' ');
                var month = (int)Enum.Parse(typeof(Months), dateSplitted[1]);

                return new DateTime(
                    int.Parse(dateSplitted[2]),
                    month,
                    int.Parse(dateSplitted[0]),
                    int.Parse(hourSplitted[0]),
                    int.Parse(hourSplitted[1]),
                    0);
            }
            else
            {
                // Article
                var dateSplitted = dateString.Split(' ');
                var month = (int)Enum.Parse(typeof(Months), dateSplitted[1]);

                return new DateTime(
                    int.Parse(dateSplitted[2]),
                    month,
                    int.Parse(dateSplitted[0]));
            }
        }
    }
}
