using Discord.Commands;
using MTD.CouchBot.Bot;
using MTD.CouchBot.Domain;
using MTD.CouchBot.Domain.Models.Bot;
using MTD.CouchBot.Managers;
using MTD.CouchBot.Managers.Implementations;
using MTD.CouchBot.Models.Bot;
using MTD.CouchBot.Modules;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTD.DiscordBot.Modules
{
    [Group("twitch")]
    public class Twitch : BaseModule
    {
        ITwitchManager _twitchManager;

        public Twitch()
        {
            _twitchManager = new TwitchManager();
        }
        
        [Command("add")]
        public async Task Add(string channelName)
        {
            if (!IsApprovedAdmin)
            {
                return;
            }

            var twitchChannelId = await _twitchManager.GetTwitchIdByLogin(channelName);

            if (string.IsNullOrEmpty(twitchChannelId))
            {
                await Context.Channel.SendMessageAsync("Twitch Channel " + channelName + " does not exist.");

                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + Context.Guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            if (server.ServerTwitchChannels == null)
                server.ServerTwitchChannels = new List<string>();

            if (server.ServerTwitchChannelIds == null)
                server.ServerTwitchChannelIds = new List<string>();

            if (!string.IsNullOrEmpty(server.OwnerTwitchChannel) && server.OwnerTwitchChannel.ToLower().Equals(channelName.ToLower()))
            {
                await Context.Channel.SendMessageAsync("The channel " + channelName + " is configured as the Owner Twitch channel. " +
                    "Please remove it with the '!cb twitch resetowner' command and then try re-adding it.");

                return;
            }

            if (!server.ServerTwitchChannels.Contains(channelName.ToLower()))
            {
                server.ServerTwitchChannels.Add(channelName.ToLower());
                server.ServerTwitchChannelIds.Add(await _twitchManager.GetTwitchIdByLogin(channelName));
                File.WriteAllText(file, JsonConvert.SerializeObject(server));

                await Context.Channel.SendMessageAsync("Added " + channelName + " to the server Twitch streamer list.");
            }
            else
            {
                await Context.Channel.SendMessageAsync(channelName + " is already on the server Twitch streamer list.");
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
            if (!IsAdmin)
            {
                return;
            }

            var twitchChannelId = await _twitchManager.GetTwitchIdByLogin(channel);

            if (string.IsNullOrEmpty(twitchChannelId))
            {
                await Context.Channel.SendMessageAsync("Twitch Channel " + channel + " does not exist.");

                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + Context.Guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            if (server.ServerTwitchChannels != null && server.ServerTwitchChannels.Contains(channel.ToLower()))
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
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            server.OwnerTwitchChannel = null;
            server.OwnerTwitchChannelId = null;
            File.WriteAllText(file, JsonConvert.SerializeObject(server));
            await Context.Channel.SendMessageAsync("Owner Twitch Channel has been reset.");
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
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            var twitchId = await _twitchManager.GetTwitchIdByLogin(channelName);

            if (!string.IsNullOrEmpty(twitchId) && twitchId == "0")
            {
                await Context.Channel.SendMessageAsync(channelName + " doesn't exist on Twitch.");

                return;
            }

            var streamResponse = await _twitchManager.GetStreamById(twitchId);
            var stream = streamResponse.stream;

            if (stream == null)
            {
                await Context.Channel.SendMessageAsync(channelName + " isn't online.");

                return;
            }
            
            string url = stream.channel.url;
            string name = stream.channel.display_name.Replace("_", "\\_").Replace("*", "\\*");
            string avatarUrl = stream.channel.logo != null ? stream.channel.logo : "https://static-cdn.jtvnw.net/jtv_user_pictures/xarth/404_user_70x70.png";
            string thumbnailUrl = stream.preview.large;

            var message = await MessagingHelper.BuildMessage(name, stream.game, stream.channel.status, url, avatarUrl,
                                                    thumbnailUrl, Constants.Twitch, stream.channel._id.ToString(), server, server.GoLiveChannel, null);
            await MessagingHelper.SendMessages(Constants.Twitch, new List<BroadcastMessage>() { message });
        }

        [Command("addgame")]
        public async Task AddGame(string gameName)
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

            if (server.ServerGameList == null)
            {
                server.ServerGameList = new List<string>();
            }

            if (!server.ServerGameList.Contains(gameName, StringComparer.CurrentCultureIgnoreCase))
            {
                server.ServerGameList.Add(gameName);
                File.WriteAllText(file, JsonConvert.SerializeObject(server));

                await Context.Channel.SendMessageAsync("Added " + gameName + " to the server Twitch game list.");
            }
            else
            {
                await Context.Channel.SendMessageAsync(gameName + " is already on the server Twitch game list.");
            }
        }

        [Command("removegame")]
        public async Task RemoveGame(string gameName)
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

            if (server.ServerGameList == null)
            {
                return;
            }

            if (server.ServerGameList.Contains(gameName, StringComparer.CurrentCultureIgnoreCase))
            {
                server.ServerGameList.Remove(gameName);
                File.WriteAllText(file, JsonConvert.SerializeObject(server));

                await Context.Channel.SendMessageAsync("Removed " + gameName + " from the server Twitch game list.");
            }
            else
            {
                await Context.Channel.SendMessageAsync(gameName + " isn't on the server Twitch game list.");
            }
        }

        [Command("listgames")]
        public async Task ListGames()
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

            if (server.ServerGameList == null)
            {
                server.ServerGameList = new List<string>();
            }

            var builder = new StringBuilder();

            foreach (var tt in server.ServerGameList)
            {
                builder.Append(tt + ", ");
            }

            string info = "```Markdown\r\n" +
              "# Server Twitch Games\r\n" +
              builder.ToString().Trim().TrimEnd(',') + "\r\n" +
              "```\r\n";

            await Context.Channel.SendMessageAsync(info);
        }

        [Command("addteam")]
        public async Task AddTeam(string teamName)
        {
            if (!IsApprovedAdmin)
            {
                return;
            }

            var team = await _twitchManager.GetTwitchTeamByName(teamName);

            if (team == null)
            {
                await Context.Channel.SendMessageAsync(teamName + " is not a valid Twitch team token. The team token is on the end of the Team URL, ie: (http://twitch.tv/teams/ths .. use !cb twitch addteam ths).");

                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + Context.Guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
            {
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));
            }

            if (server.TwitchTeams == null)
            {
                server.TwitchTeams = new List<string>();
            }

            if (!server.TwitchTeams.Contains(teamName, StringComparer.CurrentCultureIgnoreCase))
            {
                server.TwitchTeams.Add(teamName);
                File.WriteAllText(file, JsonConvert.SerializeObject(server));

                await Context.Channel.SendMessageAsync("Added " + team.DisplayName + " (" + teamName + ") to the server Twitch team list.");
            }
            else
            {
                await Context.Channel.SendMessageAsync(team.DisplayName + " (" + teamName + ") is already on the server Twitch team list.");
            }
        }

        [Command("removeteam")]
        public async Task RemoveTeam(string teamName)
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

            if (server.TwitchTeams == null)
            {
                return;
            }

            if (server.TwitchTeams.Contains(teamName, StringComparer.CurrentCultureIgnoreCase))
            {
                var team = await _twitchManager.GetTwitchTeamByName(teamName);

                server.TwitchTeams.Remove(teamName);
                File.WriteAllText(file, JsonConvert.SerializeObject(server));

                await Context.Channel.SendMessageAsync("Removed " + team.DisplayName + " (" + teamName + ") from the server Twitch team list.");
            }
            else
            {
                await Context.Channel.SendMessageAsync("Token " + teamName + " isn't on the server Twitch team list.");
            }
        }

        [Command("listteams")]
        public async Task ListTeams()
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

            if (server.TwitchTeams == null)
            {
                server.TwitchTeams = new List<string>();
            }

            var builder = new StringBuilder();

            foreach (var tt in server.TwitchTeams)
            {
                var team = await _twitchManager.GetTwitchTeamByName(tt);

                builder.Append(team.DisplayName + " (" + tt + "), ");
            }

            string info = "```Markdown\r\n" +
              "# Server Twitch Teams\r\n" +
              builder.ToString().Trim().TrimEnd(',') + "\r\n" +
              "```\r\n";

            await Context.Channel.SendMessageAsync(info);
        }

    }
}
