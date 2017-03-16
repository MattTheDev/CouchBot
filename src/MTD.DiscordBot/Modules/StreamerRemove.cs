using System;
using System.IO;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using Discord;
using Newtonsoft.Json;
using MTD.DiscordBot.Json;
using MTD.DiscordBot.Domain;
using System.Collections.Generic;
using MTD.DiscordBot.Managers;
using MTD.DiscordBot.Managers.Implementations;

namespace MTD.DiscordBot.Modules
{
    [Group("streamerremove")]
    public class StreamerRemove : ModuleBase
    {
        ITwitchManager twitchManager;

        public StreamerRemove()
        {
            twitchManager = new TwitchManager();
        }

        [Command("twitch"), Summary("Remove a twitch streamer.")]
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
                return;

            if (server.ServerTwitchChannels.Contains(channel.ToLower()))
            {
                var twitchId = await twitchManager.GetTwitchIdByLogin(channel);

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

        [Command("youtube"), Summary("Remove a youtube streamer.")]
        public async Task Youtube(string channel)
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

        [Command("beam"), Summary("Remove a beam streamer.")]
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
                return;

            if (server.ServerBeamChannels.Contains(channel.ToLower()))
            {
                server.ServerBeamChannels.Remove(channel.ToLower());
                File.WriteAllText(file, JsonConvert.SerializeObject(server));
                await Context.Channel.SendMessageAsync("Removed " + channel + " from the server Beam streamer list.");
            }
            else
            {
                await Context.Channel.SendMessageAsync(channel + " wasn't on the server Beam streamer list.");
            }
        }

        [Command("hitbox"), Summary("Remove a hitbox streamer.")]
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
                return;

            if (server.ServerHitboxChannels.Contains(channel.ToLower()))
            {
                server.ServerHitboxChannels.Remove(channel.ToLower());
                File.WriteAllText(file, JsonConvert.SerializeObject(server));
                await Context.Channel.SendMessageAsync("Removed " + channel + " from the server Hitbox streamer list.");
            }
            else
            {
                await Context.Channel.SendMessageAsync(channel + " wasn't on the server Hitbox streamer list.");
            }
        }
    }
}
