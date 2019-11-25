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
    public static string ToBase64Url(this Guid guid)
    {
        Dictionary<string, string> urlFriendly = new Dictionary<string, string>
        {
            { "+", "-" }, // base64will never have - and _ so it's safe to use them
            { "/", "_" }, 
            { "=", "" } // uuid is 128 bit so we don't need = at the end of base64
        };
        var result = Convert.ToBase64String(guid.ToByteArray()).MultipleReplace(urlFriendly);
        return result;
    }

    // reverse transformation
    public static Guid FromBase64Url(this string base64string)
    {
        Dictionary<string, string> urlFriendlyReverse = new Dictionary<string, string>
        {
            { "-", "+" },
            { "_", "/" }
        };

        // guid to base64 string will always be 24 characters exactly
        return new Guid(
            Convert.FromBase64String(base64string
                                        .MultipleReplace(urlFriendlyReverse)
                                        .PadRight(24, '='))
        );
    }
}
