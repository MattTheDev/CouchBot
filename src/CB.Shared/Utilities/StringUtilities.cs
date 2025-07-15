using System.Globalization;

namespace CB.Shared.Utilities;

public static class StringUtilities
{
    public static string FirstLetterToUpper(this string str)
    {
        if (str == null)
        {
            return null;
        }

        if (str.Length > 1)
        {
            return char.ToUpper(str[0], CultureInfo.InvariantCulture) + str[1..];
        }

        return str.ToUpper(CultureInfo.InvariantCulture);
    }
}