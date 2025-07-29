using System.Web;

namespace EveryeyeFeed.Library;

public static class Extensions
{
    public static string Decode(this string input)
    {
        return HttpUtility.HtmlDecode(input);
    }
}