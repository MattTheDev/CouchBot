using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MTD.CouchBot.Bot;
using MTD.CouchBot.Domain;
using MTD.CouchBot.Domain.Models;
using MTD.CouchBot.Domain.Utilities;
using MTD.CouchBot.Json;
using MTD.CouchBot.Managers;
using MTD.CouchBot.Managers.Implementations;
using MTD.CouchBot.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace MTD.CouchBot
{
    public class Program
    {
        #region : Class Level Variables :

        private CommandService commands;
        public static DiscordSocketClient client;
        public static BeamClient beamClient;
        private DependencyMap map;

        private static Timer twitchTimer;
        private static Timer twitchServerTimer;
        private static Timer youtubeTimer;
        private static Timer youtubeServerTimer;
        private static Timer youtubePublishedTimer;
        private static Timer cleanupTimer;
        private static Timer uptimeTimer;
        private static Timer hitboxTimer;
        private static Timer hitboxServerTimer;
        private static Timer birthdayTimer;
        private static Timer beamClientTimer;
        private bool initialServicesRan = false;

        IStatisticsManager statisticsManager;
        IYouTubeManager youtubeManager;
        ITwitchManager twitchManager;
        IHitboxManager hitboxManager;
        IBeamManager beamManager;

        #endregion

        static void Main(string[] args) => new Program().Start().GetAwaiter().GetResult();

        public async Task Start()
        {
            statisticsManager = new StatisticsManager();
            youtubeManager = new YouTubeManager();
            twitchManager = new TwitchManager();
            beamManager = new BeamManager();
            hitboxManager = new HitboxManager();

            statisticsManager.LogRestartTime();
            statisticsManager.ClearRandomInts();

            //Validate BotSettings exists.
            BotFiles.CheckConfiguration();

            if ((Constants.DiscordToken == "" || Constants.DiscordToken == "DISCORDTOKEN")
                || (Constants.YouTubeApiKey == "" || Constants.YouTubeApiKey == "YOUTUBEAPI")
                || (Constants.TwitchClientId == "" || Constants.TwitchClientId == "TWITCHID"))
            {
                Console.WriteLine("Please configure the bot. The settings can be found at: " + Constants.ConfigRootDirectory + Constants.BotSettings);
                Console.WriteLine("You need to have a Discord Token, YouTube API Key, and Twitch Client Id configured for the bot to function.");
                Console.ReadLine();
                return;
            }

            // Setup file system
            BotFiles.CheckFolderStructure();

            await DoBotStuff();
            //await ValidateGuildData();
            //await ValidateUserData();

            await ResubscribeToBeamEvents();

            // Queue up timer jobs.
            QueueTwitchChecks();
            QueueYouTubeChecks();
            QueueHitboxChecks();

            QueueCleanUp();
            QueueUptimeCheckIn();
            QueueBeamClientCheck();

            await Task.Delay(-1);
        }    

        public async Task DoBotStuff()
        {
            map = new DependencyMap();
            client = new DiscordSocketClient();
            commands = new CommandService();

            await InstallCommands();
            await client.LoginAsync(TokenType.Bot, Constants.DiscordToken);
            await client.StartAsync();

            var test = client.Guilds;

            ConfigureEventHandlers();
        }

        public async Task ValidateGuildData()
        {
            var serverFiles = Directory.GetFiles(Constants.ConfigRootDirectory + Constants.GuildDirectory);
            var totalServers = 0;
            var removedServers = 0;

            //foreach (var serverFile in serverFiles)
            //{
            //    totalServers++;
            //    var serverId = Path.GetFileNameWithoutExtension(serverFile);

            //    var serverJson = File.ReadAllText(serverFile);
            //    var server = JsonConvert.DeserializeObject<DiscordServer>(serverJson);

            //    var guild = client.GetGuild(ulong.Parse(serverId));

            //    if (guild == null)
            //    {
            //        File.Delete(Constants.ConfigRootDirectory + Constants.GuildDirectory + serverId + ".json");
            //        removedServers++;
            //    }

            //    if (guild != null)
            //    {
            //        server.Name = guild.Name;
            //        var owner = guild.Owner;
                    
            //        // Validate Guild Owner Name
            //        if (owner != null)
            //        {
            //            server.OwnerName = owner.Username;
            //        }

            //        // Validate Messaging
            //        if(string.IsNullOrEmpty(server.LiveMessage))
            //        {
            //            server.LiveMessage = "%CHANNEL% just went live with %GAME% - %TITLE% - %URL%";
            //        }

            //        if(string.IsNullOrEmpty(server.PublishedMessage))
            //        {
            //            server.PublishedMessage = "%CHANNEL% just published a new video - %TITLE% - %URL%";
            //        }

            //        serverJson = JsonConvert.SerializeObject(server);

            //        File.Delete(serverFile);
            //        File.WriteAllText(serverFile, serverJson);
            //    }
            //}

            Logging.LogInfo("Server Validating Complete. " + totalServers + " servers validated. " + removedServers + " servers removed.");
        }
        
        public async Task ResubscribeToBeamEvents()
        {
            beamClient = new BeamClient();
            var servers = BotFiles.GetConfiguredServers().Where(x => x.ServerBeamChannelIds != null && x.ServerBeamChannelIds.Count > 0);

            await Task.Run(async () =>
             {
                 await beamClient.RunWebSockets();
             });

            if(servers != null && servers.Count() > 0)
            {
                foreach(var s in servers)
                {
                    foreach(var b in s.ServerBeamChannelIds)
                    {
                        Console.WriteLine("Resub to: " + b);
                        await beamClient.SubscribeToLiveAnnouncements(b);
                    }
                }
            }
        }

        public void QueueBeamClientCheck()
        {
            beamClientTimer = new Timer(async (e) =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                Logging.LogBeam("Checking Beam Constellation");

                if (beamClient.Status() != WebSocketState.Open)
                {
                    await ResubscribeToBeamEvents();
                }
                else
                {
                    Logging.LogBeam("[BEAM CONSTELLATION STATE] " + beamClient.Status());
                }

                sw.Stop();
                Logging.LogBeam("Beam Constellation Complete - Elapsed Runtime: " + sw.ElapsedMilliseconds / 1000);
            }, null, 0, 60000);
        }

        public void QueueHitboxChecks()
        {
            hitboxServerTimer = new Timer(async (e) =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                Logging.LogBeam("Checking Server Hitbox");
                await CheckServerHitboxLive();
                sw.Stop();
                Logging.LogBeam("Server Hitbox Complete - Elapsed Runtime: " + sw.ElapsedMilliseconds / 1000);
            }, null, 0, 120000);
        }

        public void QueueTwitchChecks()
        {
            twitchServerTimer = new Timer(async (e) =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                Logging.LogTwitch("Checking Server Twitch");
                await CheckServerTwitchLive();
                sw.Stop();
                Logging.LogTwitch("Server Twitch Complete - Elapsed Runtime: " + sw.ElapsedMilliseconds / 1000);
                initialServicesRan = true;
            }, null, 0, 300000);
        }

        public void QueueYouTubeChecks()
        {
            youtubeServerTimer = new Timer(async (e) =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                Logging.LogYouTube("Checking Server YouTube");
                await CheckServerYouTubeLive();
                sw.Stop();
                Logging.LogYouTube("Server YouTube Complete - Elapsed Runtime: " + sw.ElapsedMilliseconds / 1000);
            }, null, 0, 300000);

            youtubePublishedTimer = new Timer(async (e) =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                Logging.LogYouTube("Checking YouTube Published");
                await CheckPublishedYouTube();
                sw.Stop();
                Logging.LogYouTube("YouTube Published Complete - Elapsed Runtime: " + sw.ElapsedMilliseconds / 1000);
            }, null, 0, 900000);
        }

        public async Task BroadcastMessage(string message)
        {
            var serverFiles = Directory.GetFiles(Constants.ConfigRootDirectory + Constants.GuildDirectory);

            foreach (var serverFile in serverFiles)
            {
                var serverId = Path.GetFileNameWithoutExtension(serverFile);
                var serverJson = File.ReadAllText(Constants.ConfigRootDirectory + Constants.GuildDirectory + serverId + ".json");
                var server = JsonConvert.DeserializeObject<DiscordServer>(serverJson);

                if (!string.IsNullOrEmpty(server.AnnouncementsChannel.ToString()) && server.AnnouncementsChannel != 0)
                {
                    var chat = await DiscordHelper.GetMessageChannel(server.Id, server.AnnouncementsChannel);
                    
                    if (chat != null)
                    {
                        try
                        {
                            await chat.SendMessageAsync("**[CouchBot]** " + message);
                        }
                        catch (Exception ex)
                        {
                            Logging.LogError("Broadcast Message Error: " + ex.Message + " in server " + serverId);
                        }
                    }
                }
            }
        }

        public async Task Client_JoinedGuild(IGuild arg)
        {
            await CreateGuild(arg);
        }

        public async Task Client_LeftGuild(IGuild arg)
        {
            File.Delete(Constants.ConfigRootDirectory + Constants.GuildDirectory + arg.Id + ".json");
        }

        public async Task InstallCommands()
        {
            client.MessageReceived += HandleCommand;

            await commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        public async Task HandleCommand(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;
            if (message == null) return;

            int argPos = 0;

            if (!(message.HasStringPrefix("!cb ", ref argPos) || message.HasMentionPrefix(client.CurrentUser, ref argPos))) return;

            var context = new CommandContext(client, message);

            var result = await commands.ExecuteAsync(context, argPos, map);
        }

        public async Task CheckServerTwitchLive()
        {
            var servers = BotFiles.GetConfiguredServers();
            var users = BotFiles.GetConfiguredUsers();
            var liveChannels = BotFiles.GetCurrentlyLiveTwitchChannels();

            // Loop through servers to broadcast.
            foreach (var server in servers)
            {
                if (server.Id == 0 || server.GoLiveChannel == 0)
                { continue; }

                if (server.ServerTwitchChannels != null && server.ServerTwitchChannelIds != null)
                {

                    TwitchStreamsV5 streams = null;

                    try
                    {
                        // Query Twitch for our stream.
                        streams = await twitchManager.GetStreamsByIdList(server.ServerTwitchChannelIds);
                    }
                    catch (Exception wex)
                    {
                        // Log our error and move to the next user.

                        Logging.LogError("Twitch Server Error: " + wex.Message + " in Discord Server Id: " + server.Id);
                        continue;
                    }

                    if(streams == null || streams.streams == null || streams.streams.Count < 1)
                    {
                        continue;
                    }

                    foreach (var stream in streams.streams)
                    {
                        // Get currently live channel from Live/Twitch, if it exists.
                        var channel = liveChannels.FirstOrDefault(x => x.Name == stream.channel._id.ToString());

                        if (stream != null)
                        {
                            var chat = await DiscordHelper.GetMessageChannel(server.Id, server.GoLiveChannel);

                            if(chat == null)
                            {
                                continue;
                            }

                            // Is this user live already? Have they been announced on the server in question?
                            bool checkChannelBroadcastStatus = channel == null || !channel.Servers.Contains(server.Id);
                            bool checkGoLive = !string.IsNullOrEmpty(server.GoLiveChannel.ToString()) && server.GoLiveChannel != 0;
                            bool allowEveryone = server.AllowEveryone;

                            if (checkChannelBroadcastStatus)
                            {
                                if (checkGoLive)
                                {
                                    if (chat != null)
                                    {
                                        if (channel == null)
                                        {
                                            channel = new LiveChannel()
                                            {
                                                Name = stream.channel._id.ToString(),
                                                Servers = new List<ulong>()
                                            };

                                            channel.Servers.Add(server.Id);

                                            liveChannels.Add(channel);
                                        }
                                        else
                                        {
                                            channel.Servers.Add(server.Id);
                                        }

                                        string url = stream.channel.url;

                                        EmbedBuilder embed = new EmbedBuilder();
                                        EmbedAuthorBuilder author = new EmbedAuthorBuilder();
                                        EmbedFooterBuilder footer = new EmbedFooterBuilder();

                                        if (server.LiveMessage == null)
                                        {
                                            server.LiveMessage = "%CHANNEL% just went live with %GAME% - %TITLE% - %URL%";
                                        }

                                        Color purple = new Color(100, 65, 164);
                                        author.IconUrl = client.CurrentUser.GetAvatarUrl() + "?_=" + Guid.NewGuid().ToString().Replace("-", "");
                                        author.Name = "CouchBot";
                                        author.Url = url;
                                        footer.Text = "[Twitch] - " + DateTime.UtcNow.AddHours(server.TimeZoneOffset);
                                        footer.IconUrl = "http://couchbot.io/img/twitch.jpg";
                                        embed.Author = author;
                                        embed.Color = purple;
                                        embed.Description = server.LiveMessage.Replace("%CHANNEL%", stream.channel.display_name.Replace("_", "").Replace("*", "")).Replace("%GAME%", stream.game).Replace("%TITLE%", stream.channel.status).Replace("%URL%", url);
                                        embed.Title = stream.channel.display_name + " has gone live!";
                                        embed.ThumbnailUrl = stream.channel.logo != null ? stream.channel.logo + "?_=" + Guid.NewGuid().ToString().Replace("-", "") : "https://static-cdn.jtvnw.net/jtv_user_pictures/xarth/404_user_70x70.png";
                                        embed.ImageUrl = server.AllowThumbnails ? stream.preview.large + "?_=" + Guid.NewGuid().ToString().Replace("-", "") : "";
                                        embed.Footer = footer;

                                        var role = await DiscordHelper.GetRoleByGuildAndId(server.Id, server.MentionRole);

                                        if (role == null)
                                        {
                                            server.MentionRole = 0;
                                        }

                                        var message = (allowEveryone ? server.MentionRole != 0 ? role.Mention : "@everyone " : "");

                                        if (server.UseTextAnnouncements)
                                        {
                                            if(!server.AllowThumbnails)
                                            {
                                                url = "<" + url + ">";
                                            }

                                            message += "**[Twitch]** " + server.LiveMessage.Replace("%CHANNEL%", stream.channel.display_name.Replace("_", "").Replace("*", "")).Replace("%GAME%", stream.game).Replace("%TITLE%", stream.channel.status).Replace("%URL%", url);
                                        }

                                        var finalCheck = BotFiles.GetCurrentlyLiveTwitchChannels().FirstOrDefault(x => x.Name == stream.channel._id.ToString());

                                        if (finalCheck == null || !finalCheck.Servers.Contains(server.Id))
                                        {
                                            if (channel.ChannelMessages == null)
                                                channel.ChannelMessages = new List<ChannelMessage>();

                                            channel.ChannelMessages.Add(await SendMessage(new BroadcastMessage()
                                            {
                                                GuildId = server.Id,
                                                ChannelId = server.GoLiveChannel,
                                                UserId = stream.channel._id.ToString(),
                                                Message = message,
                                                Platform = "Twitch",
                                                Embed = (!server.UseTextAnnouncements ? embed.Build() : null)
                                            }));


                                            File.WriteAllText(Constants.ConfigRootDirectory + Constants.LiveDirectory + Constants.TwitchDirectory + stream.channel._id.ToString() + ".json",
                                                JsonConvert.SerializeObject(channel));
                                        }
                                    }

                                }
                            }
                        }
                    }
                }
            }
        }

        public async Task CheckServerYouTubeLive()
        {
            var servers = BotFiles.GetConfiguredServers();
            var users = BotFiles.GetConfiguredUsers();
            var liveChannels = BotFiles.GetCurrentlyLiveYouTubeChannels();

            // Loop through servers to broadcast.
            foreach (var server in servers)
            {
                if (server.Id == 0 || server.GoLiveChannel == 0)
                { continue; }

                if (server.ServerYouTubeChannelIds != null)
                {
                    foreach (var youtubeChannelId in server.ServerYouTubeChannelIds)
                    {
                        var channel = liveChannels.FirstOrDefault(x => x.Name.ToLower() == youtubeChannelId.ToLower());

                        YouTubeSearchListChannel streamResult = null;

                        try
                        {
                            // Query Youtube for our stream.
                            streamResult = await youtubeManager.GetLiveVideoByChannelId(youtubeChannelId);
                        }
                        catch (Exception wex)
                        {
                            // Log our error and move to the next user.

                            Logging.LogError("YouTube Error: " + wex.Message + " for user: " + youtubeChannelId);
                            continue;
                        }

                        // if our stream isnt null, and we have a return from yt.
                        if (streamResult != null && streamResult.items.Count > 0)
                        {
                            var stream = streamResult.items[0];

                            bool allowEveryone = server.AllowEveryone;
                            var chat = await DiscordHelper.GetMessageChannel(server.Id, server.GoLiveChannel);

                            if(chat == null)
                            {
                                continue;
                            }

                            bool checkChannelBroadcastStatus = channel == null || !channel.Servers.Contains(server.Id);
                            bool checkGoLive = !string.IsNullOrEmpty(server.GoLiveChannel.ToString()) && server.GoLiveChannel != 0;

                            if (checkChannelBroadcastStatus)
                            {
                                if (checkGoLive)
                                {
                                    if (chat != null)
                                    {
                                        if (channel == null)
                                        {
                                            channel = new LiveChannel()
                                            {
                                                Name = youtubeChannelId,
                                                Servers = new List<ulong>()
                                            };

                                            channel.Servers.Add(server.Id);

                                            liveChannels.Add(channel);
                                        }
                                        else
                                        {
                                            channel.Servers.Add(server.Id);
                                        }

                                        string url = "http://" + (server.UseYouTubeGamingPublished ? "gaming" : "www") + ".youtube.com/watch?v=" + stream.id;

                                        EmbedBuilder embed = new EmbedBuilder();
                                        EmbedAuthorBuilder author = new EmbedAuthorBuilder();
                                        EmbedFooterBuilder footer = new EmbedFooterBuilder();

                                        var channelData = await youtubeManager.GetYouTubeChannelSnippetById(stream.snippet.channelId);

                                        if (server.LiveMessage == null)
                                        {
                                            server.LiveMessage = "%CHANNEL% just went live with %GAME% - %TITLE% - %URL%";
                                        }

                                        Color red = new Color(179, 18, 23);
                                        author.IconUrl = client.CurrentUser.GetAvatarUrl() + "?_=" + Guid.NewGuid().ToString().Replace("-", "");
                                        author.Name = "CouchBot";
                                        author.Url = url;
                                        footer.Text = "[YouTube Gaming] - " + DateTime.UtcNow.AddHours(server.TimeZoneOffset);
                                        footer.IconUrl = "http://couchbot.io/img/ytg.jpg";
                                        embed.Author = author;
                                        embed.Color = red;
                                        embed.Description = server.LiveMessage.Replace("%CHANNEL%", stream.snippet.channelTitle).Replace("%GAME%", "a game").Replace("%TITLE%", stream.snippet.title).Replace("%URL%", url);
                                        embed.Title = stream.snippet.channelTitle + " has gone live!";
                                        embed.ThumbnailUrl = channelData.items.Count > 0 ? channelData.items[0].snippet.thumbnails.high.url + "?_=" + Guid.NewGuid().ToString().Replace("-", "") : "";
                                        embed.ImageUrl = server.AllowThumbnails ? stream.snippet.thumbnails.high.url + "?_=" + Guid.NewGuid().ToString().Replace("-", "") : "";
                                        embed.Footer = footer;

                                        var role = await DiscordHelper.GetRoleByGuildAndId(server.Id, server.MentionRole);

                                        if (role == null)
                                        {
                                            server.MentionRole = 0;
                                        }

                                        var message = (allowEveryone ? server.MentionRole != 0 ? role.Mention : "@everyone " : "");

                                        if (server.UseTextAnnouncements)
                                        {
                                            if (!server.AllowThumbnails)
                                            {
                                                url = "<" + url + ">";
                                            }

                                            message += "**[YouTube Gaming]** " + server.LiveMessage.Replace("%CHANNEL%", stream.snippet.channelTitle).Replace("%GAME%", "a game").Replace("%TITLE%", stream.snippet.title).Replace("%URL%", url);
                                        }


                                        var finalCheck = BotFiles.GetCurrentlyLiveYouTubeChannels().FirstOrDefault(x => x.Name == youtubeChannelId);

                                        if (finalCheck == null || !finalCheck.Servers.Contains(server.Id))
                                        {
                                            if (channel.ChannelMessages == null)
                                                channel.ChannelMessages = new List<ChannelMessage>();

                                            channel.ChannelMessages.Add(await SendMessage(new BroadcastMessage()
                                            {
                                                GuildId = server.Id,
                                                ChannelId = server.GoLiveChannel,
                                                UserId = youtubeChannelId,
                                                Message = message,
                                                Platform = "YouTube",
                                                Embed = (!server.UseTextAnnouncements ? embed.Build() : null)
                                            }));


                                            File.WriteAllText(Constants.ConfigRootDirectory + Constants.LiveDirectory + Constants.YouTubeDirectory + youtubeChannelId + ".json", JsonConvert.SerializeObject(channel));
                                        }
                                    }

                                }
                            }
                        }
                    }
                }
            }
        }

        public async Task CheckServerHitboxLive()
        {
            var servers = BotFiles.GetConfiguredServers();
            var users = BotFiles.GetConfiguredUsers();
            var liveChannels = BotFiles.GetCurrentlyLiveHitboxChannels();

            // Loop through servers to broadcast.
            foreach (var server in servers)
            {
                if (server.Id == 0 || server.GoLiveChannel == 0)
                { continue; }

                if (server.ServerHitboxChannels != null)
                {
                    foreach (var hitboxChannel in server.ServerHitboxChannels)
                    {
                        var channel = liveChannels.FirstOrDefault(x => x.Name.ToLower() == hitboxChannel.ToLower());

                        HitboxChannel stream = null;

                        try
                        {
                            // Query Beam for our stream.
                            stream = await hitboxManager.GetChannelByName(hitboxChannel);
                        }
                        catch (Exception wex)
                        {
                            // Log our error and move to the next user.

                            Logging.LogError("Hitbox Error: " + wex.Message + " for user: " + hitboxChannel + " in Discord Server Id: " + server.Id);
                            continue;
                        }

                        // if our stream isnt null, and we have a return from beam.
                        if (stream != null && stream.livestream != null && stream.livestream.Count > 0)
                        {
                            if (stream.livestream[0].media_is_live == "1")
                            {
                                bool allowEveryone = server.AllowEveryone;
                                var chat = await DiscordHelper.GetMessageChannel(server.Id, server.GoLiveChannel);

                                if (chat == null)
                                {
                                    continue;
                                }

                                bool checkChannelBroadcastStatus = channel == null || !channel.Servers.Contains(server.Id);
                                bool checkGoLive = !string.IsNullOrEmpty(server.GoLiveChannel.ToString()) && server.GoLiveChannel != 0;

                                if (checkChannelBroadcastStatus)
                                {
                                    if (checkGoLive)
                                    {
                                        if (chat != null)
                                        {
                                            if (channel == null)
                                            {
                                                channel = new LiveChannel()
                                                {
                                                    Name = hitboxChannel,
                                                    Servers = new List<ulong>()
                                                };

                                                channel.Servers.Add(server.Id);

                                                liveChannels.Add(channel);
                                            }
                                            else
                                            {
                                                channel.Servers.Add(server.Id);
                                            }

                                            string gameName = stream.livestream[0].category_name == null ? "a game" : stream.livestream[0].category_name;
                                            string url = "http://hitbox.tv/" + hitboxChannel;

                                            EmbedBuilder embed = new EmbedBuilder();
                                            EmbedAuthorBuilder author = new EmbedAuthorBuilder();
                                            EmbedFooterBuilder footer = new EmbedFooterBuilder();

                                            if (server.LiveMessage == null)
                                            {
                                                server.LiveMessage = "%CHANNEL% just went live with %GAME% - %TITLE% - %URL%";
                                            }

                                            Color green = new Color(153, 204, 0);
                                            author.IconUrl = client.CurrentUser.GetAvatarUrl() + "?_=" + Guid.NewGuid().ToString().Replace("-", "");
                                            author.Name = "CouchBot";
                                            author.Url = url;
                                            footer.Text = "[Hitbox] - " + DateTime.UtcNow.AddHours(server.TimeZoneOffset);
                                            footer.IconUrl = "http://couchbot.io/img/hitbox.jpg";
                                            embed.Author = author;
                                            embed.Color = green;
                                            embed.Description = server.LiveMessage.Replace("%CHANNEL%", hitboxChannel).Replace("%GAME%", gameName).Replace("%TITLE%", stream.livestream[0].media_status).Replace("%URL%", url);
                                            embed.Title = hitboxChannel + " has gone live!";
                                            embed.ThumbnailUrl = "http://edge.sf.hitbox.tv" + stream.livestream[0].channel.user_logo + "?_=" + Guid.NewGuid().ToString().Replace("-", "");
                                            embed.ImageUrl = server.AllowThumbnails ? "http://edge.sf.hitbox.tv" + stream.livestream[0].media_thumbnail_large + "?_=" + Guid.NewGuid().ToString().Replace("-", "") : "";
                                            embed.Footer = footer;

                                            var role = await DiscordHelper.GetRoleByGuildAndId(server.Id, server.MentionRole);

                                            if (role == null)
                                            {
                                                server.MentionRole = 0;
                                            }

                                            var message = (allowEveryone ? server.MentionRole != 0 ? role.Mention : "@everyone " : "");

                                            if (server.UseTextAnnouncements)
                                            {
                                                if (!server.AllowThumbnails)
                                                {
                                                    url = "<" + url + ">";
                                                }

                                                message += "**[Hitbox]** " + server.LiveMessage.Replace("%CHANNEL%", hitboxChannel).Replace("%GAME%", gameName).Replace("%TITLE%", stream.livestream[0].media_status).Replace("%URL%", url);
                                            }

                                            var finalCheck = BotFiles.GetCurrentlyLiveHitboxChannels().FirstOrDefault(x => x.Name == hitboxChannel);

                                            if (finalCheck == null || !finalCheck.Servers.Contains(server.Id))
                                            {
                                                if (channel.ChannelMessages == null)
                                                    channel.ChannelMessages = new List<ChannelMessage>();

                                                channel.ChannelMessages.Add(await SendMessage(new BroadcastMessage()
                                                {
                                                    GuildId = server.Id,
                                                    ChannelId = server.GoLiveChannel,
                                                    UserId = hitboxChannel,
                                                    Message = message,
                                                    Platform = "Hitbox",
                                                    Embed = (!server.UseTextAnnouncements ? embed.Build() : null)
                                                }));

                                                File.WriteAllText(Constants.ConfigRootDirectory + Constants.LiveDirectory + Constants.HitboxDirectory + hitboxChannel + ".json", JsonConvert.SerializeObject(channel));
                                            }
                                        }

                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public async Task CheckPublishedYouTube()
        {
            var servers = BotFiles.GetConfiguredServers();
            var users = BotFiles.GetConfiguredUsers();
            var liveChannels = new List<LiveChannel>();
            var now = DateTime.UtcNow;
            var then = now.AddMinutes(-15);

            foreach (var server in servers)
            {
                // If server isnt set or published channel isnt set, skip it.
                if (server.Id == 0 || server.PublishedChannel == 0)
                {
                    continue;
                }

                // If they dont allow published, skip it.
                if (!server.AllowPublished)
                {
                    continue;
                }
                                
                var chat = await DiscordHelper.GetMessageChannel(server.Id, server.PublishedChannel);

                if (chat == null)
                {
                    continue;
                }

                if (server.ServerYouTubeChannelIds == null || server.ServerYouTubeChannelIds.Count < 0)
                {
                    continue;
                }

                foreach (var user in server.ServerYouTubeChannelIds)
                {
                    if (string.IsNullOrEmpty(user))
                    {
                        continue;
                    }

                    YouTubePlaylist playlist = null;

                    try
                    {
                        var details = await youtubeManager.GetContentDetailsByChannelId(user);

                        if (details == null || details.items == null || details.items.Count < 1 || string.IsNullOrEmpty(details.items[0].contentDetails.relatedPlaylists.uploads))
                        {
                            continue;
                        }

                        playlist = await youtubeManager.GetPlaylistItemsByPlaylistId(details.items[0].contentDetails.relatedPlaylists.uploads);

                        if (playlist == null || playlist.items == null || playlist.items.Count < 1)
                        {
                            continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logging.LogError("YouTube Published Error: " + ex.Message + " for user: " + user + " in Discord Server: " + server.Id);
                        continue;
                    }

                    foreach (var video in playlist.items)
                    {
                        var publishDate = DateTime.Parse(video.snippet.publishedAt, null, System.Globalization.DateTimeStyles.AdjustToUniversal);

                        if (!(publishDate > then && publishDate < now))
                        {
                            continue;
                        }

                        string url = "http://" + (server.UseYouTubeGamingPublished ? "gaming" : "www") + ".youtube.com/watch?v=" + video.snippet.resourceId.videoId;

                        EmbedBuilder embed = new EmbedBuilder();
                        EmbedAuthorBuilder author = new EmbedAuthorBuilder();
                        EmbedFooterBuilder footer = new EmbedFooterBuilder();

                        var channelData = await youtubeManager.GetYouTubeChannelSnippetById(video.snippet.channelId);

                        if (server.PublishedMessage == null)
                        {
                            server.PublishedMessage = "%CHANNEL% just went live with %GAME% - %TITLE% - %URL%";
                        }

                        Color red = new Color(179, 18, 23);
                        author.IconUrl = client.CurrentUser.GetAvatarUrl() + "?_=" + Guid.NewGuid().ToString().Replace("-", "");
                        author.Name = "CouchBot";
                        author.Url = url;
                        footer.Text = "[YouTube] - " + DateTime.UtcNow.AddHours(server.TimeZoneOffset);
                        footer.IconUrl = "http://couchbot.io/img/ytg.jpg";
                        embed.Author = author;
                        embed.Color = red;
                        embed.Description = server.PublishedMessage.Replace("%CHANNEL%", video.snippet.channelTitle).Replace("%GAME%", "a game").Replace("%TITLE%", video.snippet.title).Replace("%URL%", url);
                        embed.Title = video.snippet.channelTitle + " published a new video!";
                        embed.ThumbnailUrl = channelData.items.Count > 0 ? channelData.items[0].snippet.thumbnails.high.url + "?_=" + Guid.NewGuid().ToString().Replace("-", "") : "";
                        embed.ImageUrl = server.AllowThumbnails ? video.snippet.thumbnails.high.url + "?_=" + Guid.NewGuid().ToString().Replace("-", "") : "";
                        embed.Footer = footer;

                        var lc = liveChannels.FirstOrDefault(x => x.Name.ToLower() == user.ToLower());
                        if (lc == null)
                        {
                            lc = new LiveChannel();
                            lc.Servers = new List<ulong>();
                        }

                        if (lc == null || lc.Servers.Count < 1 || (!lc.Servers.Contains(server.Id) && !lc.Name.Equals(user + "|" + video.snippet.resourceId.videoId)))
                        {
                            var role = await DiscordHelper.GetRoleByGuildAndId(server.Id, server.MentionRole);

                            if (role == null)
                            {
                                server.MentionRole = 0;
                            }

                            var message = (server.AllowEveryone ? server.MentionRole != 0 ? role.Mention : "@everyone " : "");

                            if (server.UseTextAnnouncements)
                            {
                                if (!server.AllowThumbnails)
                                {
                                    url = "<" + url + ">";
                                }

                                message += "**[YouTube]** " + server.PublishedMessage.Replace("%CHANNEL%", video.snippet.channelTitle).Replace("%TITLE%", video.snippet.title).Replace("%URL%", url);
                            }

                            await SendMessage(new BroadcastMessage()
                            {
                                GuildId = server.Id,
                                ChannelId = server.PublishedChannel,
                                UserId = user,
                                Message = message,
                                Platform = "YouTube",
                                Embed = (!server.UseTextAnnouncements ? embed.Build() : null)
                            });
                        }

                        lc.Name = user + "|" + video.snippet.resourceId.videoId;
                        lc.Servers.Add(server.Id);

                        liveChannels.Add(lc);                        
                    }
                }
            }
        }

        public void QueueCleanUp()
        {
            cleanupTimer = new Timer(async (e) =>
            {
                using (var httpClient = new HttpClient())
                {
                    if (initialServicesRan)
                    {
                        Logging.LogInfo("Cleaning Up Live Files.");
                        await CleanUpLiveStreams("youtube");
                        await CleanUpLiveStreams("twitch");
                        await CleanUpLiveStreams("hitbox");
                        Logging.LogInfo("Cleaning Up Live Files Complete.");
                    }
                }
            }, null, 0, 600000);
        }

        public void QueueUptimeCheckIn()
        {
            uptimeTimer = new Timer((e) =>
            {
                using (var httpClient = new HttpClient())
                {
                    Logging.LogInfo("Adding to Uptime.");
                    statisticsManager.AddUptimeMinutes();
                    Logging.LogInfo("Uptime Update Complete.");
                }
            }, null, 0, 300000);
        }

        public void ConfigureEventHandlers()
        {
            client.JoinedGuild += Client_JoinedGuild;
            client.LeftGuild += Client_LeftGuild;
            client.UserJoined += Client_UserJoined;
            client.UserLeft += Client_UserLeft;
        }

        private async Task Client_UserLeft(IGuildUser arg)
        {
            UpdateGuildUsers(arg.Guild);

            var guild = new DiscordServer();
            var guildFile = Constants.ConfigRootDirectory + Constants.GuildDirectory + arg.Guild.Id + ".json";

            if (File.Exists(guildFile))
            {
                var json = File.ReadAllText(guildFile);
                guild = JsonConvert.DeserializeObject<DiscordServer>(json);
            }

            if (guild != null)
            {
                if (guild.GreetingsChannel != 0 && guild.Goodbyes)
                {
                    var channel = (IMessageChannel)await arg.Guild.GetChannelAsync(guild.GreetingsChannel);

                    if(string.IsNullOrEmpty(guild.GoodbyeMessage))
                    {
                        guild.GoodbyeMessage = "Good bye, " + arg.Username + ", thanks for hanging out!";
                    }

                    guild.GoodbyeMessage = guild.GoodbyeMessage.Replace("%USER%", arg.Username).Replace("%NEWLINE%","\r\n");

                    await channel.SendMessageAsync(guild.GoodbyeMessage);
                }
            }
        }

        private async Task Client_UserJoined(IGuildUser arg)
        {
            UpdateGuildUsers(arg.Guild);

            var guild = new DiscordServer();
            var guildFile = Constants.ConfigRootDirectory + Constants.GuildDirectory + arg.Guild.Id + ".json";

            if (File.Exists(guildFile))
            {
                var json = File.ReadAllText(guildFile);
                guild = JsonConvert.DeserializeObject<DiscordServer>(json);
            }

            if (guild != null)
            {
                if (guild.GreetingsChannel != 0 && guild.Greetings)
                {
                    var channel = (IMessageChannel)await arg.Guild.GetChannelAsync(guild.GreetingsChannel);

                    if (string.IsNullOrEmpty(guild.GreetingMessage))
                    {
                        guild.GreetingMessage = "Welcome to the server, " + arg.Mention;
                    }

                    guild.GreetingMessage = guild.GreetingMessage.Replace("%USER%", arg.Mention).Replace("%NEWLINE%", "\r\n");

                    await channel.SendMessageAsync(guild.GreetingMessage);
                }
            }
        }

        public async Task CreateGuild(IGuild arg)
        {
            var guild = new DiscordServer();
            var guildFile = Constants.ConfigRootDirectory + Constants.GuildDirectory + arg.Id + ".json";

            if (File.Exists(guildFile))
            {
                var json = File.ReadAllText(guildFile);
                guild = JsonConvert.DeserializeObject<DiscordServer>(json);
            }

            if (guild.Users == null)
                guild.Users = new List<string>();

            foreach (var user in await arg.GetUsersAsync())
            {
                guild.Users.Add(user.Id.ToString());
            }

            var owner = await arg.GetUserAsync(arg.OwnerId);
            guild.Id = arg.Id;
            guild.OwnerId = arg.OwnerId;
            guild.OwnerName = owner.Username;
            guild.Name = arg.Name;
            guild.AllowEveryone = true;

            var guildJson = JsonConvert.SerializeObject(guild);
            File.WriteAllText(guildFile, guildJson);
        }

        public async Task UpdateGuildUsers(IGuild arg)
        {
            var guild = new DiscordServer();
            var guildFile = Constants.ConfigRootDirectory + Constants.GuildDirectory + arg.Id + ".json";

            if (File.Exists(guildFile))
            {
                var json = File.ReadAllText(guildFile);
                guild = JsonConvert.DeserializeObject<DiscordServer>(json);
            }

            guild.Users = new List<string>();

            foreach (var user in await arg.GetUsersAsync())
            {
                guild.Users.Add(user.Id.ToString());
            }

            var guildJson = JsonConvert.SerializeObject(guild);
            File.WriteAllText(guildFile, guildJson);
        }

        public async Task CleanUpLiveStreams(string platform)
        {
            if (platform == "twitch")
            {
                var liveStreams = new List<LiveChannel>();

                foreach (var live in Directory.GetFiles(Constants.ConfigRootDirectory + Constants.LiveDirectory + Constants.TwitchDirectory))
                {
                    var channel = JsonConvert.DeserializeObject<LiveChannel>(File.ReadAllText(live));
                    if (liveStreams.FirstOrDefault(x => x.Name == channel.Name) == null)
                    {
                        liveStreams.Add(channel);
                    }
                }

                foreach (var stream in liveStreams)
                {
                    try
                    {
                        var liveStream = await twitchManager.GetStreamById(stream.Name);

                        if (liveStream == null || liveStream.stream == null)
                        {
                            //await CleanupMessages(stream.ChannelMessages);

                            File.Delete(Constants.ConfigRootDirectory + Constants.LiveDirectory + Constants.TwitchDirectory + stream.Name + ".json");
                        }
                    }
                    catch (Exception wex)
                    {

                        Logging.LogError("Clean Up Twitch Error: " + wex.Message + " for user: " + stream.Name);
                    }
                }
            }

            if (platform == "youtube")
            {
                var liveStreams = new List<LiveChannel>();

                foreach (var live in Directory.GetFiles(Constants.ConfigRootDirectory + Constants.LiveDirectory + Constants.YouTubeDirectory))
                {
                    var channel = JsonConvert.DeserializeObject<LiveChannel>(File.ReadAllText(live));
                    if (liveStreams.FirstOrDefault(x => x.Name == channel.Name) == null)
                    {
                        liveStreams.Add(channel);
                    }
                }

                foreach (var stream in liveStreams)
                {
                    try
                    {
                        var youtubeStream = await youtubeManager.GetLiveVideoByChannelId(stream.Name);

                        if (youtubeStream == null || youtubeStream.items.Count < 1)
                        {
                            var file = Constants.ConfigRootDirectory + Constants.LiveDirectory + Constants.YouTubeDirectory + stream.Name + ".json";

                            //await CleanupMessages(stream.ChannelMessages);

                            File.Delete(file);
                        }
                    }
                    catch (Exception wex)
                    {

                        Logging.LogError("Clean Up YouTube Error: " + wex.Message + " for user: " + stream.Name);
                    }
                }
            }

            if (platform == "hitbox")
            {
                var liveStreams = new List<LiveChannel>();

                foreach (var live in Directory.GetFiles(Constants.ConfigRootDirectory + Constants.LiveDirectory + Constants.HitboxDirectory))
                {
                    var channel = JsonConvert.DeserializeObject<LiveChannel>(File.ReadAllText(live));
                    if (liveStreams.FirstOrDefault(x => x.Name == channel.Name) == null)
                    {
                        liveStreams.Add(channel);
                    }
                }

                foreach (var stream in liveStreams)
                {
                    try
                    {
                        var liveStream = await hitboxManager.GetChannelByName(stream.Name);

                        if (liveStream == null || liveStream.livestream == null || liveStream.livestream.Count < 1 || liveStream.livestream[0].media_is_live == "0")
                        {
                            //await CleanupMessages(stream.ChannelMessages);

                            File.Delete(Constants.ConfigRootDirectory + Constants.LiveDirectory + Constants.HitboxDirectory + stream.Name + ".json");
                        }
                    }
                    catch (Exception wex)
                    {

                        Logging.LogError("Clean Up Hitbox Error: " + wex.Message + " for user: " + stream.Name);
                    }
                }
            }
        }

        private async Task CleanupMessages(List<ChannelMessage> channelMessages)
        {
            if (channelMessages != null && channelMessages.Count > 0)
            {
                foreach (var channelMessage in channelMessages)
                {
                    if(!channelMessage.DeleteOffline)
                    {
                        continue;
                    }

                    var messageChannel = await DiscordHelper.GetMessageChannel(channelMessage.GuildId, channelMessage.ChannelId);
                    var message = await messageChannel.GetMessageAsync(channelMessage.MessageId);
                    IList<IMessage> msgs = new List<IMessage>();
                    msgs.Add(message);
                    await messageChannel.DeleteMessagesAsync(msgs);
                }
            }
        }

        public async Task<ChannelMessage> SendMessage(BroadcastMessage message)
        {
            var chat = await DiscordHelper.GetMessageChannel(message.GuildId, message.ChannelId);

            if (chat != null)
            {
                try
                {
                    ChannelMessage channelMessage = new ChannelMessage();
                    channelMessage.ChannelId = message.ChannelId;
                    channelMessage.GuildId = message.GuildId;
                    channelMessage.DeleteOffline = message.DeleteOffline;

                    if (message.Embed != null)
                    {
                        RequestOptions options = new RequestOptions();
                        options.RetryMode = RetryMode.AlwaysRetry;
                        var msg = await chat.SendMessageAsync(message.Message, false, message.Embed, options);

                        if(msg != null || msg.Id != 0)
                        {
                            channelMessage.MessageId = msg.Id;
                        }
                    }
                    else
                    {
                        var msg = await chat.SendMessageAsync(message.Message);

                        if (msg != null || msg.Id != 0)
                        {
                            channelMessage.MessageId = msg.Id;
                        }
                    }

                    if (message.Platform.Equals("YouTube"))
                    {
                        statisticsManager.AddToYouTubeAlertCount();
                    }

                    if (message.Platform.Equals("Twitch"))
                    {
                        statisticsManager.AddToTwitchAlertCount();
                    }

                    if (message.Platform.Equals("Beam"))
                    {
                        statisticsManager.AddToBeamAlertCount();
                    }

                    if (message.Platform.Equals("Hitbox"))
                    {
                        statisticsManager.AddToHitboxAlertCount();
                    }
                }
                catch (Exception ex)
                {
                    Logging.LogError("Send Message Error: " + ex.Message + " in server " + message.GuildId);
                }
            }

            return null; // we never get here :(
        }

        #region TODO

        //public void QueueSubGoalCheck()
        //{
        //    uptimeTimer = new Timer(async (e) =>
        //    {
        //        using (var httpClient = new HttpClient())
        //        {
        //            Logging.LogInfo("Checking Sub/Follower Goals.");
        //            await CheckSubGoals();
        //            Logging.LogInfo("Checking Sub/Follower Goals Complete.");
        //        }
        //    }, null, 0, 120000);
        //}

        //public async Task CheckSubGoals()
        //{
        //    var userFiles = Directory.GetFiles(Constants.ConfigRootDirectory + Constants.UserDirectory);

        //    foreach (var file in userFiles)
        //    {
        //        var user = JsonConvert.DeserializeObject<User>(File.ReadAllText(file));

        //        if (!string.IsNullOrEmpty(user.TwitchFollowerGoal)
        //            || !string.IsNullOrEmpty(user.BeamFollowerGoal)
        //            || !string.IsNullOrEmpty(user.YouTubeSubGoal))
        //        {

        //            var twitchName = user.TwitchName;
        //            var youtubeId = user.YouTubeChannelId;
        //            var beamName = user.BeamName;

        //            TwitchFollowers twitch = null;
        //            YouTubeChannelStatistics youtube = null;
        //            BeamChannel beam = null;

        //            if (twitchName != null)
        //            {
        //                twitch = await twitchManager.GetFollowersByName(twitchName);
        //            }

        //            if (!string.IsNullOrEmpty(youtubeId))
        //            {
        //                youtube = await youtubeManager.GetChannelStatisticsById(youtubeId);
        //            }

        //            if (!string.IsNullOrEmpty(beamName))
        //            {
        //                beam = await beamManager.GetBeamChannelByName(beamName);
        //            }

        //            var serverFiles = Directory.GetFiles(Constants.ConfigRootDirectory + Constants.GuildDirectory);

        //            foreach (var serverFile in serverFiles)
        //            {
        //                var server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(serverFile));

        //                if (((!server.BroadcastOthers && user.Id == server.OwnerId) || server.BroadcastOthers) && server.BroadcastSubGoals && server.Users.Contains(user.Id.ToString()))
        //                {
        //                    if (twitch != null && !string.IsNullOrEmpty(user.TwitchFollowerGoal))
        //                    {
        //                        if (twitch._total >= (int.Parse(user.TwitchFollowerGoal)) && !user.TwitchFollowerGoalMet)
        //                        {
        //                            user.TwitchFollowerGoalMet = true;
        //                            File.WriteAllText(Constants.ConfigRootDirectory + Constants.UserDirectory + user.Id + ".json", JsonConvert.SerializeObject(user));

        //                            var discordUser = client.GetUser(user.Id);

        //                            if (messages.Where(x => x.GuildId == server.Id).FirstOrDefault() == null)
        //                            {
        //                                messages.Add(new BroadcastMessage()
        //                                {
        //                                    GuildId = server.Id,
        //                                    ChannelId = server.AnnouncementsChannel,
        //                                    UserId = user.Id,
        //                                    Message = (server.AllowEveryone ? "@everyone " : "") + "**[Twitch]** " + user.TwitchName + " (" + discordUser.Username + ") has broken their sub goal of " + user.TwitchFollowerGoal + "!! Congrats! <3",
        //                                    Platform = "Twitch"
        //                                });
        //                            }
        //                        }
        //                    }

        //                    if (youtube != null && !string.IsNullOrEmpty(user.YouTubeSubGoal))
        //                    {
        //                        if (youtube.items.Count > 0 && int.Parse(youtube.items[0].statistics.subscriberCount) > int.Parse(user.YouTubeSubGoal) && !user.YouTubeSubGoalMet)
        //                        {
        //                            user.YouTubeSubGoalMet = true;
        //                            File.WriteAllText(Constants.ConfigRootDirectory + Constants.UserDirectory + user.Id + ".json", JsonConvert.SerializeObject(user));

        //                            var discordUser = client.GetUser(user.Id);

        //                            if (chat != null)
        //                            {
        //                                await chat.SendMessageAsync((server.AllowEveryone ? "@everyone " : "") + "**[YouTube]** " + user.TwitchName + " (" + discordUser.Username + ") has broken their sub goal of " + user.YouTubeSubGoal + "!! Congrats! <3");
        //                            }
        //                        }
        //                    }

        //                    if (beam != null && !string.IsNullOrEmpty(user.BeamFollowerGoal))
        //                    {
        //                        if (beam.numFollowers > int.Parse(user.BeamFollowerGoal) && !user.YouTubeSubGoalMet)
        //                        {
        //                            user.BeamFollowerGoalMet = true;
        //                            File.WriteAllText(Constants.ConfigRootDirectory + Constants.UserDirectory + user.Id + ".json", JsonConvert.SerializeObject(user));

        //                            var discordUser = client.GetUser(user.Id);

        //                            if (chat != null)
        //                            {
        //                                await chat.SendMessageAsync((server.AllowEveryone ? "@everyone " : "") + "**[Beam]** " + user.TwitchName + " (" + discordUser.Username + ") has broken their sub goal of " + user.BeamFollowerGoal + "!! Congrats! <3");
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        //public void QueueBirthdays()
        //{
        //    birthdayTimer = new Timer(async (e) =>
        //    {
        //        var userFiles = Directory.GetFiles(Constants.ConfigRootDirectory + Constants.GuildDirectory);
        //        foreach(var serverFile in serverFiles)
        //        {
        //            var server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(serverFile));


        //        }
        //        await client.GetGuildAsync(1234);
        //    }, null, 0, 60000);
        //}

        #endregion
    }
}