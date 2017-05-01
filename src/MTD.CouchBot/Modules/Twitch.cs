using Discord;
using Discord.Commands;
using MTD.CouchBot;
using MTD.CouchBot.Domain;
using MTD.CouchBot.Json;
using MTD.CouchBot.Managers;
using MTD.CouchBot.Managers.Implementations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    }
}
