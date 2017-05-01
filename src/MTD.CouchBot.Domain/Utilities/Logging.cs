using System;

namespace MTD.CouchBot.Domain.Utilities
{
    public static class Logging
    {
        public static void LogError(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("[" + DateTime.UtcNow + "] - ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("[Error]");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" " + message);
            Console.ResetColor();
        }

        public static void LogInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("[" + DateTime.UtcNow + "] - ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("[Info]");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" " + message);
            Console.ResetColor();
        }

        public static void LogBeam(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("[" + DateTime.UtcNow + "] - ");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("[Beam]");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" " + message);
            Console.ResetColor();
        }

        public static void LogTwitch(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("[" + DateTime.UtcNow + "] - ");
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Write("[Twitch]");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" " + message);
            Console.ResetColor();
        }

        public static void LogYouTube(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("[" + DateTime.UtcNow + "] - ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("[YouTube]");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" " + message);
            Console.ResetColor();
        }

        public static void LogYouTubeGaming(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("[" + DateTime.UtcNow + "] - ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("[YouTube]");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" " + message);
            Console.ResetColor();
        }
    }
}
