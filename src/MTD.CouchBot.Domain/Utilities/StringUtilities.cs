namespace MTD.CouchBot.Domain.Utilities
{
    public static class StringUtilities
    {
        public static string ScrubChatMessage(string message)
        {
            return message.Replace("_", "\\_").Replace("*", "\\*");
        }
    }
}
