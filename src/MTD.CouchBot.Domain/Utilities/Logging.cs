using System;

namespace MTD.CouchBot.Domain.Utilities
{
    public static class Logging
    {
        private static object _messageLog = new object();

        public static void LogError(string message)
        {
            lock (_messageLog)
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
        }

        public static void LogInfo(string message)
        {
            lock (_messageLog)
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
        }

        public static void LogBeam(string message)
        {
            lock (_messageLog)
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
        }

        public static void LogTwitch(string message)
        {
            lock (_messageLog)
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
        }

        public static void LogYouTube(string message)
        {
            lock (_messageLog)
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

        public static void LogYouTubeGaming(string message)
        {
            lock (_messageLog)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("[" + DateTime.UtcNow + "] - ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("[YouTube Gaming]");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(" " + message);
                Console.ResetColor();
            }
        }

        public static void LogHitbox(string message)
        {
            lock (_messageLog)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("[" + DateTime.UtcNow + "] - ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("[Hitbox]");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(" " + message);
                Console.ResetColor();
            }
        }
    }
}
