using Discord;
using Discord.Commands;
using Microsoft.Extensions.Options;
using MTD.CouchBot.Domain;
using MTD.CouchBot.Domain.Models.Bot;
using MTD.CouchBot.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTD.CouchBot.Domain.Enums;
using MTD.CouchBot.Managers;

namespace MTD.CouchBot.Modules
{
    [Group("twitch")]
    public class Twitch : BaseModule
    {
        private readonly TwitchManager _twitchManager;
        private readonly MessagingService _messagingService;
        private readonly BotSettings _botSettings;
        private readonly FileService _fileService;

        public Twitch(TwitchManager twitchManager, MessagingService messagingService, IOptions<BotSettings> botSettings, FileService fileService) 
            : base(botSettings, fileService)
        { 
            _twitchManager = twitchManager;
            _messagingService = messagingService;
            _botSettings = botSettings.Value;
            _fileService = fileService;
        }
        
        [Command("add")]
        public async Task Add(string name)
        {
            if (!IsApprovedAdmin)
            {
                return;
            }

            var twitchChannelId = await _twitchManager.GetTwitchIdByLogin(name);

            if (string.IsNullOrEmpty(twitchChannelId))
            {
                await Context.Channel.SendMessageAsync("Twitch Channel " + name + " does not exist.");

                return;
            }

            var file = $"{_botSettings.DirectorySettings.ConfigRootDirectory}{_botSettings.DirectorySettings.GuildDirectory}{Context.Guild.Id}.json";
            var server = new DiscordServer();

            if (File.Exists(file))
            {
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));
            }

            if (server.ServerTwitchChannelIds == null)
            {
                server.ServerTwitchChannelIds = new List<string>();
            }

            if (!string.IsNullOrEmpty(server.OwnerTwitchChannelId) && server.OwnerTwitchChannelId.Equals(twitchChannelId))
            {
                await Context.Channel.SendMessageAsync("The channel " + name + " is configured as the Owner Twitch channel. " +
                    "Please remove it with the '!cb twitch resetowner' command and then try re-adding it.");

                return;
            }

            if (!server.ServerTwitchChannelIds.Contains(twitchChannelId))
            {
                server.ServerTwitchChannelIds.Add(twitchChannelId);
                await _fileService.SaveDiscordServer(server, Context.Guild);

                await Context.Channel.SendMessageAsync("Added " + name + " to the server Twitch streamer list.");
            }
            else
            {
                await Context.Channel.SendMessageAsync(name + " is already on the server Twitch streamer list.");
            }
        }

        [Command("remove")]
        public async Task Remove(string name)
        {
            if (!IsApprovedAdmin)
            {
                return;
            }

            var twitchChannelId = await _twitchManager.GetTwitchIdByLogin(name);

            if (string.IsNullOrEmpty(twitchChannelId))
            {
                await Context.Channel.SendMessageAsync("Twitch Channel " + name + " does not exist.");

                return;
            }

            var file = $"{_botSettings.DirectorySettings.ConfigRootDirectory}{_botSettings.DirectorySettings.GuildDirectory}{Context.Guild.Id}.json";
            var server = new DiscordServer();

            if (File.Exists(file))
            {
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));
            }

            if (server.ServerTwitchChannelIds == null)
            {
                return;
            }

            if (server.ServerTwitchChannelIds.Contains(twitchChannelId))
            {
                server.ServerTwitchChannelIds.Remove(twitchChannelId);
                await _fileService.SaveDiscordServer(server, Context.Guild);

                await Context.Channel.SendMessageAsync("Removed " + name + " from the server Twitch streamer list.");
            }
            else
            {
                await Context.Channel.SendMessageAsync(name + " wasn't on the server Twitch streamer list.");
            }
        }

        [Command("owner")]
        public async Task Owner(string name)
        {
            if (!IsAdmin)
            {
                return;
            }

            var twitchChannelId = await _twitchManager.GetTwitchIdByLogin(name);

            if (string.IsNullOrEmpty(twitchChannelId))
            {
                await Context.Channel.SendMessageAsync("Twitch Channel " + name + " does not exist.");

                return;
            }

            var file = $"{_botSettings.DirectorySettings.ConfigRootDirectory}{_botSettings.DirectorySettings.GuildDirectory}{Context.Guild.Id}.json";
            var server = new DiscordServer();

            if (File.Exists(file))
            {
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));
            }

            if (server.ServerTwitchChannelIds != null && server.ServerTwitchChannelIds.Contains(twitchChannelId))
            {
                await Context.Channel.SendMessageAsync("The channel " + name + " is in the list of server Twitch Channels. " +
                    "Please remove it with '!cb twitch remove " + name + "' and then retry setting your owner channel.");

                return;
            }

            server.OwnerTwitchChannelId = twitchChannelId;
            await _fileService.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("Owner Twitch Channel has been set to " + name + ".");
        }

        [Command("resetowner")]
        public async Task ResetOwner()
        {
            if (!IsAdmin)
            {
                return;
            }

            var file = $"{_botSettings.DirectorySettings.ConfigRootDirectory}{_botSettings.DirectorySettings.GuildDirectory}{Context.Guild.Id}.json";
            var server = new DiscordServer();

            if (File.Exists(file))
            {
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));
            }

            server.OwnerTwitchChannelId = null;
            await _fileService.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("Owner Twitch Channel has been reset.");
        }

        [Command("announce")]
        public async Task Announce(string channelName)
        {
            if (!IsAdmin)
            {
                return;
            }

            var file = $"{_botSettings.DirectorySettings.ConfigRootDirectory}{_botSettings.DirectorySettings.GuildDirectory}{Context.Guild.Id}.json";
            var server = new DiscordServer();

            if (File.Exists(file))
            {
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));
            }

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
            
            var url = stream.channel.url;
            var name = Format.Sanitize(stream.channel.display_name);
            var avatarUrl = stream.channel.logo ?? "https://static-cdn.jtvnw.net/jtv_user_pictures/xarth/404_user_70x70.png";
            var thumbnailUrl = stream.preview.large;
            
            var message = _messagingService.BuildMessage(name, stream.game, stream.channel.status, url, avatarUrl,
                                                    thumbnailUrl, Constants.Twitch, stream.channel._id.ToString(), server, server.GoLiveChannel, null, false,
                stream.viewers, stream.channel.views, stream.channel.followers);
            await _messagingService.SendMessages(Constants.Twitch, new List<BroadcastMessage> { message });
        }

        [Command("addgame")]
        public async Task AddGame(string gameName)
        {
            if (!IsApprovedAdmin)
            {
                return;
            }

            var file = $"{_botSettings.DirectorySettings.ConfigRootDirectory}{_botSettings.DirectorySettings.GuildDirectory}{Context.Guild.Id}.json";
            var server = new DiscordServer();

            if (File.Exists(file))
            {
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));
            }

            if (server.ServerGameList == null)
            {
                server.ServerGameList = new List<string>();
            }

            var games = await _twitchManager.SearchForGameByName(gameName);

            if(games.games == null)
            {
                await Context.Channel.SendMessageAsync(gameName + " is not a valid game. Please check the name, and try again.");
                return;
            }

            if (games.games.FirstOrDefault(x => x.name.Equals(gameName, StringComparison.CurrentCultureIgnoreCase)) == null)
            {
                var suggestedGameList = "That is not a valid game name. Is the game you want on the list below?\r\n\r\n" +
                    String.Join(", ", games.games.Select(x => x.name));

                await Context.Channel.SendMessageAsync(suggestedGameList);
                return;
            }

            if (!server.ServerGameList.Contains(gameName, StringComparer.CurrentCultureIgnoreCase))
            {
                server.ServerGameList.Add(gameName);
                await _fileService.SaveDiscordServer(server, Context.Guild);

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

            var file = $"{_botSettings.DirectorySettings.ConfigRootDirectory}{_botSettings.DirectorySettings.GuildDirectory}{Context.Guild.Id}.json";
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
                await _fileService.SaveDiscordServer(server, Context.Guild);

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

            var file = $"{_botSettings.DirectorySettings.ConfigRootDirectory}{_botSettings.DirectorySettings.GuildDirectory}{Context.Guild.Id}.json";
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

            var info = "```Markdown\r\n" +
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

            var file = $"{_botSettings.DirectorySettings.ConfigRootDirectory}{_botSettings.DirectorySettings.GuildDirectory}{Context.Guild.Id}.json";
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
                await _fileService.SaveDiscordServer(server, Context.Guild);

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

            var file = $"{_botSettings.DirectorySettings.ConfigRootDirectory}{_botSettings.DirectorySettings.GuildDirectory}{Context.Guild.Id}.json";
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
                await _fileService.SaveDiscordServer(server, Context.Guild);

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

            var file = $"{_botSettings.DirectorySettings.ConfigRootDirectory}{_botSettings.DirectorySettings.GuildDirectory}{Context.Guild.Id}.json";
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

            var info = "```Markdown\r\n" +
              "# Server Twitch Teams\r\n" +
              builder.ToString().Trim().TrimEnd(',') + "\r\n" +
              "```\r\n";

            await Context.Channel.SendMessageAsync(info);
        }

        [Command("live")]
        public async Task Live()
        {
            var users = await Context.Guild.GetUsersAsync();
            var list = new List<string>();

            foreach (var u in users)
            {
                if (u.Activity != null && u.Activity.Type == ActivityType.Streaming)
                {
                    try
                    {
                        var activity = (StreamingGame) u.Activity;
                        if (!string.IsNullOrEmpty(activity.Url))
                        {
                            list.Add("<" + activity.Url + ">");
                        }
                    }
                    catch (Exception)
                    {
                        // Invalid Activity.
                    }
                }
            }

            await Context.Channel.SendMessageAsync("Currently Live: \r\n" + string.Join("\r\n", list));
        }

        [Command("discover")]
        public async Task Discover(string discoveryType)
        {
            var discoveryTypeLower = discoveryType.ToLowerInvariant();

            if (!IsAdmin)
            {
                return;
            }

            if(discoveryTypeLower != "all" && discoveryTypeLower != "none" && discoveryTypeLower != "role")
            {
                await Context.Channel.SendMessageAsync("The twitch discover command syntax is: ```!cb twitch discover [all/none/role] {roleName}\r\nPick one - all, none, or role.\r\nIf role, provide the role name to use. Do not tag the role, just provide the name.\r\n```");
                return;
            }

            if(discoveryTypeLower == "role")
            {
                await Context.Channel.SendMessageAsync("Please provide the role name when selecting role as your discover option. (ie: !cb twitch discover role streamers. Do not tag the role.)");
                return;
            }

            var file = $"{_botSettings.DirectorySettings.ConfigRootDirectory}{_botSettings.DirectorySettings.GuildDirectory}{Context.Guild.Id}.json";
            var server = new DiscordServer();

            if (File.Exists(file))
            {
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));
            }

            server.DiscoverTwitch = discoveryTypeLower;
            await _fileService.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("Discover Twitch has been set to '" + discoveryTypeLower + "'.");
        }

        [Command("discover")]
        public async Task Discover(string discoveryType, string roleName)
        {
            discoveryType = discoveryType.ToLowerInvariant();
            
            if (!IsAdmin)
            {
                return;
            }

            if (discoveryType != "all" && discoveryType != "none" && discoveryType != "role")
            {
                await Context.Channel.SendMessageAsync("The twitch discover command syntax is: ```!cb twitch discover [all/none/role] {roleName}\r\nPick one - all, none, or role.\r\nIf role, provide the role name to use. Do not tag the role, just provide the name.\r\n```");
                return;
            }

            if (discoveryType == "role" && string.IsNullOrEmpty(roleName))
            {
                await Context.Channel.SendMessageAsync("Please provide the role name when selecting role as your discover option. (ie: !cb twitch discover role streamers. Do not tag the role.)");
                return;
            }

            var role = Context.Guild.Roles.FirstOrDefault(r => string.Equals(r.Name, roleName, StringComparison.CurrentCultureIgnoreCase));

            if(role == null)
            {
                await Context.Channel.SendMessageAsync("The role " + roleName + " doesn't exist.");
                return;
            }

            var file = $"{_botSettings.DirectorySettings.ConfigRootDirectory}{_botSettings.DirectorySettings.GuildDirectory}{Context.Guild.Id}.json";
            var server = new DiscordServer();

            if (File.Exists(file))
            {
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));
            }

            server.DiscoverTwitch = discoveryType;
            server.DiscoverTwitchRole = role.Id;
            await _fileService.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("Discover Twitch has been set to '" + discoveryType + "' with the role '" + roleName + "'.");
        }

        [Command("liverole")]
        public async Task LiveRole(IRole role)
        {
            if (!IsAdmin) return;

            var server = GetServer();

            server.LiveTwitchRole = role.Id;
            _fileService.SaveDiscordServer(server);
            await Context.Channel.SendMessageAsync($"Your Live Twitch Role has been set to: {role.Name}");
        }

        /* New Methods */
        [Command("add")]
        [Alias("+")]
        public async Task Add(string channelName, IGuildChannel guildChannel)
        {
            if (!IsApprovedAdmin)
            {
                return;
            }

            var twitchChannelId = await _twitchManager.GetTwitchIdByLogin(channelName);

            if (string.IsNullOrEmpty(twitchChannelId))
            {
                await Context.Channel.SendMessageAsync($"Sorry, {channelName}, doesn't appear to be a valid channel name.");

                return;
            }

            var server = GetServer();

            if (server == null)
            {
                server = new DiscordServer();
            }

            if (server.Streamers == null)
            {
                server.Streamers = new List<DiscordStreamer>();
            }

            var streamer = server.Streamers.Count == 0 ? null : server.Streamers
                .FirstOrDefault(s => s.Platform == Platform.Twitch &&
                                     s.DisordChannelId == guildChannel.Id &&
                                     s.StreamerChannelId == twitchChannelId);

            if (streamer != null)
            {
                await Context.Channel.SendMessageAsync($"Sorry, {channelName} is already configured on that channel.");
                return;
            }

            streamer = new DiscordStreamer
            {
                DisordChannelId = guildChannel.Id,
                Platform = Platform.Twitch,
                StreamerChannelId = twitchChannelId
            };

            server.Streamers.Add(streamer);

            _fileService.SaveDiscordServer(server);

            await Context.Channel.SendMessageAsync($"{channelName} will now announce on <#{guildChannel.Id}> when they go live.");
        }

        [Command("remove")]
        [Alias("-")]
        public async Task Remove(string channelName, IGuildChannel guildChannel)
        {
            if (!IsApprovedAdmin)
            {
                return;
            }

            var twitchChannelId = await _twitchManager.GetTwitchIdByLogin(channelName);

            if (string.IsNullOrEmpty(twitchChannelId))
            {
                await Context.Channel.SendMessageAsync($"Sorry, {channelName}, doesn't appear to be a valid channel name.");

                return;
            }

            var server = GetServer();

            if (server == null)
            {
                await Context.Channel.SendMessageAsync($"Sorry, {channelName} is not configured on this server.");
                return;
            }

            if (server.Streamers == null)
            {
                await Context.Channel.SendMessageAsync($"Sorry, {channelName} is not configured on this server.");
                return;
            }

            var streamer = server.Streamers.Count == 0 ? null : server.Streamers
                .FirstOrDefault(s => s.Platform == Platform.Twitch &&
                                     s.DisordChannelId == guildChannel.Id &&
                                     s.StreamerChannelId == twitchChannelId);

            if (streamer == null)
            {
                await Context.Channel.SendMessageAsync($"Sorry, {channelName} is not configured on that channel.");
                return;
            }

            server.Streamers.Remove(streamer);

            _fileService.SaveDiscordServer(server);

            await Context.Channel.SendMessageAsync($"{channelName} will no longer announce on <#{guildChannel.Id}> when they go live.");
        }
    }
}
