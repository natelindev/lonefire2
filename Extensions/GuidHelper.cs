using System;
using System.Collections.Generic;

public static class GuidHelper
{
    // string to guid
    public static Guid ToGuid(this string value)
    {
        Guid.TryParse(value, out Guid result);
        return result;
    }
    public static bool IsGuid(this string value)
    {
        return Guid.TryParse(value, out _);
    }

    public static bool IsBase64String(this string base64)
    {
        Span<byte> buffer = new Span<byte>(new byte[base64.Length]);
        return Convert.TryFromBase64String(base64, buffer, out _);
    }

    public static bool IsBase64UrlString(this string base64)
    {
        if (base64.Length % 4 == 1)
            return false;

        Dictionary<string, string> urlFriendlyReverse = new Dictionary<string, string>
        {
            { "-", "+" },
            { "_", "/" }
        };

        Span<byte> buffer = new Span<byte>(new byte[base64.Length]);
        return Convert
                .TryFromBase64String(
                base64.MultipleReplace(urlFriendlyReverse)
                      .PadRight(base64.Length + (4 - base64.Length % 4) % 4, '='), buffer, out _);
    }

    public static string MultipleReplace(this string text, Dictionary<string, string> replacements)
    {
        string retVal = text;
        foreach (string textToReplace in replacements.Keys)
        {
            retVal = retVal.Replace(textToReplace, replacements[textToReplace]);
        }
        return retVal;
    }

    // shorten guid with url friendliness and not losing any information
    public static string Base64UrlEncode(this Guid guid)
    {
        Dictionary<string, string> urlFriendly = new Dictionary<string, string>
        {
            { "+", "-" }, // base64will never have - and _ so it's safe to use them
            { "/", "_" }, 
            { "=", "" } // we don't use = at the end of base64 to shorten it
        };
        var result = Convert.ToBase64String(guid.ToByteArray()).MultipleReplace(urlFriendly);
        return result;
    }

    // reverse transformation
    public static Guid Base64UrlDecode(this string base64)
    {
        Dictionary<string, string> urlFriendlyReverse = new Dictionary<string, string>
        {
            { "-", "+" },
            { "_", "/" }
        };

        return new Guid(
            Convert.FromBase64String(base64
                                        .MultipleReplace(urlFriendlyReverse)
                                        .PadRight(base64.Length + (4 - base64.Length % 4) % 4, '='))
        );
    }
}
