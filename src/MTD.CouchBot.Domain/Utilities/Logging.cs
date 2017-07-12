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

        public static void LogMixer(string message)
        {
            lock (_messageLog)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("[" + DateTime.UtcNow + "] - ");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("[" + Constants.Mixer + "]");
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
                Console.Write("[" + Constants.Twitch + "]");
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
                Console.Write("[" + Constants.YouTube + "]");
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
                Console.Write("[" + Constants.YouTubeGaming + "]");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(" " + message);
                Console.ResetColor();
            }
        }

        public static void LogVidMe(string message)
        {
            lock (_messageLog)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("[" + DateTime.UtcNow + "] - ");
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write("[" + Constants.VidMe + "]");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(" " + message);
                Console.ResetColor();
            }
        }

        public static void LogSmashcast(string message)
        {
            lock (_messageLog)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("[" + DateTime.UtcNow + "] - ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("[" + Constants.Smashcast + "]");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(" " + message);
                Console.ResetColor();
            }
        }

        public static void LogPicarto(string message)
        {
            lock (_messageLog)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("[" + DateTime.UtcNow + "] - ");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("[" + Constants.Picarto + "]");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(" " + message);
                Console.ResetColor();
            }
        }
    }
}
