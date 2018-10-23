using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MTD.CouchBot.Services
{
    public class LoggingService
    {
        private readonly IConfiguration _configuration;

        private string LogDirectory { get; }
        private string LogFile => Path.Combine(LogDirectory, $"{DateTime.UtcNow:yyyy-MM-dd}.txt");
        private string ErrorLogFile => Path.Combine(LogDirectory, $"{DateTime.UtcNow:yyyy-MM-dd}_Errors.txt");

        public LoggingService(DiscordSocketClient discord, CommandService commands, IConfiguration configuration)
        {
            _configuration = configuration;
            LogDirectory = $"{_configuration["Directories:BotDataRoot"]}{_configuration["Directories:Logs"]}";

            discord.Log += OnLogAsync;
            commands.Log += OnLogAsync;
        }

        private Task OnLogAsync(LogMessage msg)
        {
            var logText = $"{DateTime.UtcNow:hh:mm:ss} [{msg.Severity}] {msg.Source}: {msg.Exception?.ToString() ?? msg.Message}";
            File.AppendAllText(LogFile, $"{logText}\r\n");

            return Console.Out.WriteLineAsync(logText);
        }

    }
}