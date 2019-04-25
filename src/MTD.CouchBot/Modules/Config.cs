using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using MTD.CouchBot.Domain.Models.Bot;
using MTD.CouchBot.Services;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MTD.CouchBot.Modules
{
    [Group("config")]
    public class Config : BaseModule
    {
        private readonly BotSettings _botSettings;
        private readonly FileService _fileService;
        private readonly DiscordShardedClient _discord;

        public Config(IOptions<BotSettings> botSettings, FileService fileService, DiscordShardedClient discord) 
            : base(botSettings)
        {
            _botSettings = botSettings.Value;
            _fileService = fileService;
            _discord = discord;
        }

        [Command("timezoneoffset"), Summary("Sets servers time zone offset.")]
        public async Task TimeZoneOffset(float offset)
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
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            server.TimeZoneOffset = offset;
            await _fileService.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("Your Server Time Zone Offset has been set.");
        }

        [Command("textannouncements")]
        public async Task TextAnnouncements(string trueFalse)
        {
            var guild = ((IGuildUser)Context.Message.Author).Guild;
            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }

            trueFalse = trueFalse.ToLower();
            if (!trueFalse.Equals("true") && !trueFalse.Equals("false"))
            {
                await Context.Channel.SendMessageAsync("Pass true or false when configuring Text Announcements. (ie: !cb config textannouncements true)");
                return;
            }

            var file = _botSettings.DirectorySettings.ConfigRootDirectory + _botSettings.DirectorySettings.GuildDirectory + guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            server.UseTextAnnouncements = bool.Parse(trueFalse);
            await _fileService.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("Text announcements has been set to: " + trueFalse);
        }

        [Command("list")]
        public async Task List()
        {
            if(!IsAdmin)
            {
                return;
            }

            var server = _fileService.GetConfiguredServerById(Context.Guild.Id);
            var builder = new EmbedBuilder();
            var authorBuilder = new EmbedAuthorBuilder();
            var footerBuilder = new EmbedFooterBuilder();

            authorBuilder.IconUrl = _discord.CurrentUser.GetAvatarUrl();
            authorBuilder.Name = _discord.CurrentUser.Username;
            authorBuilder.Url = "http://mattthedev.codes";

            footerBuilder.IconUrl = _discord.CurrentUser.GetAvatarUrl();
            footerBuilder.Text = $"[CouchBot] - {DateTime.UtcNow.AddHours(server.TimeZoneOffset)}";

            builder.Description = "To list your server configuration, please type one of the following commands:";
            builder.Url = "http://mattthedev.codes";

            builder.Author = authorBuilder;
            builder.Footer = footerBuilder;

            builder.AddField(new EmbedFieldBuilder()
            {
                Name = "Commands",
                Value = "!cb config list allows\r\n" +
                        "!cb config list channels\r\n" +
                        "!cb config list messages\r\n" +
                        "!cb config list misc\r\n",
                IsInline = false
            });

            await Context.Channel.SendMessageAsync("", false, builder.Build());
        }

        [Command("list")]
        public async Task List(string section)
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
            authorBuilder.Url = "http://mattthedev.codes";

            footerBuilder.IconUrl = _discord.CurrentUser.GetAvatarUrl();
            footerBuilder.Text = $"[CouchBot] - {DateTime.UtcNow.AddHours(server.TimeZoneOffset)}";

            builder.Url = "http://mattthedev.codes";

            builder.Author = authorBuilder;
            builder.Footer = footerBuilder;

            var message = "";
            var error = "";

            switch (section.ToLower())
            {
                case "allows":
                    message += _fileService.GetAllowsByServerId(Context.Guild.Id);
                    break;
                case "channels":
                    message += _fileService.GetChannelsByServerId(Context.Guild.Id);
                    break;
                case "messages":
                    message += _fileService.GetMessagesByServerId(Context.Guild.Id);
                    break;
                case "misc":
                    message += await _fileService.GetMiscConfigByServerId(Context.Guild.Id);
                    break;
                default:
                    error = "Please pass in a valid section. Valid sections are: allows, channels, messages, and misc.";
                    break;
            }

            if(!string.IsNullOrEmpty(error))
            {
                builder.AddField(new EmbedFieldBuilder()
                {
                    Name = "Error!",
                    Value = error,
                    IsInline = false
                });

                return;
            }

            //message += "```";

            builder.AddField(new EmbedFieldBuilder()
            {
                Name = server.Name + " Configuration Settings",
                Value = message,
                IsInline = false
            });

            await Context.Channel.SendMessageAsync("", false, builder.Build());
        }

        [Command("publishedytg"), Summary("Sets www vs gaming in published content urls.")]
        public async Task PublishedYtg(string trueFalse)
        {
            var guild = ((IGuildUser)Context.Message.Author).Guild;
            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }

            trueFalse = trueFalse.ToLower();
            if (!trueFalse.Equals("true") && !trueFalse.Equals("false"))
            {
                await Context.Channel.SendMessageAsync("Pass true or false when configuring PublishedYTG. (ie: !cb config publishedytg true)");
                return;
            }

            var file = _botSettings.DirectorySettings.ConfigRootDirectory + _botSettings.DirectorySettings.GuildDirectory + guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            server.UseYouTubeGamingPublished = bool.Parse(trueFalse);
            await _fileService.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("Publised YTG has been set to: " + trueFalse);
        }

        [Command("deleteoffline"), Summary("Do you want items to be deleted when you go offline?")]
        public async Task DeleteWhenOffline(string trueFalse)
        {
            var guild = ((IGuildUser)Context.Message.Author).Guild;
            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }

            trueFalse = trueFalse.ToLower();
            if (!trueFalse.Equals("true") && !trueFalse.Equals("false"))
            {
                await Context.Channel.SendMessageAsync("Pass true or false when configuring DeleteOffline. (ie: !cb config deleteoffline true)");
                return;
            }

            var file = _botSettings.DirectorySettings.ConfigRootDirectory + _botSettings.DirectorySettings.GuildDirectory + guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            server.DeleteWhenOffline = bool.Parse(trueFalse);
            await _fileService.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("Delete Offline has been set to: " + trueFalse);
        }

        [Command("mentionrole"), Summary("Set the role to mention instead of Everyone.")]
        public async Task MentionRole(string role)
        {
            var guild = ((IGuildUser)Context.Message.Author).Guild;
            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }

            if(!role.ToLower().Contains("here"))
            {
                return;
            }

            var file = _botSettings.DirectorySettings.ConfigRootDirectory + _botSettings.DirectorySettings.GuildDirectory + guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            server.MentionRole = 1;

            await _fileService.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("Mention Role has been set to: " + role);
        }
        
        [Command("mentionrole"), Summary("Set the role to mention instead of Everyone.")]
        public async Task MentionRole(IRole role)
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
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            server.MentionRole = role.Name.ToLower().Contains("everyone") ? 0 : role.Id;

            await _fileService.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("Mention Role has been set to: " + role.Name);
        }
    }

}
