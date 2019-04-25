using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using MTD.CouchBot.Domain.Models.Bot;
using MTD.CouchBot.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTD.CouchBot.Modules
{
    [Group("command")]
    public class Command : BaseModule
    {
        private readonly BotSettings _botSettings;
        private readonly FileService _fileService;
        private readonly DiscordShardedClient _discord;

        public Command(IOptions<BotSettings> botSettings, FileService fileService, DiscordShardedClient discord) : base(botSettings)
        {
            _botSettings = botSettings.Value;
            _fileService = fileService;
            _discord = discord;
        }

        [Command("add")]
        public async Task Add(string command, int cooldown, string output)
        {
            if(!IsAdmin)
            {
                return;
            }

            var file = $"{_botSettings.DirectorySettings.ConfigRootDirectory}{_botSettings.DirectorySettings.GuildDirectory}{Context.Guild.Id}.json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            if(server.CustomCommands == null)
            {
                server.CustomCommands = new List<CustomCommand>();
            }

            server.CustomCommands.Add(new CustomCommand() { Command = command, Output = output, Cooldown = cooldown,
                Repeat = false, Interval = 0, LastRun = DateTime.UtcNow });

            await _fileService.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync($"New command {command} has been created with a {cooldown} second cooldown.");
        }

        [Command("add")]
        public async Task Add(string command, int cooldown, string output, bool repeat, int interval, Discord.IGuildChannel channel)
        {
            if (!IsAdmin)
            {
                return;
            }

            var file = $"{_botSettings.DirectorySettings.ConfigRootDirectory}{_botSettings.DirectorySettings.GuildDirectory}{Context.Guild.Id}.json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            if (server.CustomCommands == null)
            {
                server.CustomCommands = new List<CustomCommand>();
            }

            server.CustomCommands.Add(new CustomCommand() { Command = command, Output = output, Cooldown = cooldown,
            Repeat = repeat, Interval = interval, LastRun = DateTime.UtcNow, ChannelId = channel.Id});

            await _fileService.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync($"New command {command} has been created with a {cooldown} second cooldown.");
        }

        [Command("remove")]
        public async Task Remove(string command)
        {
            if (!IsAdmin)
            {
                return;
            }

            var file = $"{_botSettings.DirectorySettings.ConfigRootDirectory}{_botSettings.DirectorySettings.GuildDirectory}{Context.Guild.Id}.json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            var c = server.CustomCommands.FirstOrDefault(cc => cc.Command.Equals(command, StringComparison.CurrentCultureIgnoreCase));
            server.CustomCommands.Remove(c);

            await _fileService.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync($"Command {command} has been removed.");
        }

        [Command("list")]
        public async Task List()
        {
            if (!IsAdmin)
            {
                return;
            }

            var server = _fileService.GetConfiguredServerById(Context.Guild.Id);

            var builder = new EmbedBuilder();
            var authorBuilder = new EmbedAuthorBuilder();
            var footerBuilder = new EmbedFooterBuilder();

            authorBuilder.IconUrl = _discord.CurrentUser.GetAvatarUrl();
            authorBuilder.Name = _discord.CurrentUser.Username;
            authorBuilder.Url = "http://couchbot.io";

            footerBuilder.IconUrl = _discord.CurrentUser.GetAvatarUrl();
            footerBuilder.Text = $"[CouchBot] - {DateTime.UtcNow.AddHours(server.TimeZoneOffset)}";

            builder.Description = "This server has the following custom commands:";
            builder.Url = "http://couchbot.io";

            builder.Author = authorBuilder;
            builder.Footer = footerBuilder;

            var commandBuilder = new StringBuilder();
            var outputBuilder = new StringBuilder();
            var cooldownBuilder = new StringBuilder();
            
            if (server.CustomCommands != null)
            {
                foreach (var cc in server.CustomCommands)
                {
                    commandBuilder.AppendLine(cc.Command);
                    outputBuilder.AppendLine(cc.Output);
                    cooldownBuilder.AppendLine($"{cc.Cooldown}");
                }
            }
            else
            {
                commandBuilder.AppendLine("None. :(");
            }

            builder.AddField(new EmbedFieldBuilder()
            {
                Name = "Commands",
                Value = commandBuilder.ToString(),
                IsInline = true
            });

            //builder.AddField(new EmbedFieldBuilder()
            //{
            //    Name = "Output",
            //    Value = outputBuilder.ToString(),
            //    IsInline = true
            //});

            //builder.AddField(new EmbedFieldBuilder()
            //{
            //    Name = "Cooldown",
            //    Value = cooldownBuilder.ToString(),
            //    IsInline = true
            //});

            await Context.Channel.SendMessageAsync("", false, builder.Build());
 
        }
    }
}
