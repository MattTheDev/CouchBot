using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using MTD.CouchBot.Domain.Exceptions;

namespace MTD.CouchBot.Services
{
    public class StartupService
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private readonly IConfiguration _config;

        public StartupService(
            DiscordSocketClient discord,
            CommandService commands,
            IConfiguration config)
        {
            _config = config;
            _discord = discord;
            _commands = commands;
        }

        public async Task StartAsync()
        {
            var discordToken = _config["Keys:DiscordBotToken"];
            if (string.IsNullOrWhiteSpace(discordToken))
            {
                throw new ConfigurationException("Please enter your bot's token into the `BotSettings.json` file found in the applications root directory.");
            }

            await _discord.LoginAsync(TokenType.Bot, discordToken);
            await _discord.StartAsync();

            var commandAssembly = Assembly.Load("MTD.CouchBot.Commands");

            await _commands.AddModulesAsync(commandAssembly);
        }

        public void CreateRequiredDirectories()
        {
            var root = _config["Directories:BotDataRoot"];
            var guildConfig = $"{root}{_config["Directories:GuildConfigurations"]}";
            var liveLocks = $"{root}{_config["Directories:LiveLocks"]}";
            var logs = $"{root}{_config["Directories:Logs"]}";

            if (!Directory.Exists(root))
            {
                Directory.CreateDirectory(root);
            }

            if (!Directory.Exists(guildConfig))
            {
                Directory.CreateDirectory(guildConfig);
            }

            if (!Directory.Exists(liveLocks))
            {
                Directory.CreateDirectory(liveLocks);
            }

            if (!Directory.Exists(logs))
            {
                Directory.CreateDirectory(logs);
            }
        }
    }
}