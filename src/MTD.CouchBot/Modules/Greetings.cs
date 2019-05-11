using Discord;
using Discord.Commands;
using Microsoft.Extensions.Options;
using MTD.CouchBot.Domain.Models.Bot;
using MTD.CouchBot.Services;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MTD.CouchBot.Modules
{
    // Create a module with the 'sample' prefix
    [Group("greetings")]
    public class Greetings : BaseModule
    {
        private readonly BotSettings _botSettings;
        private readonly FileService _fileService;

        public Greetings(IOptions<BotSettings> botSettings, FileService fileService) : base(botSettings, fileService)
        {
            _botSettings = botSettings.Value;
            _fileService = fileService;
        }

        [Command("on"), Summary("Turns the greetings on")]
        public async Task On()
        {
            var guild = ((IGuildUser)Context.Message.Author).Guild;

            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }

            var file = _botSettings.DirectorySettings.ConfigRootDirectory + _botSettings.DirectorySettings.GuildDirectory + guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
            {
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));
            }

            server.Greetings = true;
            if(string.IsNullOrEmpty(server.GreetingMessage))
            {
                server.GreetingMessage = "Welcome to the server, %USER%";
                server.GoodbyeMessage = "Good bye, %USER%, thanks for hanging out!";
            }

            await _fileService.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("Greetings have been turned on.");
        }

        [Command("off"), Summary("Turns the greetings off")]
        public async Task Off()
        {
            var guild = ((IGuildUser)Context.Message.Author).Guild;

            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }

            var file = _botSettings.DirectorySettings.ConfigRootDirectory + _botSettings.DirectorySettings.GuildDirectory + guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
            {
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));
            }

            server.Greetings = false;
            await _fileService.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("Greetings have been turned off.");
        }

        [Command("set"), Summary("Sets the greeting message")]
        public async Task Set(string message)
        {
            var guild = ((IGuildUser)Context.Message.Author).Guild;

            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }

            var file = _botSettings.DirectorySettings.ConfigRootDirectory + _botSettings.DirectorySettings.GuildDirectory + guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
            {
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));
            }

            server.GreetingMessage = message;
            await _fileService.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("Greeting has been set.");
        }

        [Command("test")]
        public async Task Test()
        {
            if (!IsAdmin)
            {
                return;
            }

            var guild = new DiscordServer();
            var guildFile = _botSettings.DirectorySettings.ConfigRootDirectory + _botSettings.DirectorySettings.GuildDirectory + Context.Guild.Id + ".json";

            if (File.Exists(guildFile))
            {
                var json = File.ReadAllText(guildFile);
                guild = JsonConvert.DeserializeObject<DiscordServer>(json);
            }

            if (guild != null)
            {
                if (guild.GreetingsChannel != 0)
                {
                    var channel = (IMessageChannel)await Context.Guild.GetChannelAsync(guild.GreetingsChannel);

                    if (string.IsNullOrEmpty(guild.GreetingMessage))
                    {
                        guild.GreetingMessage = "Welcome to the server, %USER%";
                    }

                    var name = "";
                    if (guild.GreetingMessage.Contains("%RANDOMUSER%"))
                    {
                        var users = (await Context.Guild.GetUsersAsync());

                        var random = new Random().Next(0, users.Count - 1);
                        var user = users.ElementAt(random);
                        name = user.Username;
                    }

                    guild.GreetingMessage = guild.GreetingMessage.Replace("%NEWLINE%", "\r\n").Replace("%RANDOMUSER%", name);

                    await channel.SendMessageAsync(guild.GreetingMessage);
                }
            }
        }
    }
}
