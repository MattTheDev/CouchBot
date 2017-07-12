using Discord;
using Discord.Commands;
using MTD.CouchBot.Domain;
using MTD.CouchBot.Domain.Models.Bot;
using MTD.CouchBot.Domain.Utilities;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace MTD.CouchBot.Modules
{
    [Group("channel"), Summary("Subset of Commands to configure server channels.")]
    public class Channel : ModuleBase
    {
        [Command("announce"), Summary("Sets the server announcement channel.")]
        public async Task Announce(IGuildChannel guildChannel)
        {
            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + guildChannel.Guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            server.AnnouncementsChannel = guildChannel.Id;
            await BotFiles.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("The Announce Channel has been set.");
        }

        [Command("live"), Summary("Sets go live channel.")]
        public async Task Live(IGuildChannel guildChannel)
        {
            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + guildChannel.Guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            server.GoLiveChannel = guildChannel.Id;
            await BotFiles.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("The Live Channel has been set.");
        }

        [Command("ownerlive"), Summary("Sets owner live channel.")]
        public async Task OwnerLive(IGuildChannel guildChannel)
        {
            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + guildChannel.Guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            server.OwnerLiveChannel = guildChannel.Id;
            await BotFiles.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("The Owner Live Channel has been set.");
        }

        [Command("greetings"), Summary("Sets greetings channel.")]
        public async Task Greetings(IGuildChannel guildChannel)
        {
            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + guildChannel.Guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            server.GreetingsChannel = guildChannel.Id;
            await BotFiles.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("The Greetings Channel has been set.");
        }

        [Command("published"), Summary("Sets published video channel.")]
        public async Task Published(IGuildChannel guildChannel)
        {
            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + guildChannel.Guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            server.PublishedChannel = guildChannel.Id;
            await BotFiles.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("The Published Channel has been set.");
        }

        [Command("ownerpublished"), Summary("Sets owner published video channel.")]
        public async Task OwnerPublished(IGuildChannel guildChannel)
        {
            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + guildChannel.Guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            server.OwnerPublishedChannel = guildChannel.Id;
            await BotFiles.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("The Owner Published Channel has been set.");
        }

        [Command("ownertwitchfeed"), Summary("Sets owner twitch channel feed channel.")]
        public async Task OwnerTwitchFeedChannel(IGuildChannel guildChannel)
        {
            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + guildChannel.Guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            server.OwnerTwitchFeedChannel = guildChannel.Id;
            await BotFiles.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("The Owner Twitch Channel Feed Channel has been set.");
        }

        [Command("twitchfeed"), Summary("Sets twitch channel feed channel.")]
        public async Task TwitchFeedChannel(IGuildChannel guildChannel)
        {
            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + guildChannel.Guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            server.TwitchFeedChannel = guildChannel.Id;
            await BotFiles.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("The Twitch Channel Feed Channel has been set.");
        }

        [Command("clear"), Summary("Clears channels settings for a guild.")]
        public async Task Clear(string option)
        {
            var guild = ((IGuildUser)Context.Message.Author).Guild;
            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            if (File.Exists(file))
            {
                option = option.ToLower();
                string label = "";

                switch (option)
                {
                    case "live":
                        server.GoLiveChannel = 0;
                        label = "Live Channel";
                        break;
                    case "ownerlive":
                        server.OwnerLiveChannel = 0;
                        label = "Owner Live Channel";
                        break;
                    case "announce":
                        server.AnnouncementsChannel = 0;
                        label = "Announcements";
                        break;
                    case "greetings":
                        server.GreetingsChannel = 0;
                        label = "Greetings";
                        break;
                    case "ownerpublished":
                        server.OwnerPublishedChannel = 0;
                        label = "Owner Published";
                        break;
                    case "published":
                        server.PublishedChannel = 0;
                        label = "Published";
                        break;
                    case "ownertwitchfeed":
                        server.OwnerTwitchFeedChannel = 0;
                        label = "Owner Twitch Feed";
                        break;
                    case "twitchfeed":
                        server.TwitchFeedChannel = 0;
                        label = "Twitch Feed";
                        break;
                    case "all":
                        server.AnnouncementsChannel = 0;
                        server.GoLiveChannel = 0;
                        server.GreetingsChannel = 0;
                        server.PublishedChannel = 0;
                        server.OwnerPublishedChannel = 0;
                        server.OwnerLiveChannel = 0;
                        server.OwnerTwitchFeedChannel = 0;
                        server.TwitchFeedChannel = 0;
                        label = "All";
                        break;
                    default:
                        break;
                }

                if (!string.IsNullOrEmpty(label))
                {
                    await BotFiles.SaveDiscordServer(server, Context.Guild);
                    await Context.Channel.SendMessageAsync(label + " settings have been reset.");
                }
            }
        }
    }
}
