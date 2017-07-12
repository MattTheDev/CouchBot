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
    [Group("allow"), Summary("Subset of Commands to configure server settings.")]
    public class Allow : ModuleBase
    {
        [Command("mention"), Summary("Sets use of a mention tag.")]
        public async Task Mention(string trueFalse)
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
                await Context.Channel.SendMessageAsync("Pass true or false when configuring AllowEveryone. (ie: !cb config AllowEveryone true)");
                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            server.AllowEveryone = bool.Parse(trueFalse);
            await BotFiles.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("Allow everyone has been set to: " + trueFalse);
        }

        [Command("thumbnails"), Summary("Sets use of thumbnails.")]
        public async Task Thumbnails(string trueFalse)
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
                await Context.Channel.SendMessageAsync("Pass true or false when configuring AllowThumbnails. (ie: !cb config AllowThumbnails true)");
                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            server.AllowThumbnails = bool.Parse(trueFalse);
            await BotFiles.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("Allow thumbnails has been set to: " + trueFalse);
        }

        [Command("live"), Summary("Sets announcing of published content.")]
        public async Task Live(string trueFalse)
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
                await Context.Channel.SendMessageAsync("Pass true or false when configuring allow live. (ie: !cb allow live true)");
                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            server.AllowLive = bool.Parse(trueFalse);
            await BotFiles.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("Allow live has been set to: " + trueFalse);
        }

        [Command("published"), Summary("Sets announcing of published content.")]
        public async Task Published(string trueFalse)
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
                await Context.Channel.SendMessageAsync("Pass true or false when configuring allow published. (ie: !cb allow published true)");
                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            server.AllowPublished = bool.Parse(trueFalse);
            await BotFiles.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("Allow published has been set to: " + trueFalse);
        }

        [Command("goals"), Summary("Sets broadcasting of sub goals being met.")]
        public async Task Goals(string trueFalse)
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
                await Context.Channel.SendMessageAsync("Pass true or false when configuring BroadcastSubGoals. (ie: !cb config BroadcastSubGoals true)");
                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            server.BroadcastSubGoals = bool.Parse(trueFalse);
            await BotFiles.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("Allow sub goals has been set to: " + trueFalse);
        }

        [Command("channelfeed"), Summary("Sets announcing of channel feed.")]
        public async Task ChannelFeed(string trueFalse)
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
                await Context.Channel.SendMessageAsync("Pass true or false when configuring allow channel feed. (ie: !cb allow channelfeed true)");
                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            server.AllowChannelFeed = bool.Parse(trueFalse);
            await BotFiles.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("Allow channel feed has been set to: " + trueFalse);
        }

        [Command("ownerchannelfeed"), Summary("Sets announcing of owner channel feed.")]
        public async Task ChannelFeedOwner(string trueFalse)
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
                await Context.Channel.SendMessageAsync("Pass true or false when configuring allow owner channel feed. (ie: !cb allow ownerchannelfeed true)");
                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            server.AllowOwnerChannelFeed = bool.Parse(trueFalse);
            await BotFiles.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("Allow owner channel feed has been set to: " + trueFalse);
        }
    }
}
