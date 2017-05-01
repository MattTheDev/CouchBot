using Discord;
using Discord.Commands;
using MTD.CouchBot;
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
            await Context.Channel.SendMessageAsync("Current CouchBot Beam Constellation Connection Status: " + Program.beamClient.Status());
        }

        [Command("add")]
        public async Task Add(string channel)
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
    }
}
