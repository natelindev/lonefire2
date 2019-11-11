using System;

public static class ExtensionMethods
{
    public static Guid ToGuid(this string value)
    {
        Guid.TryParse(value, out Guid result);
        return result;
    }
}
