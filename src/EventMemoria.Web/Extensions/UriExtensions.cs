using System.Web;

namespace EventMemoria.Web.Extensions;

public static class UriExtensions
{
    public static string Encode(this Uri uri)
        => HttpUtility.UrlEncode(uri.ToString());
}
