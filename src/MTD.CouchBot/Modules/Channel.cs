using Discord;
using Discord.Commands;
using MTD.CouchBot.Domain;
using MTD.CouchBot.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            File.WriteAllText(file, JsonConvert.SerializeObject(server));
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
            File.WriteAllText(file, JsonConvert.SerializeObject(server));
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
            File.WriteAllText(file, JsonConvert.SerializeObject(server));
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
            File.WriteAllText(file, JsonConvert.SerializeObject(server));
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
            File.WriteAllText(file, JsonConvert.SerializeObject(server));
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
            File.WriteAllText(file, JsonConvert.SerializeObject(server));
            await Context.Channel.SendMessageAsync("The Owner Published Channel has been set.");
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
                    case "golivechannel":
                        server.GoLiveChannel = 0;
                        label = "Go Live Channel";
                        break;
                    case "announcechannel":
                        server.AnnouncementsChannel = 0;
                        label = "Announcements";
                        break;
                    case "greetingschannel":
                        server.GreetingsChannel = 0;
                        label = "Greetings";
                        break;
                    case "publishedchannel":
                        server.PublishedChannel = 0;
                        label = "Published";
                        break;
                    case "all":
                        server.AnnouncementsChannel = 0;
                        server.GoLiveChannel = 0;
                        server.GreetingsChannel = 0;
                        server.PublishedChannel = 0;
                        label = "All";
                        break;
                    default:
                        break;
                }

                if (!string.IsNullOrEmpty(label))
                {
                    File.WriteAllText(file, JsonConvert.SerializeObject(server));
                    await Context.Channel.SendMessageAsync(label + " settings have been reset.");
                }
            }
        }
    }
}
