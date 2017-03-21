using System;

namespace MTD.CouchBot.Domain.Utilities
{
    public static class Logging
    {
        private static string prefix = "[" + DateTime.UtcNow + "] ";

        public static void LogError(string message)
        {
            //Console.BackgroundColor = ConsoleColor.Red;
            //Console.ForegroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(prefix + message);
            Console.ResetColor();
        }

        public static void LogInfo(string message)
        {
            //Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(prefix + message);
            Console.ResetColor();
        }

        public static void LogBeam(string message)
        {
            //Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(prefix + message);
            Console.ResetColor();
        }

        public static void LogTwitch(string message)
        {
            //Console.ForegroundColor = ConsoleColor.Magenta;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(prefix + message);
            Console.ResetColor();
        }

        public static void LogYouTube(string message)
        {
            //Console.ForegroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(prefix + message);
            Console.ResetColor();
        }
    }
}
