using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using MTD.CouchBot.Domain;
using MTD.CouchBot.Domain.Models.Bot;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MTD.CouchBot.Services
{
    public class LoggingService
    {
        private static object _messageLog = new object();

        private readonly DiscordShardedClient _discord;
        private readonly CommandService _commands;
        private readonly BotSettings _botSettings;

        private string _logDirectory { get; }
        private string _logFile => Path.Combine(_logDirectory, $"{DateTime.UtcNow.ToString("yyyy-MM-dd")}.txt");

        public LoggingService(DiscordShardedClient discord, CommandService commands, IOptions<BotSettings> botSettings)
        {
            _logDirectory = Path.Combine(AppContext.BaseDirectory, "logs");

            _discord = discord;
            _commands = commands;
            _botSettings = botSettings.Value;

            _discord.Log += OnLogAsync;
            _commands.Log += OnLogAsync;
        }

        public async Task LogError(string message)
        {
            var guildChannel = _discord.GetChannel(_botSettings.BotConfig.DiscordErrorChannelId);

            await ((IMessageChannel)guildChannel).SendMessageAsync($"[Error] [{DateTime.UtcNow}] - {message}");
        }

        public async Task LogAudit(string message)
        {
            var guildChannel = _discord.GetChannel(_botSettings.BotConfig.DiscordErrorChannelId);

            await ((IMessageChannel)guildChannel).SendMessageAsync($"[Audit] [{DateTime.UtcNow}] - {message}");
        }

        private Task OnLogAsync(LogMessage msg)
        {
            if (!Directory.Exists(_logDirectory))     // Create the log directory if it doesn't exist
                Directory.CreateDirectory(_logDirectory);
            if (!File.Exists(_logFile))               // Create today's log file if it doesn't exist
                File.Create(_logFile).Dispose();

            string logText = $"{DateTime.UtcNow.ToString("hh:mm:ss")} [{msg.Severity}] {msg.Source}: {msg.Exception?.ToString() ?? msg.Message}";
            File.AppendAllText(_logFile, logText + "\n");     // Write the log text to a file

            return Console.Out.WriteLineAsync(logText);       // Write the log text to the console
        }

        /**** LEGACY - NEEDS TO BE REMOVED ****/
        public void LogInfo(string message)
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

        public void LogMixer(string message)
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

        public void LogTwitch(string message)
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

        public void LogYouTube(string message)
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

        public void LogYouTubeGaming(string message)
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

        public void LogSmashcast(string message)
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

        public void LogPicarto(string message)
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

        public void LogPiczel(string message)
        {
            lock (_messageLog)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("[" + DateTime.UtcNow + "] - ");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("[" + Constants.Piczel + "]");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(" " + message);
                Console.ResetColor();
            }
        }

        public void LogMobcrush(string message)
        {
            lock (_messageLog)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("[" + DateTime.UtcNow + "] - ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("[" + Constants.Mobcrush + "]");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(" " + message);
                Console.ResetColor();
            }
        }
    }
}
