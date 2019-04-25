namespace MTD.CouchBot.Domain.Utilities
{
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
                return char.ToUpper(str[0]) + str.Substring(1);
            }

            return str.ToUpper();
        }


        public static bool IsBoolean(this string str)
        {
            return str.ToLower().Equals("true") || str.ToLower().Equals("false");
        }
    }
}
