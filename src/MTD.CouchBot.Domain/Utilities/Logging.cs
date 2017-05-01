using System;

namespace MTD.CouchBot.Domain.Utilities
{
    public static class Logging
    {
        public static void LogError(string message)
        {
            string prefix = "[" + DateTime.UtcNow + "] ";
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(prefix + message);
            Console.ResetColor();
        }

        public static void LogInfo(string message)
        {
            string prefix = "[" + DateTime.UtcNow + "] ";
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(prefix + message);
            Console.ResetColor();
        }

        public static void LogBeam(string message)
        {
            string prefix = "[" + DateTime.UtcNow + "] ";
            Console.WriteLine(prefix + message);
            Console.ResetColor();
        }

        public static void LogTwitch(string message)
        {
            string prefix = "[" + DateTime.UtcNow + "] ";
            Console.WriteLine(prefix + message);
            Console.ResetColor();
        }

        public static void LogYouTube(string message)
        {
            string prefix = "[" + DateTime.UtcNow + "] ";
            Console.WriteLine(prefix + message);
            Console.ResetColor();
        }
    }
}
