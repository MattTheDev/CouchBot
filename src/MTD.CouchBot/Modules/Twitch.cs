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
    [Group("twitch")]
    public class Twitch : ModuleBase
    {
        ITwitchManager _twitchManager;

        public Twitch()
        {
            _twitchManager = new TwitchManager();
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

            if (server.ServerTwitchChannels == null)
                server.ServerTwitchChannels = new List<string>();

            if (server.ServerTwitchChannelIds == null)
                server.ServerTwitchChannelIds = new List<string>();

            if (server.OwnerHitboxChannel.ToLower().Equals(channel.ToLower()))
            {
                await Context.Channel.SendMessageAsync("The channel " + channel + " is configured as the Owner Twitch channel. " +
                    "Please remove it with the '!cb twitch resetowner' command and then try re-adding it.");

                return;
            }

            if (!server.ServerTwitchChannels.Contains(channel.ToLower()))
            {
                server.ServerTwitchChannels.Add(channel.ToLower());
                server.ServerTwitchChannelIds.Add(await _twitchManager.GetTwitchIdByLogin(channel));
                File.WriteAllText(file, JsonConvert.SerializeObject(server));

                await Context.Channel.SendMessageAsync("Added " + channel + " to the server Twitch streamer list.");
            }
            else
            {
                await Context.Channel.SendMessageAsync(channel + " is already on the server Twitch streamer list.");
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

            if (server.ServerTwitchChannels == null)
                return;

            if (server.ServerTwitchChannels.Contains(channel.ToLower()))
            {
                var twitchId = await _twitchManager.GetTwitchIdByLogin(channel);

                server.ServerTwitchChannels.Remove(channel.ToLower());
                server.ServerTwitchChannelIds.Remove(twitchId);
                File.WriteAllText(file, JsonConvert.SerializeObject(server));

                await Context.Channel.SendMessageAsync("Removed " + channel + " from the server Twitch streamer list.");
            }
            else
            {
                await Context.Channel.SendMessageAsync(channel + " wasn't on the server Twitch streamer list.");
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

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + user.Guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            var twitchChannelId = await _twitchManager.GetTwitchIdByLogin(channel);

            if (string.IsNullOrEmpty(twitchChannelId))
            {
                await Context.Channel.SendMessageAsync("Twitch Channel " + channel + " does not exist.");
            }
            else
            {
                if (server.ServerTwitchChannels.Contains(channel.ToLower()))
                {
                    await Context.Channel.SendMessageAsync("The channel " + channel + " is in the list of server Twitch Channels. " +
                        "Please remove it with '!cb twitch remove " + channel + "' and then retry setting your owner channel.");

                    return;
                }

                server.OwnerTwitchChannel = channel;
                server.OwnerTwitchChannelId = twitchChannelId;
                File.WriteAllText(file, JsonConvert.SerializeObject(server));
                await Context.Channel.SendMessageAsync("Owner Twitch Channel has been set to " + channel + ".");
            }
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

            server.OwnerTwitchChannel = null;
            server.OwnerTwitchChannelId = null;
            File.WriteAllText(file, JsonConvert.SerializeObject(server));
            await Context.Channel.SendMessageAsync("Owner Twitch Channel has been reset.");
        }
    }
}
