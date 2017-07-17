using Discord.Commands;
using MTD.CouchBot;
using MTD.CouchBot.Bot;
using MTD.CouchBot.Domain;
using MTD.CouchBot.Domain.Models.Bot;
using MTD.CouchBot.Domain.Utilities;
using MTD.CouchBot.Managers;
using MTD.CouchBot.Managers.Implementations;
using MTD.CouchBot.Models.Bot;
using MTD.CouchBot.Modules;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MTD.DiscordBot.Modules
{
    [Group("mixer")]
    public class Mixer : BaseModule
    {
        readonly IMixerManager _mixerManager;

        public Mixer()
        {
            _mixerManager = new MixerManager();
        }

        [Command("status")]
        public async Task Status()
        {
            await Context.Channel.SendMessageAsync("Current " + Program.client.CurrentUser.Username + " Mixer Constellation Connection Status: " + Program.beamClient.Status());
        }

        [Command("add")]
        public async Task Add(string channelName)
        {
            if (!IsApprovedAdmin)
            {
                return;
            }

            var channel = await _mixerManager.GetChannelByName(channelName);

            if (channel == null)
            {
                await Context.Channel.SendMessageAsync("The Mixer channel, " + channelName + ", does not exist.");
                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + Context.Guild.Id + ".json";
            DiscordServer server = null;

            if (File.Exists(file))
            {
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));
            }

            if (server == null)
            {
                return;
            }

            if (server.ServerBeamChannels == null)
            {
                server.ServerBeamChannels = new List<string>();
            }

            if (server.ServerBeamChannelIds == null)
            {
                server.ServerBeamChannelIds = new List<string>();
            }

            if (!string.IsNullOrEmpty(server.OwnerBeamChannel) && server.OwnerBeamChannel.ToLower().Equals(channelName.ToLower()))
            {
                await Context.Channel.SendMessageAsync("The channel " + channelName + " is configured as the Owner Mixer channel. " +
                    "Please remove it with the '!cb mixer resetowner' command and then try re-adding it.");

                return;
            }

            if (!server.ServerBeamChannels.Contains(channelName.ToLower()))
            {
                server.ServerBeamChannels.Add(channelName.ToLower());
                server.ServerBeamChannelIds.Add(channel.id.Value.ToString());

                if (Constants.EnableMixer)
                {
                    await Program.beamClient.SubscribeToLiveAnnouncements(channel.id.Value.ToString());

                    if(channel.online)
                    {
                        await Announce(channel.token);
                    }
                }

                await BotFiles.SaveDiscordServer(server, Context.Guild);
                await Context.Channel.SendMessageAsync("Added " + channelName + " to the server Mixer streamer list.");
            }
            else
            {
                await Context.Channel.SendMessageAsync(channelName + " is already on the server Mixer streamer list.");
            }
        }

        [Command("remove")]
        public async Task Remove(string channel)
        {
            if (!IsApprovedAdmin)
            {
                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + Context.Guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
            {
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));
            }

            if (server.ServerBeamChannels == null)
            {
                return;
            }

            if (server.ServerBeamChannels.Contains(channel.ToLower()))
            {
                var beamChannel = await _mixerManager.GetChannelByName(channel);
                server.ServerBeamChannels.Remove(channel.ToLower());
                server.ServerBeamChannelIds.Remove(beamChannel.id.Value.ToString());
                await BotFiles.SaveDiscordServer(server, Context.Guild);
                await Context.Channel.SendMessageAsync("Removed " + channel + " from the server Mixer streamer list.");
            }
            else
            {
                await Context.Channel.SendMessageAsync(channel + " wasn't on the server Mixer streamer list.");
            }
        }

        [Command("owner")]
        public async Task Owner(string channel)
        {
            if (!IsAdmin)
            {
                return;
            }

            var beamChannel = await _mixerManager.GetChannelByName(channel);

            if (beamChannel == null)
            {
                await Context.Channel.SendMessageAsync("Mixer Channel " + channel + " does not exist.");

                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + Context.Guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
            {
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));
            }

            if (server.ServerBeamChannels != null && server.ServerBeamChannels.Contains(channel.ToLower()))
            {
                await Context.Channel.SendMessageAsync("The channel " + channel + " is in the list of server Mixer Channels. " +
                    "Please remove it with '!cb mixer remove " + channel + "' and then retry setting your owner channel.");

                return;
            }

            server.OwnerBeamChannel = channel;
            server.OwnerBeamChannelId = beamChannel.id.Value.ToString();
            await Program.beamClient.SubscribeToLiveAnnouncements(beamChannel.id.Value.ToString());
            await BotFiles.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("Owner Mixer Channel has been set to " + channel + ".");
        }

        [Command("resetowner")]
        public async Task ResetOwner()
        {
            if (!IsAdmin)
            {
                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + Context.Guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
            {
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));
            }

            server.OwnerBeamChannel = null;
            server.OwnerBeamChannelId = null;
            await BotFiles.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("Owner Mixer Channel has been reset.");
        }

        [Command("announce")]
        public async Task Announce(string channelName)
        {
            if (!IsAdmin)
            {
                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + Context.Guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
            {
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));
            }

            var stream = await _mixerManager.GetChannelByName(channelName);

            if (stream == null)
            {
                await Context.Channel.SendMessageAsync(channelName + " doesn't exist on Mixer.");

                return;
            }

            if (stream.online)
            {
                string gameName = stream.type == null ? "a game" : stream.type.name;
                string url = "http://mixer.com/" + stream.token;
                string avatarUrl = stream.user.avatarUrl != null ? stream.user.avatarUrl : "https://mixer.com/_latest/assets/images/main/avatars/default.jpg";
                string thumbnailUrl = "https://thumbs.mixer.com/channel/" + stream.id + ".small.jpg";
                string channelId = stream.id.Value.ToString();

                var message = await MessagingHelper.BuildMessage(stream.token, gameName, stream.name, url,
                    avatarUrl, thumbnailUrl, Constants.Mixer, channelId, server, server.GoLiveChannel, null);
                await MessagingHelper.SendMessages(Constants.Mixer, new List<BroadcastMessage>() { message });
            }
            else
            {
                await Context.Channel.SendMessageAsync(channelName + " is offline.");
            }
        }
    }
}
