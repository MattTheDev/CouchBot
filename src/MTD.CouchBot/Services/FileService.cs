using Discord;
using Microsoft.Extensions.Options;
using MTD.CouchBot.Domain.Models.Bot;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using MTD.CouchBot.Domain.Utilities;

namespace MTD.CouchBot.Services
{
    public class FileService
    {
        private readonly BotSettings _botSettings;
        private readonly DiscordShardedClient _discord;
        private readonly DiscordService _discordService;

        public FileService(IOptions<BotSettings> botSettings, DiscordShardedClient discord, DiscordService discordService)
        {
            _botSettings = botSettings.Value;
            _discord = discord;
            _discordService = discordService;
        }

        public void SaveDiscordServer(DiscordServer server)
        {
            var file = _botSettings.DirectorySettings.ConfigRootDirectory + _botSettings.DirectorySettings.GuildDirectory + server.Id + ".json";
            File.WriteAllText(file, JsonConvert.SerializeObject(server));
        }

        public async Task SaveDiscordServer(DiscordServer server, IGuild guild)
        {
            var guildOwner = await guild.GetOwnerAsync();
            server.Id = guild.Id;
            server.Name = guild.Name;
            server.OwnerId = guild.OwnerId;
            server.OwnerName = guildOwner == null ? "" : guildOwner.Username;

            var file = _botSettings.DirectorySettings.ConfigRootDirectory + _botSettings.DirectorySettings.GuildDirectory + server.Id + ".json";
            File.WriteAllText(file, JsonConvert.SerializeObject(server));
        }

        public List<string> GetConfiguredServerFileNames()
        {
            var servers = new List<string>();

            // Get Servers
            foreach (var server in Directory.GetFiles(_botSettings.DirectorySettings.ConfigRootDirectory + _botSettings.DirectorySettings.GuildDirectory))
            {
                servers.Add(Path.GetFileName(server.Replace(".json", "")));
            }

            return servers;
        }

        public List<DiscordServer> GetConfiguredServers()
        {
            var servers = new List<DiscordServer>();

            // Get Servers
            foreach (var server in Directory.GetFiles(_botSettings.DirectorySettings.ConfigRootDirectory + _botSettings.DirectorySettings.GuildDirectory))
            {
                try
                {
                    servers.Add(JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(server)));
                }
                catch (Exception)
                {
                    continue;
                }
            }

            return servers;
        }

        public List<string> GetConfiguredServerPaths()
        {
            var servers = new List<string>();

            // Get Servers
            foreach (var server in Directory.GetFiles(_botSettings.DirectorySettings.ConfigRootDirectory + _botSettings.DirectorySettings.GuildDirectory))
            {
                servers.Add(server);
            }

            return servers;
        }

        public List<DiscordServer> GetConfiguredServersWithLiveChannel()
        {
            var servers = new List<DiscordServer>();

            // Get Servers
            foreach (var s in Directory.GetFiles(_botSettings.DirectorySettings.ConfigRootDirectory + _botSettings.DirectorySettings.GuildDirectory))
            {
                try
                {
                    var server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(s));

                    if (!server.AllowLive)
                    {
                        continue;
                    }

                    if ((server.Id == 0 || server.GoLiveChannel == 0) && (server.Id == 0 || server.OwnerLiveChannel == 0))
                    {
                        continue;
                    }

                    servers.Add(server);
                }
                catch (Exception)
                {
                    continue;
                }
            }

            return servers;
        }

        public List<DiscordServer> GetConfiguredServersWithOwnerLiveChannel()
        {
            var servers = new List<DiscordServer>();

            // Get Servers
            foreach (var s in Directory.GetFiles(_botSettings.DirectorySettings.ConfigRootDirectory + _botSettings.DirectorySettings.GuildDirectory))
            {
                try
                {
                    var server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(s));

                    if (!server.AllowLive)
                    {
                        continue;
                    }

                    if (server.Id == 0 || server.OwnerLiveChannel == 0)
                    {
                        continue;
                    }

                    servers.Add(server);
                }
                catch (Exception)
                {
                    continue;
                }
            }

            return servers;
        }

        public List<DiscordServer> GetConfiguredServersWithPublishedChannel()
        {
            var servers = new List<DiscordServer>();

            // Get Servers
            foreach (var s in Directory.GetFiles(_botSettings.DirectorySettings.ConfigRootDirectory + _botSettings.DirectorySettings.GuildDirectory))
            {
                try
                {
                    var server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(s));

                    if (!server.AllowLive)
                    {
                        continue;
                    }

                    if ((server.Id == 0 || server.PublishedChannel == 0) && (server.Id == 0 || server.OwnerPublishedChannel == 0))
                    {
                        continue;
                    }

                    servers.Add(server);
                }
                catch (Exception)
                {
                    continue;
                }
            }

            return servers;
        }


        public List<DiscordServer> GetServersWithLiveChannelAndAllowDiscover()
        {
            var servers = new List<DiscordServer>();

            // Get Servers
            foreach (var s in Directory.GetFiles(_botSettings.DirectorySettings.ConfigRootDirectory + _botSettings.DirectorySettings.GuildDirectory))
            {
                try
                {
                    var server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(s));

                    if (string.IsNullOrEmpty(server.DiscoverTwitch))
                    {
                        continue;
                    }

                    if (!server.AllowLive && !(server.DiscoverTwitch.Equals("all") || server.DiscoverTwitch.Equals("role")))
                    {
                        continue;
                    }

                    if (server.Id == 0 || server.GoLiveChannel == 0)
                    {
                        continue;
                    }

                    servers.Add(server);
                }
                catch (Exception)
                {
                    continue;
                }
            }

            return servers;
        }

        public List<DiscordServer> GetServersWithCustomCommandsWithRepeat()
        {
            var servers = new List<DiscordServer>();

            foreach (var s in Directory.GetFiles(_botSettings.DirectorySettings.ConfigRootDirectory + 
                _botSettings.DirectorySettings.GuildDirectory))
            {
                try
                {
                    var server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(s));

                    if(server.CustomCommands == null || server.CustomCommands.FirstOrDefault(cc => cc.Repeat) == null)
                    {
                        continue;
                    }

                    servers.Add(server);
                }
                catch (Exception)
                {
                    continue;
                }
            }

            return servers;
        }

        public DiscordServer GetConfiguredServerById(ulong id)
        {
            try
            {
                return JsonConvert.DeserializeObject<DiscordServer>(
                    File.ReadAllText(_botSettings.DirectorySettings.ConfigRootDirectory + _botSettings.DirectorySettings.GuildDirectory + id + ".json"));
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        public List<LiveChannel> GetCurrentlyLiveBeamChannels()
        {
            var liveChannels = new List<LiveChannel>();

            // Get Live Channels
            foreach (var live in Directory.GetFiles(_botSettings.DirectorySettings.ConfigRootDirectory + _botSettings.DirectorySettings.LiveDirectory + _botSettings.DirectorySettings.MixerDirectory))
            {
                try
                {
                    liveChannels.Add(JsonConvert.DeserializeObject<LiveChannel>(File.ReadAllText(live)));
                }
                catch (Exception)
                {
                    continue;
                }
            }

            return liveChannels;
        }

        public List<LiveChannel> GetCurrentlyLiveYouTubeChannels()
        {
            var liveChannels = new List<LiveChannel>();

            // Get Live Channels
            foreach (var live in Directory.GetFiles(_botSettings.DirectorySettings.ConfigRootDirectory + _botSettings.DirectorySettings.LiveDirectory + _botSettings.DirectorySettings.YouTubeDirectory))
            {
                try
                {
                    liveChannels.Add(JsonConvert.DeserializeObject<LiveChannel>(File.ReadAllText(live)));
                }
                catch (Exception)
                {
                    continue;
                }
            }

            return liveChannels;
        }

        public List<LiveChannel> GetCurrentlyLiveTwitchChannels()
        {
            var liveChannels = new List<LiveChannel>();

            // Get Live Channels
            foreach (var live in Directory.GetFiles(_botSettings.DirectorySettings.ConfigRootDirectory + _botSettings.DirectorySettings.LiveDirectory + _botSettings.DirectorySettings.TwitchDirectory))
            {
                try
                {
                    liveChannels.Add(JsonConvert.DeserializeObject<LiveChannel>(File.ReadAllText(live)));
                }
                catch (Exception)
                {
                    continue;
                }
            }

            return liveChannels;
        }

        public List<LiveChannel> GetCurrentlyLiveHitboxChannels()
        {
            var liveChannels = new List<LiveChannel>();

            // Get Live Channels
            foreach (var live in Directory.GetFiles(_botSettings.DirectorySettings.ConfigRootDirectory + _botSettings.DirectorySettings.LiveDirectory + _botSettings.DirectorySettings.SmashcastDirectory))
            {
                try
                {
                    liveChannels.Add(JsonConvert.DeserializeObject<LiveChannel>(File.ReadAllText(live)));
                }
                catch (Exception)
                {
                    continue;
                }
            }

            return liveChannels;
        }

        public List<LiveChannel> GetCurrentlyLivePicartoChannels()
        {
            var liveChannels = new List<LiveChannel>();

            // Get Live Channels
            foreach (var live in Directory.GetFiles(_botSettings.DirectorySettings.ConfigRootDirectory + _botSettings.DirectorySettings.LiveDirectory + _botSettings.DirectorySettings.PicartoDirectory))
            {
                try
                {
                    liveChannels.Add(JsonConvert.DeserializeObject<LiveChannel>(File.ReadAllText(live)));
                }
                catch (Exception)
                {
                    continue;
                }
            }

            return liveChannels;
        }

        public List<LiveChannel> GetCurrentlyLiveMobcrushChannels()
        {
            var liveChannels = new List<LiveChannel>();

            // Get Live Channels
            foreach (var live in Directory.GetFiles(_botSettings.DirectorySettings.ConfigRootDirectory + _botSettings.DirectorySettings.LiveDirectory + _botSettings.DirectorySettings.MobcrushDirectory))
            {
                try
                {
                    liveChannels.Add(JsonConvert.DeserializeObject<LiveChannel>(File.ReadAllText(live)));
                }
                catch (Exception)
                {
                    continue;
                }
            }

            return liveChannels;
        }

        public List<LiveChannel> GetCurrentlyLivePiczelChannels()
        {
            var liveChannels = new List<LiveChannel>();

            // Get Live Channels
            foreach (var live in Directory.GetFiles(_botSettings.DirectorySettings.ConfigRootDirectory + _botSettings.DirectorySettings.LiveDirectory + _botSettings.DirectorySettings.PiczelDirectory))
            {
                try
                {
                    liveChannels.Add(JsonConvert.DeserializeObject<LiveChannel>(File.ReadAllText(live)));
                }
                catch (Exception)
                {
                    continue;
                }
            }

            return liveChannels;
        }

        public void DeleteLiveBeamChannel(string beamId)
        {
            File.Delete(_botSettings.DirectorySettings.ConfigRootDirectory + _botSettings.DirectorySettings.LiveDirectory + _botSettings.DirectorySettings.MixerDirectory + beamId + ".json");
        }

        public List<DiscordServer> GetServersWithNoChannelsSet()
        {
            var servers = new List<DiscordServer>();

            // Get Servers
            foreach (var s in Directory.GetFiles(_botSettings.DirectorySettings.ConfigRootDirectory + _botSettings.DirectorySettings.GuildDirectory))
            {
                try
                {
                    var server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(s));


                    if (server.GoLiveChannel == 0 && server.OwnerLiveChannel == 0
                        && server.PublishedChannel == 0 && server.OwnerPublishedChannel == 0
                        && server.GreetingsChannel == 0)
                    {
                        servers.Add(server);
                    }
                }
                catch (Exception)
                {
                    continue;
                }
            }

            return servers;
        }

        public List<string> GetWhitelistedServers()
        {
            return (File.ReadAllText(_botSettings.DirectorySettings.ConfigRootDirectory + "Whitelist.txt")).Split(',').ToList();
        }

        public ChannelCounts GetUniqueServerChannelCounts()
        {
            var servers = GetConfiguredServers();

            var mixerChannels = new List<string>();
            var mobcrushChannels = new List<string>();
            var picartoChannels = new List<string>();
            var piczelChannels = new List<string>();
            var smashcastChannels = new List<string>();
            var twitchChannels = new List<string>();
            var youTubeChannels = new List<string>();
            var twitchTeams = new List<string>();
            var twitchGames = new List<string>();
            var mixerTeams = new List<string>();

            foreach (var s in servers)
            {
                if (s.ServerBeamChannelIds != null && s.ServerBeamChannelIds.Count > 0)
                {
                    foreach (var c in s.ServerBeamChannelIds)
                    {
                        if (!mixerChannels.Contains(c))
                        {
                            mixerChannels.Add(c);
                        }
                    }
                }

                if (s.OwnerBeamChannelId != null)
                {
                    if (!mixerChannels.Contains(s.OwnerBeamChannelId))
                    {
                        mixerChannels.Add(s.OwnerBeamChannelId);
                    }
                }

                if (s.ServerMobcrushIds != null && s.ServerMobcrushIds.Count > 0)
                {
                    foreach (var c in s.ServerMobcrushIds)
                    {
                        if (!mobcrushChannels.Contains(c))
                        {
                            mobcrushChannels.Add(c);
                        }
                    }
                }

                if (s.OwnerMobcrushId != null)
                {
                    if (!mobcrushChannels.Contains(s.OwnerMobcrushId))
                    {
                        mobcrushChannels.Add(s.OwnerMobcrushId);
                    }
                }

                if (s.PicartoChannels != null && s.PicartoChannels.Count > 0)
                {
                    foreach (var c in s.PicartoChannels)
                    {
                        if (!picartoChannels.Contains(c))
                        {
                            picartoChannels.Add(c);
                        }
                    }
                }

                if (s.OwnerPicartoChannel != null)
                {
                    if (!picartoChannels.Contains(s.OwnerPicartoChannel))
                    {
                        picartoChannels.Add(s.OwnerPicartoChannel);
                    }
                }

                if (s.ServerPiczelChannelIds != null && s.ServerPiczelChannelIds.Count > 0)
                {
                    foreach (var c in s.ServerPiczelChannelIds)
                    {
                        if (!piczelChannels.Contains(c.ToString()))
                        {
                            piczelChannels.Add(c.ToString());
                        }
                    }
                }

                if (s.OwnerPiczelChannelId != null)
                {
                    if (!piczelChannels.Contains(s.OwnerPiczelChannelId.ToString()))
                    {
                        piczelChannels.Add(s.OwnerPiczelChannelId.ToString());
                    }
                }

                if (s.ServerHitboxChannels != null && s.ServerHitboxChannels.Count > 0)
                {
                    foreach (var c in s.ServerHitboxChannels)
                    {
                        if (!smashcastChannels.Contains(c))
                        {
                            smashcastChannels.Add(c);
                        }
                    }
                }

                if (s.OwnerHitboxChannel != null)
                {
                    if (!smashcastChannels.Contains(s.OwnerHitboxChannel))
                    {
                        smashcastChannels.Add(s.OwnerHitboxChannel);
                    }
                }

                if (s.ServerTwitchChannelIds != null && s.ServerTwitchChannelIds.Count > 0)
                {
                    foreach (var c in s.ServerTwitchChannelIds)
                    {
                        if (!twitchChannels.Contains(c))
                        {
                            twitchChannels.Add(c);
                        }
                    }
                }

                if (s.OwnerTwitchChannelId != null)
                {
                    if (!twitchChannels.Contains(s.OwnerTwitchChannelId))
                    {
                        twitchChannels.Add(s.OwnerTwitchChannelId);
                    }
                }

                if (s.ServerYouTubeChannelIds != null && s.ServerYouTubeChannelIds.Count > 0)
                {
                    foreach (var c in s.ServerYouTubeChannelIds)
                    {
                        if (!youTubeChannels.Contains(c))
                        {
                            youTubeChannels.Add(c);
                        }
                    }
                }

                if (s.OwnerYouTubeChannelId != null)
                {
                    if (!youTubeChannels.Contains(s.OwnerYouTubeChannelId))
                    {
                        youTubeChannels.Add(s.OwnerYouTubeChannelId);
                    }
                }

                if (s.ServerGameList != null && s.ServerGameList.Count > 0)
                {
                    foreach (var g in s.ServerGameList)
                    {
                        if (!twitchGames.Contains(g))
                        {
                            twitchGames.Add(g);
                        }
                    }
                }

                if (s.TwitchTeams != null && s.TwitchTeams.Count > 0)
                {
                    foreach (var tt in s.TwitchTeams)
                    {
                        if (!twitchTeams.Contains(tt))
                        {
                            twitchTeams.Add(tt);
                        }
                    }
                }

                if (s.MixerTeams != null && s.MixerTeams.Count > 0)
                {
                    foreach (var mt in s.MixerTeams)
                    {
                        if (!mixerTeams.Contains(mt.ToString()))
                        {
                            mixerTeams.Add(mt.ToString());
                        }
                    }
                }
            }

            return new ChannelCounts
            {
                MixerCount = mixerChannels.Distinct().Count(),
                MobcrushCount = mobcrushChannels.Distinct().Count(),
                PicartoCount = picartoChannels.Distinct().Count(),
                PiczelCount = piczelChannels.Distinct().Count(),
                SmashcastCount = smashcastChannels.Distinct().Count(),
                TwitchCount = twitchChannels.Distinct().Count(),
                YouTubeCount = youTubeChannels.Distinct().Count(),
                TwitchGameCount = twitchGames.Distinct().Count(),
                TwitchTeamCount = twitchTeams.Distinct().Count(),
                MixerTeamCount = mixerTeams.Distinct().Count()
            };
        }

        public string GetAllowsByServerId(ulong serverId)
        {
            var server = GetConfiguredServerById(serverId);

            return  "- Allow Mention Mixer Live: " + server.AllowMentionMixerLive + "\r\n" +
                    "- Allow Mention Owner Mixer Live: " + server.AllowMentionOwnerMixerLive + "\r\n" +
                    "- Allow Mention Mobcrush Live: " + server.AllowMentionMobcrushLive + "\r\n" +
                    "- Allow Mention Owner Mobcrush Live: " + server.AllowMentionOwnerMobcrushLive + "\r\n" +
                    "- Allow Mention Picarto Live: " + server.AllowMentionPicartoLive + "\r\n" +
                    "- Allow Mention Owner Picarto Live: " + server.AllowMentionOwnerPicartoLive + "\r\n" +
                    "- Allow Mention Piczel Live: " + server.AllowMentionPiczelLive + "\r\n" +
                    "- Allow Mention Owner Piczel Live: " + server.AllowMentionOwnerPiczelLive + "\r\n" +
                    "- Allow Mention Smashcast Live: " + server.AllowMentionSmashcastLive + "\r\n" +
                    "- Allow Mention Owner Smashcast Live: " + server.AllowMentionOwnerSmashcastLive + "\r\n" +
                    "- Allow Mention Twitch Live: " + server.AllowMentionTwitchLive + "\r\n" +
                    "- Allow Mention Owner Twitch Live: " + server.AllowMentionOwnerTwitchLive + "\r\n" +
                    "- Allow Mention YouTube Live: " + server.AllowMentionYouTubeLive + "\r\n" +
                    "- Allow Mention Owner YouTube Live: " + server.AllowMentionOwnerYouTubeLive + "\r\n" +
                    "- Allow Mention YouTube Published: " + server.AllowMentionYouTubePublished + "\r\n" +
                    "- Allow Mention Owner YouTube Published: " + server.AllowMentionOwnerYouTubePublished +"\r\n" +
                    "- Allow Thumbnails: " + server.AllowThumbnails + "\r\n" +
                    "- Allow Greetings: " + server.Greetings + "\r\n" +
                    "- Allow Goodbyes: " + server.Goodbyes + "\r\n" +
                    "- Allow Live Content: " + server.AllowLive + "\r\n" +
                    "- Allow Published Content: " + server.AllowPublished + "\r\n" +
                    $"- Allow Twitch Vodcasts: {server.AllowVodcasts}\r\n";
        }

        public string GetChannelsByServerId(ulong serverId)
        {
            var server = GetConfiguredServerById(serverId);

            var golive = (IGuildChannel) _discord.GetChannel(server.GoLiveChannel);
            var goliveChannel = golive != null ? $"{golive.Name} ({golive.Id})" : "Not Set";

            var ownerGolive = (IGuildChannel)_discord.GetChannel(server.OwnerLiveChannel);
            var ownerGoliveChannel = ownerGolive != null ? $"{ownerGolive.Name} ({ownerGolive.Id})" : "Not Set";

            var greetings = (IGuildChannel)_discord.GetChannel(server.GreetingsChannel);
            var greetingsChannel = greetings != null ? $"{greetings.Name} ({greetings.Id})" : "Not Set";

            var vod = (IGuildChannel)_discord.GetChannel(server.PublishedChannel);
            var vodChannel = vod != null ? $"{vod.Name} ({vod.Id})" : "Not Set";

            var ownerVod = (IGuildChannel)_discord.GetChannel(server.OwnerPublishedChannel);
            var ownerVodChannel = ownerVod != null ? $"{ownerVod.Name} ({ownerVod.Id})" : "Not Set";

            return
                $"- Owner Go Live Channel: {ownerGoliveChannel}\r\n" +
                $"- Owner Published Channel: {ownerVodChannel}\r\n" +
                $"- Go Live Channel: {goliveChannel}\r\n" +
                $"- Published Channel: {vodChannel}\r\n" +
                $"- Greetings Channel: {greetingsChannel}\r\n";
        }

        public string GetMessagesByServerId(ulong serverId)
        {
            var server = GetConfiguredServerById(serverId);

            return "- Live Message: " + (string.IsNullOrEmpty(server.LiveMessage) ? "Default" : server.LiveMessage) + "\r\n" +
                    "- Published Message: " + (string.IsNullOrEmpty(server.PublishedMessage) ? "Default" : server.PublishedMessage) + "\r\n" +
                    "- Greeting Message: " + (string.IsNullOrEmpty(server.GreetingMessage) ? "Default" : server.GreetingMessage) + "\r\n" +
                    "- Goodbye Message: " + (string.IsNullOrEmpty(server.GoodbyeMessage) ? "Default" : server.GoodbyeMessage) + "\r\n" +
                    "- Stream Offline Message: " + (string.IsNullOrEmpty(server.StreamOfflineMessage) ? "Default" : server.StreamOfflineMessage) + "\r\n";
        }

        public async Task<string> GetMiscConfigByServerId(ulong serverId)
        {
            var server = GetConfiguredServerById(serverId);
            var guild = _discord.GetGuild(serverId);
            var role = await _discordService.GetRoleByGuildAndId(server.Id, server.MentionRole);
            var roleName = "";

            if (role == null && server.MentionRole != 1)
            {
                server.MentionRole = 0;
            }

            if (server.MentionRole == 0)
            {
                roleName = "Everyone";
            }
            else if (server.MentionRole == 1)
            {
                roleName = "Here";
            }
            else
            {
                roleName = role.Name.Replace("@", "");
            }

            return $"- Bot Prefix: {server.Prefix}\r\n" +
                       $"- Use Text Announcements: {server.UseTextAnnouncements}\r\n" +
                       $"- Use YTG URLS For VOD Content: {server.UseYouTubeGamingPublished}\r\n" +
                       $"- Mention Role: {roleName}\r\n" +
                       $"- Time Zone Offset: {server.TimeZoneOffset}\r\n" +
                       $"- Discover Twitch: {(string.IsNullOrEmpty(server.DiscoverTwitch) ? "None" : server.DiscoverTwitch.FirstLetterToUpper())}\r\n" +
                       $"- Discover Twitch Role: {(server.DiscoverTwitchRole == 0 ? "None" : guild.GetRole(server.DiscoverTwitchRole).Name)}\r\n";
        }
    }
}