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

namespace MTD.CouchBot.Modules
{
    [Group("streameradd")]
    public class StreamerAdd : ModuleBase
    {
        ITwitchManager twitchManager;
        IBeamManager _beamManager;

        public StreamerAdd()
        {
            twitchManager = new TwitchManager();
            _beamManager = new BeamManager();
        }

        [Command("twitch"), Summary("Add a new twitch streamer.")]
        public async Task Twitch(string channel)
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

            if (server.ServerTwitchChannels == null)
                server.ServerTwitchChannels = new List<string>();

            if (server.ServerTwitchChannelIds == null)
                server.ServerTwitchChannelIds = new List<string>();

            if (!server.ServerTwitchChannels.Contains(channel.ToLower()))
            {
                server.ServerTwitchChannels.Add(channel.ToLower());
                server.ServerTwitchChannelIds.Add(await twitchManager.GetTwitchIdByLogin(channel));
                File.WriteAllText(file, JsonConvert.SerializeObject(server));

                await Context.Channel.SendMessageAsync("Added " + channel + " to the server Twitch streamer list.");
            }
            else
            {
                await Context.Channel.SendMessageAsync(channel + " is already on the server Twitch streamer list.");
            }
        }

        [Command("youtube"), Summary("Add a new youtube streamer.")]
        public async Task Youtube(string channel)
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

        [Command("beam"), Summary("Add a new beam streamer.")]
        public async Task Beam(string channel)
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
                server.ServerBeamChannels = new List<string>();

            if (server.ServerBeamChannelIds == null)
                server.ServerBeamChannelIds = new List<string>();

            if (!server.ServerBeamChannels.Contains(channel.ToLower()))
            {
                var beamChannel = await _beamManager.GetBeamChannelByName(channel);
                server.ServerBeamChannels.Add(channel.ToLower());
                server.ServerBeamChannelIds.Add(beamChannel.id.Value.ToString());
                await Program.beamClient.SubscribeToLiveAnnouncements(beamChannel.id.Value.ToString());
                File.WriteAllText(file, JsonConvert.SerializeObject(server));
                await Context.Channel.SendMessageAsync("Added " + channel + " to the server Beam streamer list.");
            }
            else
            {
                await Context.Channel.SendMessageAsync(channel + " is already on the server Beam streamer list.");
            }
        }

        [Command("hitbox"), Summary("Add a new Hitbox streamer.")]
        public async Task Hitbox(string channel)
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

            if (server.ServerHitboxChannels == null)
                server.ServerHitboxChannels = new List<string>();

            if (!server.ServerHitboxChannels.Contains(channel.ToLower()))
            {
                server.ServerHitboxChannels.Add(channel.ToLower());
                File.WriteAllText(file, JsonConvert.SerializeObject(server));
                await Context.Channel.SendMessageAsync("Added " + channel + " to the server Hitbox streamer list.");
            }
            else
            {
                await Context.Channel.SendMessageAsync(channel + " is already on the server Hitbox streamer list.");
            }
        }
    }
}
