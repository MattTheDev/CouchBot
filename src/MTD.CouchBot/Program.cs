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

        private static Timer beamClientTimer;
        private static Timer hitboxTimer;
        private static Timer hitboxOwnerTimer;
        private static Timer twitchTimer;
        private static Timer twitchOwnerTimer;
        private static Timer youtubeTimer;
        private static Timer youtubeOwnerTimer;
        private static Timer youtubePublishedTimer;
        private static Timer youtubePublishedOwnerTimer;
        private static Timer twitchFeedTimer;
        private static Timer twitchFeedOwnerTimer;

        private static Timer cleanupTimer;
        private static Timer uptimeTimer;
        private bool initialServicesRan = false;

        IStatisticsManager statisticsManager;
        IYouTubeManager youtubeManager;
        ITwitchManager twitchManager;
        ISmashcastManager smashcastManager;
        IBeamManager beamManager;

        #endregion

        static void Main(string[] args) => new Program().Start().GetAwaiter().GetResult();

        public async Task Start()
        {
            Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);

            Logging.LogInfo("Starting CouchBot.");
            Logging.LogInfo("Initializing Managers.");

            statisticsManager = new StatisticsManager();
            youtubeManager = new YouTubeManager();
            twitchManager = new TwitchManager();
            beamManager = new BeamManager();
            smashcastManager = new SmashcastManager();

            Logging.LogInfo("Managers Initialized.");
            Logging.LogInfo("Log Last Restart Time and Date.");

            statisticsManager.LogRestartTime();

            Logging.LogInfo("Clear randomly seeded Beam task Ids.");

            statisticsManager.ClearRandomInts();

            Logging.LogInfo("Check Bot Configuration.");

            BotFiles.CheckConfiguration();

            if ((Constants.DiscordToken == "" || Constants.DiscordToken == "DISCORDTOKEN")
                || (Constants.YouTubeApiKey == "" || Constants.YouTubeApiKey == "YOUTUBEAPI")
                || (Constants.TwitchClientId == "" || Constants.TwitchClientId == "TWITCHID")
                || (string.IsNullOrEmpty(Constants.Prefix)))
            {
                Console.WriteLine("Please configure the bot. The settings can be found at: " + Constants.ConfigRootDirectory + Constants.BotSettings);
                Console.WriteLine("You need to have a Discord Token, YouTube API Key, and Twitch Client Id configured for the bot to function.");
                Console.ReadLine();
                return;
            }

            Logging.LogInfo("Bot Configuration - All Set.");
            Logging.LogInfo("Check Folder Structure.");

            BotFiles.CheckFolderStructure();

            Logging.LogInfo("Folder Structure - All Set.");
            Logging.LogInfo("Do Bot Things.");

            await DoBotStuff();

            Logging.LogInfo("Bot Things Done.");
            Logging.LogInfo("Resubscribe to Beam Events.");

            await ResubscribeToBeamEvents();

            Logging.LogInfo("Subscribed to Beam Events - All Set.");
            Logging.LogInfo("Queue Timer Jobs.");

            QueueTwitchChecks();
            QueueYouTubeChecks();
            QueueHitboxChecks();

            QueueCleanUp();
            QueueUptimeCheckIn();
            QueueBeamClientCheck();

            Logging.LogInfo("Timer Jobs Queued - All Set.");

            await Task.Delay(-1);
        }

        public async Task DoBotStuff()
        {
            map = new DependencyMap();
            client = new DiscordSocketClient();
            commands = new CommandService();

            Logging.LogInfo("Installing Commands, Logging into Discord, and Starting Client.");

            await InstallCommands();
            await client.LoginAsync(TokenType.Bot, Constants.DiscordToken);
            await client.StartAsync();

            Logging.LogInfo("Commands Installed, Discord Logged Into, and Client Started - All Set.");
            Logging.LogInfo("Configuring Event Handlers.");

            ConfigureEventHandlers();

            Logging.LogInfo("Event Handlers Configured - All Set.");
        }

        public async Task ResubscribeToBeamEvents()
        {
            var count = 0;
            beamClient = new BeamClient();

            Logging.LogBeam("Staritng Beam Resubscription.");
            Logging.LogBeam("Getting Server Files.");

            var servers = BotFiles.GetConfiguredServers().Where(x => x.ServerBeamChannelIds != null && x.ServerBeamChannelIds.Count > 0);

            await Task.Run(async () =>
             {
                 Logging.LogBeam("Connecting to Beam Constellation.");

                 await beamClient.RunWebSockets();

                 Logging.LogBeam("Connected to Beam Constellation.");
             });


            Logging.LogBeam("Initiating Subscription Loop.");
            Stopwatch sw = new Stopwatch();
            sw.Start();

            if (servers != null && servers.Count() > 0)
            {
                foreach (var s in servers)
                {
                    foreach (var b in s.ServerBeamChannelIds)
                    {
                        await beamClient.SubscribeToLiveAnnouncements(b);
                        count++;
                    }
                }
            }

            sw.Stop();
            Logging.LogBeam("Subscription Loop Complete. Processed " + count + " channels in " + sw.ElapsedMilliseconds + " milliseconds.");
            Logging.LogBeam("Beam Resubscription Complete - All Set.");
        }

        public void QueueBeamClientCheck()
        {
            beamClientTimer = new Timer(async (e) =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                Logging.LogBeam("Beam Constellation Health Check Started.");

                if (beamClient.Status() != WebSocketState.Open)
                {
                    await ResubscribeToBeamEvents();
                }
                else
                {
                    Logging.LogBeam("" + beamClient.Status());
                }

                sw.Stop();
                Logging.LogBeam("Beam Constellation Health Check Complete - Elapsed Runtime: " + sw.ElapsedMilliseconds + " milliseconds.");
            }, null, 0, 60000);
        }

        public void QueueHitboxChecks()
        {
            hitboxTimer = new Timer(async (e) =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                Logging.LogHitbox("Checking Smashcast Channels.");
                await CheckHitboxLive();
                sw.Stop();
                Logging.LogHitbox("Smashcast Check Complete - Elapsed Runtime: " + sw.ElapsedMilliseconds + " milliseconds.");
            }, null, 0, 120000);

            hitboxOwnerTimer = new Timer(async (e) =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                Logging.LogHitbox("Checking Owner Smashcast Channels.");
                await CheckOwnerHitboxLive();
                sw.Stop();
                Logging.LogHitbox("Owner Smashcast Check Complete - Elapsed Runtime: " + sw.ElapsedMilliseconds + " milliseconds.");
            }, null, 0, 120000);
        }

        public void QueueTwitchChecks()
        {
            twitchTimer = new Timer(async (e) =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                Logging.LogTwitch("Checking Twitch Channels.");
                await CheckTwitchLive();
                sw.Stop();
                Logging.LogTwitch("Twitch Check Complete - Elapsed Runtime: " + sw.ElapsedMilliseconds + " milliseconds.");
                initialServicesRan = true;
            }, null, 0, 300000);

            twitchOwnerTimer = new Timer(async (e) =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                Logging.LogTwitch("Checking Owner Twitch Channels.");
                await CheckOwnerTwitchLive();
                sw.Stop();
                Logging.LogTwitch("Owner Twitch Check Complete - Elapsed Runtime: " + sw.ElapsedMilliseconds + " milliseconds.");
                initialServicesRan = true;
            }, null, 0, 120000);
        }

        public void QueueYouTubeChecks()
        {
            youtubeTimer = new Timer(async (e) =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                Logging.LogYouTubeGaming("Checking YouTube Gaming Channels.");
                await CheckYouTubeLive();
                sw.Stop();
                Logging.LogYouTubeGaming("YouTube Gaming Check Complete - Elapsed Runtime: " + sw.ElapsedMilliseconds + " milliseconds.");
            }, null, 0, 300000);

            youtubeOwnerTimer = new Timer(async (e) =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                Logging.LogYouTubeGaming("Checking Owner YouTube Gaming Channels.");
                await CheckOwnerYouTubeLive();
                sw.Stop();
                Logging.LogYouTubeGaming("Owner YouTube Gaming Check Complete - Elapsed Runtime: " + sw.ElapsedMilliseconds + " milliseconds.");
            }, null, 0, 120000);

            youtubePublishedTimer = new Timer(async (e) =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                Logging.LogYouTube("Checking YouTube Published");
                await CheckPublishedYouTube();
                sw.Stop();
                Logging.LogYouTube("YouTube Published Complete - Elapsed Runtime: " + sw.ElapsedMilliseconds + " milliseconds.");
            }, null, 0, 900000);

            youtubePublishedOwnerTimer = new Timer(async (e) =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                Logging.LogYouTube("Checking Owner YouTube Published");
                await CheckOwnerPublishedYouTube();
                sw.Stop();
                Logging.LogYouTube("Owner YouTube Published Complete - Elapsed Runtime: " + sw.ElapsedMilliseconds + " milliseconds.");
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

            if (!(message.HasStringPrefix(Constants.Prefix + " ", ref argPos) || message.HasMentionPrefix(client.CurrentUser, ref argPos))) return;

            var context = new CommandContext(client, message);

            var result = await commands.ExecuteAsync(context, argPos, map);
        }

        public async Task CheckTwitchLive()
        {
            var servers = BotFiles.GetConfiguredServers();
            var liveChannels = BotFiles.GetCurrentlyLiveTwitchChannels();

            // Loop through servers to broadcast.
            foreach (var server in servers)
            {
                if (!server.AllowLive)
                {
                    continue;
                }

                if (server.Id != 0 && server.GoLiveChannel != 0 &&
                    server.ServerTwitchChannels != null && server.ServerTwitchChannelIds != null)
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

                    if (streams == null || streams.streams == null || streams.streams.Count < 1)
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

                                    // Build our message
                                    string url = stream.channel.url;
                                    string channelName = stream.channel.display_name.Replace("_", "").Replace("*", "");
                                    string avatarUrl = stream.channel.logo != null ? stream.channel.logo : "https://static-cdn.jtvnw.net/jtv_user_pictures/xarth/404_user_70x70.png";
                                    string thumbnailUrl = stream.preview.large;

                                    Logging.LogTwitch(channelName + " has gone online.");

                                    var message = await MessagingHelper.BuildMessage(channelName, stream.game, stream.channel.status, url, avatarUrl,
                                        thumbnailUrl, Constants.Twitch, stream.channel._id.ToString(), server);

                                    var finalCheck = BotFiles.GetCurrentlyLiveTwitchChannels().FirstOrDefault(x => x.Name == stream.channel._id.ToString());

                                    if (finalCheck == null || !finalCheck.Servers.Contains(server.Id))
                                    {
                                        if (channel.ChannelMessages == null)
                                            channel.ChannelMessages = new List<ChannelMessage>();

                                        channel.ChannelMessages.AddRange(await MessagingHelper.SendMessages(Constants.Twitch, new List<BroadcastMessage>() { message }));

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

        public async Task CheckOwnerTwitchLive()
        {
            var servers = BotFiles.GetConfiguredServers();
            var liveChannels = BotFiles.GetCurrentlyLiveTwitchChannels();

            // Loop through servers to broadcast.
            foreach (var server in servers)
            {
                if (!server.AllowLive)
                {
                    continue;
                }

                if (server.Id != 0 && server.OwnerLiveChannel != 0 &&
                    !string.IsNullOrEmpty(server.OwnerTwitchChannel) && !string.IsNullOrEmpty(server.OwnerTwitchChannelId))
                {
                    TwitchStreamV5 twitchStream = null;

                    try
                    {
                        // Query Twitch for our stream.
                        twitchStream = await twitchManager.GetStreamById(server.OwnerTwitchChannelId);
                    }
                    catch (Exception wex)
                    {
                        // Log our error and move to the next user.

                        Logging.LogError("Twitch Server Error: " + wex.Message + " in Discord Server Id: " + server.Id);
                        continue;
                    }

                    if (twitchStream == null || twitchStream.stream == null)
                    {
                        continue;
                    }

                    var stream = twitchStream.stream;

                    // Get currently live channel from Live/Twitch, if it exists.
                    var channel = liveChannels.FirstOrDefault(x => x.Name == stream._id.ToString());

                    if (stream != null)
                    {
                        var chat = await DiscordHelper.GetMessageChannel(server.Id, server.OwnerLiveChannel);

                        if (chat == null)
                        {
                            continue;
                        }

                        bool checkChannelBroadcastStatus = channel == null || !channel.Servers.Contains(server.Id);
                        bool checkGoLive = !string.IsNullOrEmpty(server.OwnerLiveChannel.ToString()) && server.OwnerLiveChannel != 0;

                        if (checkChannelBroadcastStatus)
                        {
                            if (checkGoLive)
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

                                // Build our message
                                string url = stream.channel.url;
                                string channelName = stream.channel.display_name.Replace("_", "").Replace("*", "");
                                string avatarUrl = stream.channel.logo != null ? stream.channel.logo : "https://static-cdn.jtvnw.net/jtv_user_pictures/xarth/404_user_70x70.png";
                                string thumbnailUrl = stream.preview.large;

                                Logging.LogTwitch(channelName + " has gone online.");

                                var message = await MessagingHelper.BuildMessage(channelName, stream.game, stream.channel.status, url, avatarUrl,
                                    thumbnailUrl, Constants.Twitch, stream.channel._id.ToString(), server);

                                var finalCheck = BotFiles.GetCurrentlyLiveTwitchChannels().FirstOrDefault(x => x.Name == stream.channel._id.ToString());

                                if (finalCheck == null || !finalCheck.Servers.Contains(server.Id))
                                {
                                    if (channel.ChannelMessages == null)
                                        channel.ChannelMessages = new List<ChannelMessage>();

                                    channel.ChannelMessages.AddRange(await MessagingHelper.SendMessages(Constants.Twitch, new List<BroadcastMessage>() { message }));

                                    File.WriteAllText(Constants.ConfigRootDirectory + Constants.LiveDirectory + Constants.TwitchDirectory + stream.channel._id.ToString() + ".json",
                                        JsonConvert.SerializeObject(channel));
                                }
                            }
                        }
                    }
                }
            }
        }

        public async Task CheckYouTubeLive()
        {
            var servers = BotFiles.GetConfiguredServers();
            var liveChannels = BotFiles.GetCurrentlyLiveYouTubeChannels();

            // Loop through servers to broadcast.
            foreach (var server in servers)
            {
                if (!server.AllowLive)
                {
                    continue;
                }

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

                                        // Build our message
                                        var channelData = await youtubeManager.GetYouTubeChannelSnippetById(stream.snippet.channelId);

                                        string url = "http://" + (server.UseYouTubeGamingPublished ? "gaming" : "www") + ".youtube.com/watch?v=" + stream.id;
                                        string channelTitle = stream.snippet.channelTitle;
                                        string avatarUrl = channelData.items.Count > 0 ? channelData.items[0].snippet.thumbnails.high.url : "";
                                        string thumbnailUrl = stream.snippet.thumbnails.high.url;

                                        Logging.LogYouTubeGaming(channelTitle + " has gone online.");

                                        var message = await MessagingHelper.BuildMessage(channelTitle, "a game", stream.snippet.title, url, avatarUrl, thumbnailUrl,
                                            Constants.YouTubeGaming, youtubeChannelId, server);

                                        var finalCheck = BotFiles.GetCurrentlyLiveYouTubeChannels().FirstOrDefault(x => x.Name == youtubeChannelId);

                                        if (finalCheck == null || !finalCheck.Servers.Contains(server.Id))
                                        {
                                            if (channel.ChannelMessages == null)
                                                channel.ChannelMessages = new List<ChannelMessage>();

                                            channel.ChannelMessages.AddRange(await MessagingHelper.SendMessages(Constants.YouTubeGaming, new List<BroadcastMessage>() { message }));

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

        public async Task CheckOwnerYouTubeLive()
        {
            var servers = BotFiles.GetConfiguredServers();
            var liveChannels = BotFiles.GetCurrentlyLiveYouTubeChannels();

            // Loop through servers to broadcast.
            foreach (var server in servers)
            {
                if (!server.AllowLive)
                {
                    continue;
                }

                if (server.Id == 0 || server.OwnerLiveChannel == 0)
                { continue; }

                if (server.OwnerYouTubeChannelId != null)
                {
                    var channel = liveChannels.FirstOrDefault(x => x.Name.ToLower() == server.OwnerYouTubeChannelId.ToLower());

                    YouTubeSearchListChannel streamResult = null;

                    try
                    {
                        // Query Youtube for our stream.
                        streamResult = await youtubeManager.GetLiveVideoByChannelId(server.OwnerYouTubeChannelId);
                    }
                    catch (Exception wex)
                    {
                        // Log our error and move to the next user.

                        Logging.LogError("YouTube Error: " + wex.Message + " for user: " + server.OwnerYouTubeChannelId);
                        continue;
                    }

                    // if our stream isnt null, and we have a return from yt.
                    if (streamResult != null && streamResult.items.Count > 0)
                    {
                        var stream = streamResult.items[0];

                        bool allowEveryone = server.AllowEveryone;
                        var chat = await DiscordHelper.GetMessageChannel(server.Id, server.OwnerLiveChannel);

                        if (chat == null)
                        {
                            continue;
                        }

                        bool checkChannelBroadcastStatus = channel == null || !channel.Servers.Contains(server.Id);
                        bool checkGoLive = !string.IsNullOrEmpty(server.OwnerLiveChannel.ToString()) && server.OwnerLiveChannel != 0;

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
                                            Name = server.OwnerYouTubeChannelId,
                                            Servers = new List<ulong>()
                                        };

                                        channel.Servers.Add(server.Id);

                                        liveChannels.Add(channel);
                                    }
                                    else
                                    {
                                        channel.Servers.Add(server.Id);
                                    }

                                    // Build our message
                                    var channelData = await youtubeManager.GetYouTubeChannelSnippetById(stream.snippet.channelId);

                                    string url = "http://" + (server.UseYouTubeGamingPublished ? "gaming" : "www") + ".youtube.com/watch?v=" + stream.id;
                                    string channelTitle = stream.snippet.channelTitle;
                                    string avatarUrl = channelData.items.Count > 0 ? channelData.items[0].snippet.thumbnails.high.url : "";
                                    string thumbnailUrl = stream.snippet.thumbnails.high.url;

                                    Logging.LogYouTubeGaming(channelTitle + " has gone online.");

                                    var message = await MessagingHelper.BuildMessage(channelTitle, "a game", stream.snippet.title, url, avatarUrl, thumbnailUrl,
                                        Constants.YouTubeGaming, server.OwnerYouTubeChannelId, server);

                                    var finalCheck = BotFiles.GetCurrentlyLiveYouTubeChannels().FirstOrDefault(x => x.Name == server.OwnerYouTubeChannelId);

                                    if (finalCheck == null || !finalCheck.Servers.Contains(server.Id))
                                    {
                                        if (channel.ChannelMessages == null)
                                            channel.ChannelMessages = new List<ChannelMessage>();

                                        channel.ChannelMessages.AddRange(await MessagingHelper.SendMessages(Constants.YouTubeGaming, new List<BroadcastMessage>() { message }));

                                        File.WriteAllText(Constants.ConfigRootDirectory + Constants.LiveDirectory + Constants.YouTubeDirectory + server.OwnerYouTubeChannelId + ".json", JsonConvert.SerializeObject(channel));
                                    }
                                }

                            }
                        }
                    }
                }
            }
        }

        public async Task CheckHitboxLive()
        {
            var servers = BotFiles.GetConfiguredServers();
            var liveChannels = BotFiles.GetCurrentlyLiveHitboxChannels();

            // Loop through servers to broadcast.
            foreach (var server in servers)
            {
                if (!server.AllowLive)
                {
                    continue;
                }

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
                            stream = await smashcastManager.GetChannelByName(hitboxChannel);
                        }
                        catch (Exception wex)
                        {
                            // Log our error and move to the next user.

                            Logging.LogError("Smashcast Error: " + wex.Message + " for user: " + hitboxChannel + " in Discord Server Id: " + server.Id);
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
                                            string url = "http://smashcast.tv/" + hitboxChannel;

                                            Logging.LogHitbox(hitboxChannel + " has gone online.");

                                            var message = await MessagingHelper.BuildMessage(
                                                hitboxChannel, gameName, stream.livestream[0].media_status, url, "http://edge.sf.hitbox.tv" +
                                                stream.livestream[0].channel.user_logo, "http://edge.sf.hitbox.tv" +
                                                stream.livestream[0].media_thumbnail_large, Constants.Smashcast, hitboxChannel, server);

                                            var finalCheck = BotFiles.GetCurrentlyLiveHitboxChannels().FirstOrDefault(x => x.Name == hitboxChannel);

                                            if (finalCheck == null || !finalCheck.Servers.Contains(server.Id))
                                            {
                                                if (channel.ChannelMessages == null)
                                                    channel.ChannelMessages = new List<ChannelMessage>();

                                                channel.ChannelMessages.AddRange(await MessagingHelper.SendMessages(Constants.Smashcast, new List<BroadcastMessage>() { message }));

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

        public async Task CheckOwnerHitboxLive()
        {
            var servers = BotFiles.GetConfiguredServers();
            var liveChannels = BotFiles.GetCurrentlyLiveHitboxChannels();

            // Loop through servers to broadcast.
            foreach (var server in servers)
            {
                if (!server.AllowLive)
                {
                    continue;
                }

                if (server.Id == 0 || server.OwnerLiveChannel == 0)
                { continue; }

                if (server.OwnerHitboxChannel != null)
                {
                    var channel = liveChannels.FirstOrDefault(x => x.Name.ToLower() == server.OwnerHitboxChannel.ToLower());

                    HitboxChannel stream = null;

                    try
                    {
                        stream = await smashcastManager.GetChannelByName(server.OwnerHitboxChannel);
                    }
                    catch (Exception wex)
                    {
                        // Log our error and move to the next user.

                        Logging.LogError("Smashcast Error: " + wex.Message + " for user: " + server.OwnerHitboxChannel + " in Discord Server Id: " + server.Id);
                        continue;
                    }

                    // if our stream isnt null, and we have a return from beam.
                    if (stream != null && stream.livestream != null && stream.livestream.Count > 0)
                    {
                        if (stream.livestream[0].media_is_live == "1")
                        {
                            bool allowEveryone = server.AllowEveryone;
                            var chat = await DiscordHelper.GetMessageChannel(server.Id, server.OwnerLiveChannel);

                            if (chat == null)
                            {
                                continue;
                            }

                            bool checkChannelBroadcastStatus = channel == null || !channel.Servers.Contains(server.Id);
                            bool checkGoLive = !string.IsNullOrEmpty(server.OwnerLiveChannel.ToString()) && server.OwnerLiveChannel != 0;

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
                                                Name = server.OwnerHitboxChannel,
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
                                        string url = "http://smashcast.tv/" + server.OwnerHitboxChannel;

                                        Logging.LogHitbox(server.OwnerHitboxChannel + " has gone online.");

                                        var message = await MessagingHelper.BuildMessage(
                                            server.OwnerHitboxChannel, gameName, stream.livestream[0].media_status, url, "http://edge.sf.hitbox.tv" +
                                            stream.livestream[0].channel.user_logo, "http://edge.sf.hitbox.tv" +
                                            stream.livestream[0].media_thumbnail_large, Constants.Smashcast, server.OwnerHitboxChannel, server);

                                        var finalCheck = BotFiles.GetCurrentlyLiveHitboxChannels().FirstOrDefault(x => x.Name == server.OwnerHitboxChannel);

                                        if (finalCheck == null || !finalCheck.Servers.Contains(server.Id))
                                        {
                                            if (channel.ChannelMessages == null)
                                                channel.ChannelMessages = new List<ChannelMessage>();

                                            channel.ChannelMessages.AddRange(await MessagingHelper.SendMessages(Constants.Smashcast, new List<BroadcastMessage>() { message }));

                                            File.WriteAllText(Constants.ConfigRootDirectory + Constants.LiveDirectory + Constants.HitboxDirectory + server.OwnerHitboxChannel + ".json", JsonConvert.SerializeObject(channel));
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
                if (!server.AllowPublished)
                {
                    continue;
                }

                // If server isnt set or published channel isnt set, skip it.
                if (server.Id == 0 || server.OwnerPublishedChannel == 0)
                {
                    continue;
                }

                // If they dont allow published, skip it.
                if (!server.AllowPublished)
                {
                    continue;
                }

                var chat = await DiscordHelper.GetMessageChannel(server.Id, server.OwnerPublishedChannel);

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
                            server.PublishedMessage = "%CHANNEL% just published a new video - %TITLE% - %URL%";
                        }

                        Color red = new Color(179, 18, 23);
                        author.IconUrl = client.CurrentUser.GetAvatarUrl() + "?_=" + Guid.NewGuid().ToString().Replace("-", "");
                        author.Name = "CouchBot";
                        author.Url = url;
                        footer.Text = "[" + Constants.YouTube + "] - " + DateTime.UtcNow.AddHours(server.TimeZoneOffset);
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

                                message += "**[" + Constants.YouTube + "]** " + server.PublishedMessage.Replace("%CHANNEL%", video.snippet.channelTitle).Replace("%TITLE%", video.snippet.title).Replace("%URL%", url);
                            }

                            Logging.LogYouTube(video.snippet.channelTitle + " has published a new video.");

                            await SendMessage(new BroadcastMessage()
                            {
                                GuildId = server.Id,
                                ChannelId = server.OwnerPublishedChannel,
                                UserId = user,
                                Message = message,
                                Platform = Constants.YouTube,
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

        public async Task CheckOwnerPublishedYouTube()
        {
            var servers = BotFiles.GetConfiguredServers();
            var users = BotFiles.GetConfiguredUsers();
            var now = DateTime.UtcNow;
            var then = now.AddMinutes(-15);

            foreach (var server in servers)
            {
                if (!server.AllowPublished)
                {
                    continue;
                }

                // If server isnt set or published channel isnt set, skip it.
                if (server.Id == 0 || server.OwnerPublishedChannel == 0)
                {
                    continue;
                }

                // If they dont allow published, skip it.
                if (!server.AllowPublished)
                {
                    continue;
                }

                var chat = await DiscordHelper.GetMessageChannel(server.Id, server.OwnerPublishedChannel);

                if (chat == null)
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(server.OwnerYouTubeChannelId))
                {
                    continue;
                }

                YouTubePlaylist playlist = null;

                try
                {
                    var details = await youtubeManager.GetContentDetailsByChannelId(server.OwnerYouTubeChannelId);

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
                    Logging.LogError("YouTube Published Error: " + ex.Message + " for user: " + server.OwnerYouTubeChannelId + " in Discord Server: " + server.Id);
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
                        server.PublishedMessage = "%CHANNEL% just published a new video - %TITLE% - %URL%";
                    }

                    Color red = new Color(179, 18, 23);
                    author.IconUrl = client.CurrentUser.GetAvatarUrl() + "?_=" + Guid.NewGuid().ToString().Replace("-", "");
                    author.Name = "CouchBot";
                    author.Url = url;
                    footer.Text = "[" + Constants.YouTube + "] - " + DateTime.UtcNow.AddHours(server.TimeZoneOffset);
                    footer.IconUrl = "http://couchbot.io/img/ytg.jpg";
                    embed.Author = author;
                    embed.Color = red;
                    embed.Description = server.PublishedMessage.Replace("%CHANNEL%", video.snippet.channelTitle).Replace("%GAME%", "a game").Replace("%TITLE%", video.snippet.title).Replace("%URL%", url);
                    embed.Title = video.snippet.channelTitle + " published a new video!";
                    embed.ThumbnailUrl = channelData.items.Count > 0 ? channelData.items[0].snippet.thumbnails.high.url + "?_=" + Guid.NewGuid().ToString().Replace("-", "") : "";
                    embed.ImageUrl = server.AllowThumbnails ? video.snippet.thumbnails.high.url + "?_=" + Guid.NewGuid().ToString().Replace("-", "") : "";
                    embed.Footer = footer;

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

                        message += "**[" + Constants.YouTube + "]** " + server.PublishedMessage.Replace("%CHANNEL%", video.snippet.channelTitle).Replace("%TITLE%", video.snippet.title).Replace("%URL%", url);
                    }

                    Logging.LogYouTube(video.snippet.channelTitle + " has published a new video.");

                    await SendMessage(new BroadcastMessage()
                    {
                        GuildId = server.Id,
                        ChannelId = server.OwnerPublishedChannel,
                        UserId = server.OwnerYouTubeChannelId,
                        Message = message,
                        Platform = Constants.YouTube,
                        Embed = (!server.UseTextAnnouncements ? embed.Build() : null)
                    });
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
                        await CleanUpLiveStreams(Constants.YouTubeGaming);
                        await CleanUpLiveStreams(Constants.Twitch);
                        await CleanUpLiveStreams(Constants.Smashcast);
                        Logging.LogInfo("Cleaning Up Live Files Complete.");
                    }
                }
            }, null, 0, 300000);
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
            }, null, 0, 60000);
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

                    if (string.IsNullOrEmpty(guild.GoodbyeMessage))
                    {
                        guild.GoodbyeMessage = "Good bye, " + arg.Username + ", thanks for hanging out!";
                    }

                    guild.GoodbyeMessage = guild.GoodbyeMessage.Replace("%USER%", arg.Username).Replace("%NEWLINE%", "\r\n");

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
            if (platform == Constants.Twitch)
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

            if (platform == Constants.YouTubeGaming)
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

            if (platform == Constants.Smashcast)
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
                        var liveStream = await smashcastManager.GetChannelByName(stream.Name);

                        if (liveStream == null || liveStream.livestream == null || liveStream.livestream.Count < 1 || liveStream.livestream[0].media_is_live == "0")
                        {
                            //await CleanupMessages(stream.ChannelMessages);

                            File.Delete(Constants.ConfigRootDirectory + Constants.LiveDirectory + Constants.HitboxDirectory + stream.Name + ".json");
                        }
                    }
                    catch (Exception wex)
                    {

                        Logging.LogError("Clean Up Smashcast Error: " + wex.Message + " for user: " + stream.Name);
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
                    if (!channelMessage.DeleteOffline)
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

                        if (msg != null || msg.Id != 0)
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

                    if (message.Platform.Equals(Constants.YouTube))
                    {
                        statisticsManager.AddToYouTubeAlertCount();
                    }

                    if (message.Platform.Equals(Constants.Twitch))
                    {
                        statisticsManager.AddToTwitchAlertCount();
                    }

                    if (message.Platform.Equals(Constants.Beam))
                    {
                        statisticsManager.AddToBeamAlertCount();
                    }

                    if (message.Platform.Equals(Constants.Smashcast))
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
    }
}