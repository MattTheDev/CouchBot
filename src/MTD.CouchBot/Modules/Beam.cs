using Discord;
using Discord.Commands;
using MTD.CouchBot;
using MTD.CouchBot.Bot;
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
    [Group("beam")]
    public class Beam : ModuleBase
    {
        IBeamManager _beamManager;

        public Beam()
        {
            _beamManager = new BeamManager();
        }

        [Command("status")]
        public async Task Status()
        {
            await Context.Channel.SendMessageAsync("Current " + Program.client.CurrentUser.Username + " Beam Constellation Connection Status: " + Program.beamClient.Status());
        }

        [Command("add")]
        public async Task Add(string channelName)
        {
            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }

            var channel = await _beamManager.GetBeamChannelByName(channelName);

            if (channel == null)
            {
                await Context.Channel.SendMessageAsync("The Beam channel, " + channelName + ", does not exist.");
                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + user.Guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            if (server.ServerBeamChannels == null)
                server.ServerBeamChannels = new List<string>();

            if (server.ServerBeamChannelIds == null)
                server.ServerBeamChannelIds = new List<string>();

            if (!string.IsNullOrEmpty(server.OwnerBeamChannel) && server.OwnerBeamChannel.ToLower().Equals(channelName.ToLower()))
            {
                await Context.Channel.SendMessageAsync("The channel " + channelName + " is configured as the Owner Beam channel. " +
                    "Please remove it with the '!cb beam resetowner' command and then try re-adding it.");

                return;
            }

            if (!server.ServerBeamChannels.Contains(channelName.ToLower()))
            {
                server.ServerBeamChannels.Add(channelName.ToLower());
                server.ServerBeamChannelIds.Add(channel.id.Value.ToString());
                await Program.beamClient.SubscribeToLiveAnnouncements(channel.id.Value.ToString());
                File.WriteAllText(file, JsonConvert.SerializeObject(server));
                await Context.Channel.SendMessageAsync("Added " + channelName + " to the server Beam streamer list.");
            }
            else
            {
                await Context.Channel.SendMessageAsync(channelName + " is already on the server Beam streamer list.");
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

            if (server.ServerBeamChannels == null)
                return;

            if (server.ServerBeamChannels.Contains(channel.ToLower()))
            {
                var beamChannel = await _beamManager.GetBeamChannelByName(channel);
                server.ServerBeamChannels.Remove(channel.ToLower());
                server.ServerBeamChannelIds.Remove(beamChannel.id.Value.ToString());
                File.WriteAllText(file, JsonConvert.SerializeObject(server));
                await Context.Channel.SendMessageAsync("Removed " + channel + " from the server Beam streamer list.");
            }
            else
            {
                await Context.Channel.SendMessageAsync(channel + " wasn't on the server Beam streamer list.");
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

            var beamChannel = await _beamManager.GetBeamChannelByName(channel);

            if (beamChannel == null)
            {
                await Context.Channel.SendMessageAsync("Beam Channel " + channel + " does not exist.");

                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + user.Guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            if (server.ServerBeamChannels != null && server.ServerBeamChannels.Contains(channel.ToLower()))
            {
                await Context.Channel.SendMessageAsync("The channel " + channel + " is in the list of server Beam Channels. " +
                    "Please remove it with '!cb beam remove " + channel + "' and then retry setting your owner channel.");

                return;
            }

            server.OwnerBeamChannel = channel;
            server.OwnerBeamChannelId = beamChannel.id.Value.ToString();
            await Program.beamClient.SubscribeToLiveAnnouncements(beamChannel.id.Value.ToString());
            File.WriteAllText(file, JsonConvert.SerializeObject(server));
            await Context.Channel.SendMessageAsync("Owner Beam Channel has been set to " + channel + ".");
        }

        [Command("resetowner")]
        public async Task ResetOwner()
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

            server.OwnerBeamChannel = null;
            server.OwnerBeamChannelId = null;
            File.WriteAllText(file, JsonConvert.SerializeObject(server));
            await Context.Channel.SendMessageAsync("Owner Beam Channel has been reset.");
        }

        [Command("announce")]
        public async Task Announce(string channelName)
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

            var stream = await _beamManager.GetBeamChannelByName(channelName);

            if (stream == null)
            {
                await Context.Channel.SendMessageAsync(channelName + " doesn't exist on Beam.");

                return;
            }

            if (stream.online)
            {
                string gameName = stream.type == null ? "a game" : stream.type.name;
                string url = "http://beam.pro/" + stream.token;
                string avatarUrl = stream.user.avatarUrl != null ? stream.user.avatarUrl : "https://beam.pro/_latest/assets/images/main/avatars/default.jpg";
                string thumbnailUrl = "https://thumbs.beam.pro/channel/" + stream.id + ".small.jpg";
                string channelId = stream.id.Value.ToString();

                var message = await MessagingHelper.BuildMessage(stream.token, gameName, stream.name, url,
                    avatarUrl, thumbnailUrl, Constants.Beam, channelId, server, server.GoLiveChannel);
                await MessagingHelper.SendMessages(Constants.Beam, new List<CouchBot.Models.BroadcastMessage>() { message });
            }
            else
            {
                await Context.Channel.SendMessageAsync(channelName + " is offline.");
            }
        }
    }
}
