using Discord;
using Discord.Commands;
using MTD.CouchBot.Domain;
using MTD.CouchBot.Json;
using MTD.CouchBot.Managers;
using MTD.CouchBot.Managers.Implementations;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MTD.DiscordBot.Modules
{
    [Group("youtube")]
    public class YouTube : ModuleBase
    {
        IYouTubeManager _youTubeManager;

        public YouTube()
        {
            _youTubeManager = new YouTubeManager();
        }
        
        [Command("add")]
        public async Task Add(string channel)
        {
            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }

            if (!channel.ToLower().StartsWith("uc") || channel.Length != 24)
            {
                await Context.Channel.SendMessageAsync("Incorrect YouTube Channel ID Provided. Channel ID's start with UC and have 24 characters.");
                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + user.Guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            if (server.ServerYouTubeChannelIds == null)
                server.ServerYouTubeChannelIds = new List<string>();

            if (!server.ServerYouTubeChannelIds.Contains(channel))
            {
                server.ServerYouTubeChannelIds.Add(channel);
                File.WriteAllText(file, JsonConvert.SerializeObject(server));
                await Context.Channel.SendMessageAsync("Added " + channel + " to the server YouTube streamer list.");
            }
            else
            {
                await Context.Channel.SendMessageAsync(channel + " is already on the server YouTube streamer list.");
            }
        }

        [Command("remove")]
        public async Task Remove(string channel)
        {
            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + user.Guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            if (server.ServerYouTubeChannelIds == null)
                return;

            if (server.ServerYouTubeChannelIds.Contains(channel))
            {
                server.ServerYouTubeChannelIds.Remove(channel);
                File.WriteAllText(file, JsonConvert.SerializeObject(server));
                await Context.Channel.SendMessageAsync("Removed " + channel + " from the server YouTube streamer list.");
            }
            else
            {
                await Context.Channel.SendMessageAsync(channel + " wasn't on the server YouTube streamer list.");
            }
        }

        [Command("owner")]
        public async Task Owner(string channel)
        {
            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }

            if (!channel.ToLower().StartsWith("uc") || channel.Length != 24)
            {
                await Context.Channel.SendMessageAsync("Incorrect YouTube Channel ID Provided. Channel ID's start with UC and have 24 characters.");
                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + user.Guild.Id + ".json";
            var server = new DiscordServer();

            server.OwnerYouTubeChannelId = channel;
            File.WriteAllText(file, JsonConvert.SerializeObject(server));
            await Context.Channel.SendMessageAsync("Owner YouTube Channel ID has been set to " + channel + ".");
        }

        [Command("resetowner")]
        public async Task ResetOwner(string channel)
        {
            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + user.Guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            server.OwnerYouTubeChannelId = null;
            File.WriteAllText(file, JsonConvert.SerializeObject(server));
            await Context.Channel.SendMessageAsync("Owner YouTube Channel ID has been reset.");
        }
    }
}
