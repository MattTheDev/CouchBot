using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MTD.CouchBot.Bot;
using MTD.CouchBot.Domain;
using MTD.CouchBot.Domain.Models.Bot;
using MTD.CouchBot.Domain.Models.Picarto;
using MTD.CouchBot.Domain.Models.Smashcast;
using MTD.CouchBot.Domain.Models.Twitch;
using MTD.CouchBot.Domain.Models.VidMe;
using MTD.CouchBot.Domain.Models.YouTube;
using MTD.CouchBot.Domain.Utilities;
using MTD.CouchBot.Managers;
using MTD.CouchBot.Managers.Implementations;
using MTD.CouchBot.Models.Bot;
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
        public static DiscordShardedClient client;
        public static BeamClient beamClient;

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
        private static Timer twitchOwnerFeedTimer;
        private static Timer twitchTeamTimer;
        private static Timer twitchGameTimer;
        private static Timer picartoTimer;
        private static Timer picartoOwnerTimer;
        private static Timer vidMeTimer;
        private static Timer vidMeOwnerTimer;
        private static Timer guildCheckTimer;

        private static Timer cleanupTimer;
        private static Timer uptimeTimer;
        private bool initialServicesRan = false;

        IStatisticsManager statisticsManager;
        IYouTubeManager youtubeManager;
        ITwitchManager twitchManager;
        ISmashcastManager smashcastManager;
        IMixerManager mixerManager;
        IPicartoManager picartoManager;
        IVidMeManager vidMeManager;

        #endregion

        static void Main(string[] args) => new Program().Start().GetAwaiter().GetResult();

        public async Task Start()
        {
            Logging.LogInfo("Starting the Bot!");
            Logging.LogInfo("Check Bot Configuration.");

            if ((Constants.DiscordToken == "" || Constants.DiscordToken == "DISCORDTOKEN")
                || (Constants.YouTubeApiKey == "" || Constants.YouTubeApiKey == "YOUTUBEAPI")
                || (Constants.TwitchClientId == "" || Constants.TwitchClientId == "TWITCHID")
                || (string.IsNullOrEmpty(Constants.Prefix)))
            {
                Console.WriteLine("\r\nYou need to configure the following items:");
                Console.WriteLine("- Bot Prefix\r\n- Discord Token\r\n- Twitch Client ID\r\n- YouTube API Key");
                Console.WriteLine("\r\nIf you're using the launcher, stop the bot, and set the settings via the UI. If you are not using the launcher, you can edit the settings JSON file and relaunch the bot.");
                Console.WriteLine("\r\nYour settings can be found at: ");
                Console.WriteLine(Assembly.GetEntryAssembly().Location.Replace(Path.GetFileName(Assembly.GetEntryAssembly().Location), "") + Constants.BotSettings);
                Console.ReadLine();
                return;
            }

            Logging.LogInfo("Bot Configuration - All Set.");
            Logging.LogInfo("Check Folder Structure.");

            BotFiles.CheckFolderStructure();

            Logging.LogInfo("Folder Structure - All Set.");

            Logging.LogInfo("Initializing Managers.");

            statisticsManager = new StatisticsManager();
            youtubeManager = new YouTubeManager();
            twitchManager = new TwitchManager();
            mixerManager = new MixerManager();
            smashcastManager = new SmashcastManager();
            picartoManager = new PicartoManager();
            vidMeManager = new VidMeManager();

            Logging.LogInfo("Managers Initialized.");
            Logging.LogInfo("Log Last Restart Time and Date.");

            statisticsManager.LogRestartTime();

            Logging.LogInfo("Clear randomly seeded Mixer task Ids.");

            statisticsManager.ClearRandomInts();

            Logging.LogInfo("Do Bot Things.");

            await DoBotStuff();

            Logging.LogInfo("Bot Things Done.");
            Logging.LogInfo("Resubscribe to Mixer Events.");

            if (Constants.EnableMixer)
            {
                await ResubscribeToBeamEvents();
                QueueBeamClientCheck();
            }

            Logging.LogInfo("Subscribed to Mixer Events - All Set.");
            Logging.LogInfo("Queue Timer Jobs.");

            if (Constants.EnableTwitch)
            {
                QueueTwitchChecks();
            }

            if (Constants.EnableYouTube)
            {
                QueueYouTubeChecks();
            }

            if (Constants.EnableSmashcast)
            {
                QueueHitboxChecks();
            }

            if (Constants.EnablePicarto)
            {
                QueuePicartoChecks();
            }

            if(Constants.EnableVidMe)
            {
                QueueVidMeChecks();
            }

            QueueCleanUp();
            QueueUptimeCheckIn();
            QueueHealthChecks();

            Logging.LogInfo("Timer Jobs Queued - All Set.");

            await Task.Delay(-1);
        }

        public async Task DoBotStuff()
        {
            client = new DiscordShardedClient(new DiscordSocketConfig()
            {
                TotalShards = Constants.TotalShards
            });

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

            Logging.LogMixer("Getting Server Files.");

            var servers = BotFiles.GetConfiguredServers().Where(x => x.ServerBeamChannelIds != null && x.ServerBeamChannelIds.Count > 0);

            await Task.Run(async () =>
            {
                Logging.LogMixer("Connecting to Mixer Constellation.");

                await beamClient.RunWebSockets();

                Logging.LogMixer("Connected to Mixer Constellation.");
            });


            Logging.LogMixer("Initiating Subscription Loop.");
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

                    if (!string.IsNullOrEmpty(s.OwnerBeamChannelId))
                    {
                        await beamClient.SubscribeToLiveAnnouncements(s.OwnerBeamChannelId);
                        count++;
                    }
                }
            }

            sw.Stop();
            Logging.LogMixer("Subscription Loop Complete. Processed " + count + " channels in " + sw.ElapsedMilliseconds + " milliseconds.");
        }

        public void QueueBeamClientCheck()
        {
            beamClientTimer = new Timer(async (e) =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                Logging.LogMixer("Mixer Constellation Health Check Started.");

                if (beamClient.Status() != WebSocketState.Open)
                {
                    await ResubscribeToBeamEvents();
                }
                else
                {
                    Logging.LogMixer("" + beamClient.Status());
                }

                sw.Stop();
                Logging.LogMixer("Mixer Constellation Health Check Complete - Elapsed Runtime: " + sw.ElapsedMilliseconds + " milliseconds.");
            }, null, 0, 60000);
        }

        public void QueueHitboxChecks()
        {
            hitboxTimer = new Timer(async (e) =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                Logging.LogSmashcast("Checking Smashcast Channels.");
                await CheckHitboxLive();
                sw.Stop();
                Logging.LogSmashcast("Smashcast Check Complete - Elapsed Runtime: " + sw.ElapsedMilliseconds + " milliseconds.");
            }, null, 0, Constants.SmashcastInterval);

            hitboxOwnerTimer = new Timer(async (e) =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                Logging.LogSmashcast("Checking Owner Smashcast Channels.");
                await CheckOwnerHitboxLive();
                sw.Stop();
                Logging.LogSmashcast("Owner Smashcast Check Complete - Elapsed Runtime: " + sw.ElapsedMilliseconds + " milliseconds.");
            }, null, 0, Constants.SmashcastInterval);
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
            }, null, 0, Constants.TwitchInterval);

            twitchOwnerTimer = new Timer(async (e) =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                Logging.LogTwitch("Checking Owner Twitch Channels.");
                await CheckOwnerTwitchLive();
                sw.Stop();
                Logging.LogTwitch("Owner Twitch Check Complete - Elapsed Runtime: " + sw.ElapsedMilliseconds + " milliseconds.");
            }, null, 0, Constants.TwitchInterval);

            twitchFeedTimer = new Timer(async (e) =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                Logging.LogTwitch("Checking Twitch Channel Feeds.");
                await CheckTwitchChannelFeeds();
                sw.Stop();
                Logging.LogTwitch("Twitch Channel Feed Check Complete - Elapsed Runtime: " + sw.ElapsedMilliseconds + " milliseconds.");
            }, null, 0, Constants.TwitchFeedInterval);

            twitchOwnerFeedTimer = new Timer(async (e) =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                Logging.LogTwitch("Checking Owner Twitch Channel Feeds.");
                await CheckTwitchOwnerChannelFeeds();
                sw.Stop();
                Logging.LogTwitch("Owner Twitch Channel Feed Check Complete - Elapsed Runtime: " + sw.ElapsedMilliseconds + " milliseconds.");
            }, null, 0, Constants.TwitchFeedInterval);

            twitchTeamTimer = new Timer(async (e) =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                Logging.LogTwitch("Checking Twitch Teams.");
                await CheckTwitchTeams();
                sw.Stop();
                Logging.LogTwitch("Checking Twitch Teams Check Complete - Elapsed Runtime: " + sw.ElapsedMilliseconds + " milliseconds.");
            }, null, 0, Constants.TwitchInterval);

            twitchGameTimer = new Timer(async (e) =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                Logging.LogTwitch("Checking Twitch Games.");
                await CheckTwitchGames();
                sw.Stop();
                Logging.LogTwitch("Checking Twitch Games Check Complete - Elapsed Runtime: " + sw.ElapsedMilliseconds + " milliseconds.");
            }, null, 0, Constants.TwitchInterval);
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
            }, null, 0, Constants.YouTubeLiveInterval);

            youtubeOwnerTimer = new Timer(async (e) =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                Logging.LogYouTubeGaming("Checking Owner YouTube Gaming Channels.");
                await CheckOwnerYouTubeLive();
                sw.Stop();
                Logging.LogYouTubeGaming("Owner YouTube Gaming Check Complete - Elapsed Runtime: " + sw.ElapsedMilliseconds + " milliseconds.");
            }, null, 0, Constants.YouTubeLiveInterval);

            youtubePublishedTimer = new Timer(async (e) =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                Logging.LogYouTube("Checking YouTube Published");
                await CheckPublishedYouTube();
                sw.Stop();
                Logging.LogYouTube("YouTube Published Complete - Elapsed Runtime: " + sw.ElapsedMilliseconds + " milliseconds.");
            }, null, 0, Constants.YouTubePublishedInterval);

            youtubePublishedOwnerTimer = new Timer(async (e) =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                Logging.LogYouTube("Checking Owner YouTube Published");
                await CheckOwnerPublishedYouTube();
                sw.Stop();
                Logging.LogYouTube("Owner YouTube Published Complete - Elapsed Runtime: " + sw.ElapsedMilliseconds + " milliseconds.");
            }, null, 0, Constants.YouTubePublishedInterval);
        }

        public void QueueVidMeChecks()
        {
            vidMeTimer = new Timer(async (e) =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                Logging.LogVidMe("Checking VidMe");
                await CheckVidMe();
                sw.Stop();
                Logging.LogVidMe("VidMe Complete - Elapsed Runtime: " + sw.ElapsedMilliseconds + " milliseconds.");
            }, null, 0, Constants.VidMeInterval);

            vidMeOwnerTimer = new Timer(async (e) =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                Logging.LogVidMe("Checking Owner VidMe Published");
                await CheckOwnerVidMe();
                sw.Stop();
                Logging.LogVidMe("Owner VidMe Complete - Elapsed Runtime: " + sw.ElapsedMilliseconds + " milliseconds.");
            }, null, 0, Constants.VidMeInterval);
        }

        public void QueuePicartoChecks()
        {
            picartoTimer = new Timer(async (e) =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                Logging.LogPicarto("Checking Picarto Channels.");
                await CheckPicartoLive();
                sw.Stop();
                Logging.LogPicarto("Picarto Check Complete - Elapsed Runtime: " + sw.ElapsedMilliseconds + " milliseconds.");
            }, null, 0, Constants.PicartoInterval);

            picartoOwnerTimer = new Timer(async (e) =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                Logging.LogPicarto("Checking Picarto Smashcast Channels.");
                await CheckOwnerPicartoLive();
                sw.Stop();
                Logging.LogPicarto("Owner Picarto Check Complete - Elapsed Runtime: " + sw.ElapsedMilliseconds + " milliseconds.");
            }, null, 0, Constants.PicartoInterval);
        }

        public void QueueHealthChecks()
        {
            guildCheckTimer = new Timer((e) =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                Logging.LogInfo("Checking Guild Configurations.");
                CheckGuildConfigurations();
                sw.Stop();
                Logging.LogInfo("Guild Configuration Check Complete - Elapsed Runtime: " + sw.ElapsedMilliseconds + " milliseconds.");
            }, null, 0, 600000);
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
                            await chat.SendMessageAsync("**[" + Program.client.CurrentUser.Username + "]** " + message);
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

            var result = await commands.ExecuteAsync(context, argPos);
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
                                    string channelName = StringUtilities.ScrubChatMessage(stream.channel.display_name);
                                    string avatarUrl = stream.channel.logo != null ? stream.channel.logo : "https://static-cdn.jtvnw.net/jtv_user_pictures/xarth/404_user_70x70.png";
                                    string thumbnailUrl = stream.preview.large;

                                    var message = await MessagingHelper.BuildMessage(channelName, stream.game, stream.channel.status, url, avatarUrl,
                                        thumbnailUrl, Constants.Twitch, stream.channel._id.ToString(), server, server.GoLiveChannel, null);

                                    var finalCheck = BotFiles.GetCurrentlyLiveTwitchChannels().FirstOrDefault(x => x.Name == stream.channel._id.ToString());

                                    if (finalCheck == null || !finalCheck.Servers.Contains(server.Id))
                                    {
                                        if (channel.ChannelMessages == null)
                                            channel.ChannelMessages = new List<ChannelMessage>();

                                        channel.ChannelMessages.AddRange(await MessagingHelper.SendMessages(Constants.Twitch, new List<BroadcastMessage>() { message }));

                                        File.WriteAllText(Constants.ConfigRootDirectory + Constants.LiveDirectory + Constants.TwitchDirectory + stream.channel._id.ToString() + ".json",
                                            JsonConvert.SerializeObject(channel));

                                        Logging.LogTwitch(channelName + " has gone online.");
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
                                string channelName = StringUtilities.ScrubChatMessage(stream.channel.display_name);
                                string avatarUrl = stream.channel.logo != null ? stream.channel.logo : "https://static-cdn.jtvnw.net/jtv_user_pictures/xarth/404_user_70x70.png";
                                string thumbnailUrl = stream.preview.large;

                                var message = await MessagingHelper.BuildMessage(channelName, stream.game, stream.channel.status, url, avatarUrl,
                                    thumbnailUrl, Constants.Twitch, stream.channel._id.ToString(), server, server.OwnerLiveChannel, null);

                                var finalCheck = BotFiles.GetCurrentlyLiveTwitchChannels().FirstOrDefault(x => x.Name == stream.channel._id.ToString());

                                if (finalCheck == null || !finalCheck.Servers.Contains(server.Id))
                                {
                                    if (channel.ChannelMessages == null)
                                        channel.ChannelMessages = new List<ChannelMessage>();

                                    channel.ChannelMessages.AddRange(await MessagingHelper.SendMessages(Constants.Twitch, new List<BroadcastMessage>() { message }));

                                    File.WriteAllText(Constants.ConfigRootDirectory + Constants.LiveDirectory + Constants.TwitchDirectory + stream.channel._id.ToString() + ".json",
                                        JsonConvert.SerializeObject(channel));

                                    Logging.LogTwitch(channelName + " has gone online.");
                                }
                            }
                        }
                    }
                }
            }
        }

        public async Task CheckTwitchChannelFeeds()
        {
            // Nothing.
        }

        public async Task CheckTwitchOwnerChannelFeeds()
        {
            var servers = BotFiles.GetConfiguredServers();

            // Loop through servers to broadcast.
            foreach (var server in servers)
            {
                if (!server.AllowOwnerChannelFeed)
                {
                    continue;
                }

                if (server.Id != 0 && server.OwnerTwitchFeedChannel != 0 &&
                    !string.IsNullOrEmpty(server.OwnerTwitchChannel) && !string.IsNullOrEmpty(server.OwnerTwitchChannelId))
                {
                    TwitchChannelFeed feed = null;

                    try
                    {
                        feed = await twitchManager.GetChannelFeedPosts(server.OwnerTwitchChannelId);
                    }
                    catch (Exception wex)
                    {
                        Logging.LogError("Twitch Server Error: " + wex.Message + " in Discord Server Id: " + server.Id);
                        continue;
                    }

                    if (feed == null || feed.posts == null || feed.posts.Count == 0)
                    {
                        continue;
                    }

                    var chat = await DiscordHelper.GetMessageChannel(server.Id, server.OwnerTwitchFeedChannel);

                    if (chat == null)
                    {
                        continue;
                    }

                    foreach (var post in feed.posts)
                    {
                        DateTime now = DateTime.UtcNow;
                        DateTime created = DateTime.Parse(post.created_at).ToUniversalTime();
                        TimeSpan diff = now - created;

                        if (diff.TotalMinutes <= 2)
                        {
                            string message = "**[New Channel Feed Update - " + created.AddHours(server.TimeZoneOffset).ToString("MM/dd/yyyy hh:mm tt") + "]**\r\n" +
                                post.body + "\r\n\r\n" +
                                "<https://twitch.tv/" + server.OwnerTwitchChannel + "/p/" + post.id + ">";

                            try
                            {
                                await chat.SendMessageAsync(message);
                            }
                            catch (Exception wex)
                            {
                                Logging.LogError("Twitch Channel Feed Error: " + wex.Message + " for user: " + server.OwnerTwitchChannel + " in server: " + server.Id);
                            }

                            Logging.LogTwitch(server.OwnerTwitchChannel + " posted a new channel feed message.");
                        }
                    }
                }
            }
        }

        public async Task CheckTwitchTeams()
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
                    server.TwitchTeams != null && server.TwitchTeams.Count > 0)
                {

                    if (server.TwitchTeams == null)
                    {
                        continue;
                    }

                    foreach (var team in server.TwitchTeams)
                    {
                        var userList = await twitchManager.GetDelimitedListOfTwitchMemberIds(team);
                        var teamResponse = await twitchManager.GetTwitchTeamByName(team);

                        TwitchStreamsV5 streams = null;

                        try
                        {
                            // Query Twitch for our stream.
                            streams = await twitchManager.GetStreamsByIdList(userList);
                        }
                        catch (Exception wex)
                        {
                            // Log our error and move to the next user.

                            Logging.LogError("Twitch Team Server Error: " + wex.Message + " in Discord Server Id: " + server.Id);
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
                                        string channelName = StringUtilities.ScrubChatMessage(stream.channel.display_name);
                                        string avatarUrl = stream.channel.logo != null ? stream.channel.logo : "https://static-cdn.jtvnw.net/jtv_user_pictures/xarth/404_user_70x70.png";
                                        string thumbnailUrl = stream.preview.large;

                                        var message = await MessagingHelper.BuildMessage(channelName, stream.game, stream.channel.status, url, avatarUrl,
                                            thumbnailUrl, Constants.Twitch, stream.channel._id.ToString(), server, server.GoLiveChannel, teamResponse.DisplayName);

                                        var finalCheck = BotFiles.GetCurrentlyLiveTwitchChannels().FirstOrDefault(x => x.Name == stream.channel._id.ToString());

                                        if (finalCheck == null || !finalCheck.Servers.Contains(server.Id))
                                        {
                                            if (channel.ChannelMessages == null)
                                                channel.ChannelMessages = new List<ChannelMessage>();

                                            channel.ChannelMessages.AddRange(await MessagingHelper.SendMessages(Constants.Twitch, new List<BroadcastMessage>() { message }));

                                            File.WriteAllText(Constants.ConfigRootDirectory + Constants.LiveDirectory + Constants.TwitchDirectory + stream.channel._id.ToString() + ".json",
                                                JsonConvert.SerializeObject(channel));

                                            Logging.LogTwitch(teamResponse.Name + " team member " + channelName + " has gone online.");
                                        }
                                    }
                                }
                            }
                        }

                    }
                }
            }
        }

        public async Task CheckTwitchGames()
        {
            var servers = BotFiles.GetConfiguredServersWithLiveChannel();
            var liveChannels = BotFiles.GetCurrentlyLiveTwitchChannels();
            var gameList = new List<TwitchGameServerModel>();

            foreach (var s in servers)
            {
                if (s.ServerGameList == null)
                {
                    continue;
                }

                foreach (var g in s.ServerGameList)
                {
                    var gameServerModel = gameList.FirstOrDefault(x => x.Name.Equals(g, StringComparison.CurrentCultureIgnoreCase));

                    if (gameServerModel == null)
                    {
                        gameList.Add(new TwitchGameServerModel() { Name = g, Servers = new List<ulong>() { s.Id } });
                    }
                    else
                    {
                        gameServerModel.Servers.Add(s.Id);
                    }
                }
            }

            foreach (var game in gameList)
            {
                List<TwitchStreamsV5.Stream> gameResponse = null;

                try
                {
                    // Query Twitch for our stream.
                    gameResponse = await twitchManager.GetStreamsByGameName(game.Name); 
                }
                catch (Exception wex)
                {
                    // Log our error and move to the next user.

                    Logging.LogError("Twitch Game Error: " + wex.Message);
                    continue;
                }

                if(gameResponse == null || gameResponse.Count == 0)
                {
                    continue;
                }

                int count = 0;

                foreach (var stream in gameResponse)
                {
                    if (count >= 5)
                    {
                        continue;
                    }

                    DateTime now = DateTime.UtcNow;
                    DateTime created = DateTime.Parse(stream.created_at).ToUniversalTime();
                    TimeSpan diff = now - created;
                    var interval = ((Constants.TwitchInterval / 1000) / 60);

                    if (diff.TotalMinutes > interval)
                    {
                        continue;
                    }

                    foreach (var s in game.Servers)
                    {
                        var server = BotFiles.GetConfiguredServerById(s);

                        var channel = liveChannels.FirstOrDefault(x => x.Name == stream.channel._id.ToString());

                        var chat = await DiscordHelper.GetMessageChannel(server.Id, server.GoLiveChannel);

                        if (chat == null)
                        {
                            continue;
                        }

                        bool checkChannelBroadcastStatus = channel == null || !channel.Servers.Contains(server.Id);

                        if (checkChannelBroadcastStatus)
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
                            string channelName = StringUtilities.ScrubChatMessage(stream.channel.display_name);
                            string avatarUrl = stream.channel.logo != null ? stream.channel.logo : "https://static-cdn.jtvnw.net/jtv_user_pictures/xarth/404_user_70x70.png";
                            string thumbnailUrl = stream.preview.large;

                            var message = await MessagingHelper.BuildMessage(channelName, stream.game, stream.channel.status, url, avatarUrl,
                                thumbnailUrl, Constants.Twitch, stream.channel._id.ToString(), server, server.GoLiveChannel, null);

                            var finalCheck = BotFiles.GetCurrentlyLiveTwitchChannels().FirstOrDefault(x => x.Name == stream.channel._id.ToString());

                            if (finalCheck == null || !finalCheck.Servers.Contains(server.Id))
                            {
                                if (channel.ChannelMessages == null)
                                    channel.ChannelMessages = new List<ChannelMessage>();

                                channel.ChannelMessages.AddRange(await MessagingHelper.SendMessages(Constants.Twitch, new List<BroadcastMessage>() { message }));

                                File.WriteAllText(Constants.ConfigRootDirectory + Constants.LiveDirectory + Constants.TwitchDirectory + stream.channel._id.ToString() + ".json",
                                    JsonConvert.SerializeObject(channel));

                                Logging.LogTwitch(channelName + " has gone live playing " + game.Name);
                            }
                        }
                    }

                    count++;
                }
            }
        }

        public async Task CheckYouTubeLive()
        {
            var servers = BotFiles.GetConfiguredServersWithLiveChannel();
            var liveChannels = BotFiles.GetCurrentlyLiveYouTubeChannels();
            var youTubeChannelList = new List<YouTubeChannelServerModel>();

            foreach (var server in servers)
            {
                if (!server.AllowLive)
                {
                    continue;
                }

                if (server.ServerYouTubeChannelIds == null)
                {
                    continue;
                }

                foreach (var c in server.ServerYouTubeChannelIds)
                {
                    var channelServerModel = youTubeChannelList.FirstOrDefault(x => x.YouTubeChannelId.Equals(c, StringComparison.CurrentCultureIgnoreCase));

                    if (channelServerModel == null)
                    {
                        youTubeChannelList.Add(new YouTubeChannelServerModel { YouTubeChannelId = c, Servers = new List<ulong> { server.Id } });
                    }
                    else
                    {
                        channelServerModel.Servers.Add(server.Id);
                    }
                }
            }

            foreach (var c in youTubeChannelList)
            {
                YouTubeSearchListChannel streamResult = null;

                try
                {
                    // Query Youtube for our stream.
                    streamResult = await youtubeManager.GetLiveVideoByChannelId(c.YouTubeChannelId);
                }
                catch (Exception wex)
                {
                    // Log our error and move to the next user.

                    Logging.LogError("YouTube Error: " + wex.Message + " for user: " + c.YouTubeChannelId);
                    continue;
                }

                if (streamResult != null && streamResult.items.Count > 0)
                {
                    var stream = streamResult.items[0];

                    foreach (var s in c.Servers)
                    {
                        var server = BotFiles.GetConfiguredServerById(s);
                        var channel = liveChannels.FirstOrDefault(x => x.Name.ToLower() == c.YouTubeChannelId.ToLower());
                        bool allowEveryone = server.AllowEveryone;
                        var chat = await DiscordHelper.GetMessageChannel(server.Id, server.GoLiveChannel);

                        if (chat == null)
                        {
                            continue;
                        }

                        if (channel == null)
                        {
                            channel = new LiveChannel()
                            {
                                Name = c.YouTubeChannelId,
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
                        YouTubeChannelSnippet channelData = null;

                        try
                        {
                            channelData = await youtubeManager.GetYouTubeChannelSnippetById(stream.snippet.channelId);
                        }
                        catch (Exception wex)
                        {
                            // Log our error and move to the next user.

                            Logging.LogError("YouTube Error: " + wex.Message + " for user: " + c.YouTubeChannelId);
                            continue;
                        }

                        if (channelData == null)
                        {
                            continue;
                        }

                        string url = "http://" + (server.UseYouTubeGamingPublished ? "gaming" : "www") + ".youtube.com/watch?v=" + stream.id;
                        string channelTitle = stream.snippet.channelTitle;
                        string avatarUrl = channelData.items.Count > 0 ? channelData.items[0].snippet.thumbnails.high.url : "";
                        string thumbnailUrl = stream.snippet.thumbnails.high.url;

                        var message = await MessagingHelper.BuildMessage(channelTitle, "a game", stream.snippet.title, url, avatarUrl, thumbnailUrl,
                            Constants.YouTubeGaming, c.YouTubeChannelId, server, server.GoLiveChannel, null);

                        var finalCheck = BotFiles.GetCurrentlyLiveYouTubeChannels().FirstOrDefault(x => x.Name == c.YouTubeChannelId);

                        if (finalCheck == null || !finalCheck.Servers.Contains(server.Id))
                        {
                            if (channel.ChannelMessages == null)
                                channel.ChannelMessages = new List<ChannelMessage>();

                            channel.ChannelMessages.AddRange(await MessagingHelper.SendMessages(Constants.YouTubeGaming, new List<BroadcastMessage>() { message }));
                            Logging.LogYouTubeGaming(channelTitle + " has gone online.");
                            File.WriteAllText(Constants.ConfigRootDirectory + Constants.LiveDirectory + Constants.YouTubeDirectory + c.YouTubeChannelId + ".json", JsonConvert.SerializeObject(channel));

                            Logging.LogYouTubeGaming(channelTitle + " has gone online.");
                        }
                    }
                }
            }
        }

        public async Task CheckOwnerYouTubeLive()
        {
            var servers = BotFiles.GetConfiguredServersWithOwnerLiveChannel();
            var liveChannels = BotFiles.GetCurrentlyLiveYouTubeChannels();
            var youTubeChannelList = new List<YouTubeChannelServerModel>();

            foreach (var server in servers)
            {
                if (!server.AllowLive)
                {
                    continue;
                }

                if (server.ServerYouTubeChannelIds == null)
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(server.OwnerYouTubeChannelId))
                {
                    var channelServerModel = youTubeChannelList.FirstOrDefault(x => x.YouTubeChannelId.Equals(server.OwnerYouTubeChannelId, StringComparison.CurrentCultureIgnoreCase));

                    if (channelServerModel == null)
                    {
                        youTubeChannelList.Add(new YouTubeChannelServerModel { YouTubeChannelId = server.OwnerYouTubeChannelId, Servers = new List<ulong> { server.Id } });
                    }
                    else
                    {
                        channelServerModel.Servers.Add(server.Id);
                    }
                }
            }

            foreach (var c in youTubeChannelList)
            {
                YouTubeSearchListChannel streamResult = null;

                try
                {
                    // Query Youtube for our stream.
                    streamResult = await youtubeManager.GetLiveVideoByChannelId(c.YouTubeChannelId);
                }
                catch (Exception wex)
                {
                    // Log our error and move to the next user.

                    Logging.LogError("YouTube Error: " + wex.Message + " for user: " + c.YouTubeChannelId);
                    continue;
                }

                if (streamResult != null && streamResult.items.Count > 0)
                {
                    var stream = streamResult.items[0];

                    foreach (var s in c.Servers)
                    {
                        var server = BotFiles.GetConfiguredServerById(s);
                        var channel = liveChannels.FirstOrDefault(x => x.Name.ToLower() == c.YouTubeChannelId.ToLower());
                        bool allowEveryone = server.AllowEveryone;
                        var chat = await DiscordHelper.GetMessageChannel(server.Id, server.OwnerLiveChannel);

                        if (chat == null)
                        {
                            continue;
                        }

                        if (channel == null)
                        {
                            channel = new LiveChannel()
                            {
                                Name = c.YouTubeChannelId,
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
                        YouTubeChannelSnippet channelData = null;

                        try
                        {
                            channelData = await youtubeManager.GetYouTubeChannelSnippetById(stream.snippet.channelId);
                        }
                        catch (Exception wex)
                        {
                            // Log our error and move to the next user.

                            Logging.LogError("YouTube Error: " + wex.Message + " for user: " + c.YouTubeChannelId);
                            continue;
                        }

                        if (channelData == null)
                        {
                            continue;
                        }

                        string url = "http://" + (server.UseYouTubeGamingPublished ? "gaming" : "www") + ".youtube.com/watch?v=" + stream.id;
                        string channelTitle = stream.snippet.channelTitle;
                        string avatarUrl = channelData.items.Count > 0 ? channelData.items[0].snippet.thumbnails.high.url : "";
                        string thumbnailUrl = stream.snippet.thumbnails.high.url;

                        var message = await MessagingHelper.BuildMessage(channelTitle, "a game", stream.snippet.title, url, avatarUrl, thumbnailUrl,
                            Constants.YouTubeGaming, c.YouTubeChannelId, server, server.OwnerLiveChannel, null);

                        var finalCheck = BotFiles.GetCurrentlyLiveYouTubeChannels().FirstOrDefault(x => x.Name == c.YouTubeChannelId);

                        if (finalCheck == null || !finalCheck.Servers.Contains(server.Id))
                        {
                            if (channel.ChannelMessages == null)
                                channel.ChannelMessages = new List<ChannelMessage>();

                            channel.ChannelMessages.AddRange(await MessagingHelper.SendMessages(Constants.YouTubeGaming, new List<BroadcastMessage>() { message }));
                            Logging.LogYouTubeGaming(channelTitle + " has gone online.");
                            File.WriteAllText(Constants.ConfigRootDirectory + Constants.LiveDirectory + Constants.YouTubeDirectory + c.YouTubeChannelId + ".json", JsonConvert.SerializeObject(channel));

                            Logging.LogYouTubeGaming(channelTitle + " has gone online.");
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

                        SmashcastChannel stream = null;

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

                        // if our stream isnt null, and we have a return from mixer.
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

                                            var message = await MessagingHelper.BuildMessage(
                                                hitboxChannel, gameName, stream.livestream[0].media_status, url, "http://edge.sf.hitbox.tv" +
                                                stream.livestream[0].channel.user_logo, "http://edge.sf.hitbox.tv" +
                                                stream.livestream[0].media_thumbnail_large, Constants.Smashcast, hitboxChannel, server, server.GoLiveChannel, null);

                                            var finalCheck = BotFiles.GetCurrentlyLiveHitboxChannels().FirstOrDefault(x => x.Name == hitboxChannel);

                                            if (finalCheck == null || !finalCheck.Servers.Contains(server.Id))
                                            {
                                                if (channel.ChannelMessages == null)
                                                    channel.ChannelMessages = new List<ChannelMessage>();

                                                channel.ChannelMessages.AddRange(await MessagingHelper.SendMessages(Constants.Smashcast, new List<BroadcastMessage>() { message }));

                                                File.WriteAllText(Constants.ConfigRootDirectory + Constants.LiveDirectory + Constants.SmashcastDirectory + hitboxChannel + ".json", JsonConvert.SerializeObject(channel));

                                                Logging.LogSmashcast(hitboxChannel + " has gone online.");
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

                    SmashcastChannel stream = null;

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

                    // if our stream isnt null, and we have a return from mixer.
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

                                        var message = await MessagingHelper.BuildMessage(
                                            server.OwnerHitboxChannel, gameName, stream.livestream[0].media_status, url, "http://edge.sf.hitbox.tv" +
                                            stream.livestream[0].channel.user_logo, "http://edge.sf.hitbox.tv" +
                                            stream.livestream[0].media_thumbnail_large, Constants.Smashcast, server.OwnerHitboxChannel, server, server.OwnerLiveChannel, null);

                                        var finalCheck = BotFiles.GetCurrentlyLiveHitboxChannels().FirstOrDefault(x => x.Name == server.OwnerHitboxChannel);

                                        if (finalCheck == null || !finalCheck.Servers.Contains(server.Id))
                                        {
                                            if (channel.ChannelMessages == null)
                                                channel.ChannelMessages = new List<ChannelMessage>();

                                            channel.ChannelMessages.AddRange(await MessagingHelper.SendMessages(Constants.Smashcast, new List<BroadcastMessage>() { message }));

                                            File.WriteAllText(Constants.ConfigRootDirectory + Constants.LiveDirectory + Constants.SmashcastDirectory + server.OwnerHitboxChannel + ".json", JsonConvert.SerializeObject(channel));

                                            Logging.LogSmashcast(server.OwnerHitboxChannel + " has gone online.");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public async Task CheckPicartoLive()
        {
            var servers = BotFiles.GetConfiguredServers();
            var liveChannels = BotFiles.GetCurrentlyLivePicartoChannels();

            // Loop through servers to broadcast.
            foreach (var server in servers)
            {
                if (!server.AllowLive)
                {
                    continue;
                }

                if (server.Id == 0 || server.GoLiveChannel == 0)
                { continue; }

                if (server.PicartoChannels != null)
                {
                    foreach (var picartoChannel in server.PicartoChannels)
                    {
                        var channel = liveChannels.FirstOrDefault(x => x.Name.ToLower() == picartoChannel.ToLower());

                        PicartoChannel stream = null;

                        try
                        {
                            stream = await picartoManager.GetChannelByName(picartoChannel);
                        }
                        catch (Exception wex)
                        {
                            // Log our error and move to the next user.

                            Logging.LogError("Picarto Error: " + wex.Message + " for user: " + picartoChannel + " in Discord Server Id: " + server.Id);
                            continue;
                        }

                        // if our stream isnt null, and we have a return from mixer.
                        if (stream != null)
                        {
                            if (stream.Online)
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
                                                    Name = picartoChannel,
                                                    Servers = new List<ulong>()
                                                };

                                                channel.Servers.Add(server.Id);

                                                liveChannels.Add(channel);
                                            }
                                            else
                                            {
                                                channel.Servers.Add(server.Id);
                                            }

                                            if (server.LiveMessage == null)
                                            {
                                                server.LiveMessage = "%CHANNEL% just went live - %TITLE% - %URL%";
                                            }

                                            string url = "https://picarto.tv/user_data/usrimg/" + stream.Name.ToLower() + "/dsdefault.jpg";

                                            EmbedBuilder embedBuilder = new EmbedBuilder();
                                            EmbedAuthorBuilder author = new EmbedAuthorBuilder();
                                            EmbedFooterBuilder footer = new EmbedFooterBuilder();

                                            author.IconUrl = "https://picarto.tv/user_data/usrimg/" + stream.Name.ToLower() + "/dsdefault.jpg";
                                            author.Name = stream.Name;
                                            author.Url = "https://picarto.tv/" + stream.Name;
                                            embedBuilder.Author = author;

                                            footer.IconUrl = "https://picarto.tv/images/Picarto_logo.png";
                                            footer.Text = "[Picarto] - " + DateTime.UtcNow.AddHours(server.TimeZoneOffset);
                                            embedBuilder.Footer = footer;

                                            embedBuilder.Title = stream.Name + " has gone live!";
                                            embedBuilder.Color = new Color(192, 192, 192);
                                            embedBuilder.ThumbnailUrl = server.AllowThumbnails ? "https://picarto.tv/user_data/usrimg/" + stream.Name.ToLower() + "/dsdefault.jpg" : "";
                                            embedBuilder.ImageUrl = "https://thumb.picarto.tv/thumbnail/" + stream.Name + ".jpg";

                                            embedBuilder.Description = server.LiveMessage.Replace("%CHANNEL%", stream.Name).Replace("%TITLE%", stream.Title).Replace("%URL%", "https://picarto.tv/" + stream.Name).Replace("%GAME%", stream.Category);

                                            embedBuilder.AddField(f =>
                                            {
                                                f.Name = "Category";
                                                f.Value = stream.Category;
                                                f.IsInline = true;
                                            });

                                            embedBuilder.AddField(f =>
                                            {
                                                f.Name = "Adult Stream?";
                                                f.Value = stream.Adult ? "Yup!" : "Nope!";
                                                f.IsInline = true;
                                            });

                                            embedBuilder.AddField(f =>
                                            {
                                                f.Name = "Total Viewers";
                                                f.Value = stream.ViewersTotal;
                                                f.IsInline = true;
                                            });

                                            embedBuilder.AddField(f =>
                                            {
                                                f.Name = "Total Followers";
                                                f.Value = stream.Followers;
                                                f.IsInline = true;
                                            });

                                            string tags = "";
                                            foreach (var t in stream.Tags)
                                            {
                                                tags += t + ", ";
                                            }

                                            embedBuilder.AddField(f =>
                                            {
                                                f.Name = "Stream Tags";
                                                f.Value = string.IsNullOrEmpty(tags.Trim().TrimEnd(',')) ? "None" : tags.Trim().TrimEnd(',');
                                                f.IsInline = false;
                                            });

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

                                                message += "**[Picarto]** " + server.LiveMessage.Replace("%CHANNEL%", stream.Name).Replace("%TITLE%", stream.Title).Replace("%URL%", "https://picarto.tv/" + stream.Name).Replace("%GAME%", stream.Category);
                                            }

                                            var broadcastMessage = new BroadcastMessage()
                                            {
                                                GuildId = server.Id,
                                                ChannelId = server.GoLiveChannel,
                                                UserId = picartoChannel,
                                                Message = message,
                                                Platform = Constants.Picarto,
                                                Embed = (!server.UseTextAnnouncements ? embedBuilder.Build() : null)
                                            };

                                            var finalCheck = BotFiles.GetCurrentlyLivePicartoChannels().FirstOrDefault(x => x.Name == picartoChannel);

                                            if (finalCheck == null || !finalCheck.Servers.Contains(server.Id))
                                            {
                                                if (channel.ChannelMessages == null)
                                                    channel.ChannelMessages = new List<ChannelMessage>();

                                                channel.ChannelMessages.AddRange(await MessagingHelper.SendMessages(Constants.Picarto, new List<BroadcastMessage>() { broadcastMessage }));

                                                File.WriteAllText(Constants.ConfigRootDirectory + Constants.LiveDirectory + Constants.PicartoDirectory + picartoChannel + ".json", JsonConvert.SerializeObject(channel));

                                                Logging.LogPicarto(picartoChannel + " has gone online.");
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

        public async Task CheckOwnerPicartoLive()
        {
            var servers = BotFiles.GetConfiguredServers();
            var liveChannels = BotFiles.GetCurrentlyLivePicartoChannels();

            // Loop through servers to broadcast.
            foreach (var server in servers)
            {
                if (!server.AllowLive)
                {
                    continue;
                }

                if (server.Id == 0 || server.OwnerLiveChannel == 0)
                { continue; }

                if (server.OwnerPicartoChannel != null)
                {
                    var channel = liveChannels.FirstOrDefault(x => x.Name.ToLower() == server.OwnerPicartoChannel.ToLower());

                    PicartoChannel stream = null;

                    try
                    {
                        stream = await picartoManager.GetChannelByName(server.OwnerPicartoChannel);
                    }
                    catch (Exception wex)
                    {
                        // Log our error and move to the next user.

                        Logging.LogError("Picarto Error: " + wex.Message + " for user: " + server.OwnerPicartoChannel + " in Discord Server Id: " + server.Id);
                        continue;
                    }

                    // if our stream isnt null, and we have a return from mixer.
                    if (stream != null)
                    {
                        if (stream.Online)
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
                                                Name = server.OwnerPicartoChannel,
                                                Servers = new List<ulong>()
                                            };

                                            channel.Servers.Add(server.Id);

                                            liveChannels.Add(channel);
                                        }
                                        else
                                        {
                                            channel.Servers.Add(server.Id);
                                        }

                                        if (server.LiveMessage == null)
                                        {
                                            server.LiveMessage = "%CHANNEL% just went live - %TITLE% - %URL%";
                                        }

                                        string url = "https://picarto.tv/user_data/usrimg/" + stream.Name.ToLower() + "/dsdefault.jpg";

                                        EmbedBuilder embedBuilder = new EmbedBuilder();
                                        EmbedAuthorBuilder author = new EmbedAuthorBuilder();
                                        EmbedFooterBuilder footer = new EmbedFooterBuilder();

                                        author.IconUrl = "https://picarto.tv/user_data/usrimg/" + stream.Name.ToLower() + "/dsdefault.jpg";
                                        author.Name = stream.Name;
                                        author.Url = "https://picarto.tv/" + stream.Name;
                                        embedBuilder.Author = author;

                                        footer.IconUrl = "https://picarto.tv/images/Picarto_logo.png";
                                        footer.Text = "[Picarto] - " + DateTime.UtcNow.AddHours(server.TimeZoneOffset);
                                        embedBuilder.Footer = footer;

                                        embedBuilder.Title = stream.Name + " has gone live!";
                                        embedBuilder.Color = new Color(192, 192, 192);
                                        embedBuilder.ThumbnailUrl = server.AllowThumbnails ? "https://picarto.tv/user_data/usrimg/" + stream.Name.ToLower() + "/dsdefault.jpg" : "";
                                        embedBuilder.ImageUrl = "https://thumb.picarto.tv/thumbnail/" + stream.Name + ".jpg";

                                        embedBuilder.Description = server.LiveMessage.Replace("%CHANNEL%", stream.Name).Replace("%TITLE%", stream.Title).Replace("%URL%", "https://picarto.tv/" + stream.Name).Replace("%GAME%", stream.Category);

                                        embedBuilder.AddField(f =>
                                        {
                                            f.Name = "Category";
                                            f.Value = stream.Category;
                                            f.IsInline = true;
                                        });

                                        embedBuilder.AddField(f =>
                                        {
                                            f.Name = "Adult Stream?";
                                            f.Value = stream.Adult ? "Yup!" : "Nope!";
                                            f.IsInline = true;
                                        });

                                        embedBuilder.AddField(f =>
                                        {
                                            f.Name = "Total Viewers";
                                            f.Value = stream.ViewersTotal;
                                            f.IsInline = true;
                                        });

                                        embedBuilder.AddField(f =>
                                        {
                                            f.Name = "Total Followers";
                                            f.Value = stream.Followers;
                                            f.IsInline = true;
                                        });

                                        string tags = "";
                                        foreach (var t in stream.Tags)
                                        {
                                            tags += t + ", ";
                                        }

                                        embedBuilder.AddField(f =>
                                        {
                                            f.Name = "Stream Tags";
                                            f.Value = tags.Trim().TrimEnd(',');
                                            f.IsInline = false;
                                        });

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

                                            message += "**[Picarto]** " + server.LiveMessage.Replace("%CHANNEL%", stream.Name).Replace("%TITLE%", stream.Title).Replace("%URL%", "https://picarto.tv/" + stream.Name).Replace("%GAME%", stream.Category);
                                        }

                                        var broadcastMessage = new BroadcastMessage()
                                        {
                                            GuildId = server.Id,
                                            ChannelId = server.GoLiveChannel,
                                            UserId = server.OwnerPicartoChannel,
                                            Message = message,
                                            Platform = Constants.Picarto,
                                            Embed = (!server.UseTextAnnouncements ? embedBuilder.Build() : null)
                                        };

                                        var finalCheck = BotFiles.GetCurrentlyLivePicartoChannels().FirstOrDefault(x => x.Name == server.OwnerPicartoChannel);

                                        if (finalCheck == null || !finalCheck.Servers.Contains(server.Id))
                                        {
                                            if (channel.ChannelMessages == null)
                                                channel.ChannelMessages = new List<ChannelMessage>();

                                            channel.ChannelMessages.AddRange(await MessagingHelper.SendMessages(Constants.Picarto, new List<BroadcastMessage>() { broadcastMessage }));

                                            File.WriteAllText(Constants.ConfigRootDirectory + Constants.LiveDirectory + Constants.PicartoDirectory + server.OwnerPicartoChannel + ".json", JsonConvert.SerializeObject(channel));

                                            Logging.LogPicarto(server.OwnerPicartoChannel + " has gone online.");
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
            var now = DateTime.UtcNow;
            var then = now.AddMilliseconds(-(Constants.YouTubePublishedInterval));

            foreach (var server in servers)
            {
                if (!server.AllowPublished)
                {
                    continue;
                }

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

                        YouTubeChannelSnippet channelData = null;

                        try
                        {
                            channelData = await youtubeManager.GetYouTubeChannelSnippetById(video.snippet.channelId);
                        }
                        catch (Exception wex)
                        {
                            // Log our error and move to the next user.

                            Logging.LogError("YouTube Error: " + wex.Message + " for user: " + video.snippet.channelId);
                            continue;
                        }

                        if (channelData == null)
                        {
                            continue;
                        }

                        if (server.PublishedMessage == null)
                        {
                            server.PublishedMessage = "%CHANNEL% just published a new video - %TITLE% - %URL%";
                        }

                        Color red = new Color(179, 18, 23);
                        author.IconUrl = client.CurrentUser.GetAvatarUrl() + "?_=" + Guid.NewGuid().ToString().Replace("-", "");
                        author.Name = Program.client.CurrentUser.Username;
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
                            ChannelId = server.PublishedChannel,
                            UserId = user,
                            Message = message,
                            Platform = Constants.YouTube,
                            Embed = (!server.UseTextAnnouncements ? embed.Build() : null)
                        });
                    }
                }
            }
        }

        public async Task CheckOwnerPublishedYouTube()
        {
            var servers = BotFiles.GetConfiguredServers();
            var users = BotFiles.GetConfiguredUsers();
            var now = DateTime.UtcNow;
            var then = now.AddMilliseconds(-(Constants.YouTubePublishedInterval));

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

                var chat = await DiscordHelper.GetMessageChannel(server.Id, server.OwnerPublishedChannel);

                if (chat == null)
                {
                    continue;
                }

                if (string.IsNullOrEmpty(server.OwnerYouTubeChannelId))
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

                    YouTubeChannelSnippet channelData = null;

                    try
                    {
                        channelData = await youtubeManager.GetYouTubeChannelSnippetById(video.snippet.channelId);

                    }
                    catch (Exception wex)
                    {
                        // Log our error and move to the next user.

                        Logging.LogError("YouTube Error: " + wex.Message + " for user: " + video.snippet.channelId);
                        continue;
                    }

                    if (channelData == null)
                    {
                        continue;
                    }

                    if (server.PublishedMessage == null)
                    {
                        server.PublishedMessage = "%CHANNEL% just published a new video - %TITLE% - %URL%";
                    }

                    Color red = new Color(179, 18, 23);
                    author.IconUrl = client.CurrentUser.GetAvatarUrl() + "?_=" + Guid.NewGuid().ToString().Replace("-", "");
                    author.Name = Program.client.CurrentUser.Username;
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

        public async Task CheckVidMe()
        {
            var servers = BotFiles.GetConfiguredServers();
            var users = BotFiles.GetConfiguredUsers();
            var now = DateTime.UtcNow;
            var then = now.AddMilliseconds(-(Constants.VidMeInterval));

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

                if (server.ServerVidMeChannelIds == null || server.ServerVidMeChannelIds.Count < 0)
                {
                    continue;
                }

                foreach (var channelId in server.ServerVidMeChannelIds)
                {
                    if (channelId == 0)
                    {
                        continue;
                    }

                    VidMeUserVideos videoResponse = null;

                    try
                    {
                        videoResponse = await vidMeManager.GetChannelVideosById(channelId);
                        
                        if (videoResponse == null || videoResponse.videos == null || videoResponse.videos.Count < 1)
                        {
                            continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logging.LogError("VidMe Published Error: " + ex.Message + " for user: " + channelId + " in Discord Server: " + server.Id);
                        continue;
                    }

                    foreach (var video in videoResponse.videos)
                    {
                        var publishDate = DateTime.Parse(video.date_published, null, System.Globalization.DateTimeStyles.AdjustToUniversal);

                        if (!(publishDate > then && publishDate < now))
                        {
                            continue;
                        }

                        string url = video.full_url;

                        EmbedBuilder embed = new EmbedBuilder();
                        EmbedAuthorBuilder author = new EmbedAuthorBuilder();
                        EmbedFooterBuilder footer = new EmbedFooterBuilder();

                        if (server.PublishedMessage == null)
                        {
                            server.PublishedMessage = "%CHANNEL% just published a new video - %TITLE% - %URL%";
                        }

                        Color red = new Color(179, 18, 23);
                        author.IconUrl = client.CurrentUser.GetAvatarUrl() + "?_=" + Guid.NewGuid().ToString().Replace("-", "");
                        author.Name = Program.client.CurrentUser.Username;
                        author.Url = url;
                        footer.Text = "[" + Constants.VidMe + "] - " + DateTime.UtcNow.AddHours(server.TimeZoneOffset);
                        footer.IconUrl = "http://couchbot.io/img/vidme.png";
                        embed.Author = author;
                        embed.Color = red;
                        embed.Description = server.PublishedMessage.Replace("%CHANNEL%", video.user.username).Replace("%GAME%", "a video").Replace("%TITLE%", video.title).Replace("%URL%", url);
                        embed.Title = video.user.username + " published a new video!";
                        embed.ThumbnailUrl = video.user.avatar_url;
                        embed.ImageUrl = video.thumbnail_url;
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

                            message += "**[" + Constants.VidMe + "]** " + 
                                server.PublishedMessage.Replace("%CHANNEL%", video.user.username).Replace("%GAME%", "a video").Replace("%TITLE%", video.title).Replace("%URL%", url);
                        }

                        Logging.LogVidMe(video.user.username + " has published a new video.");

                        await SendMessage(new BroadcastMessage()
                        {
                            GuildId = server.Id,
                            ChannelId = server.PublishedChannel,
                            UserId = video.user.username,
                            Message = message,
                            Platform = Constants.YouTube,
                            Embed = (!server.UseTextAnnouncements ? embed.Build() : null)
                        });
                    }
                }
            }
        }

        public async Task CheckOwnerVidMe()
        {
            var servers = BotFiles.GetConfiguredServers();
            var users = BotFiles.GetConfiguredUsers();
            var now = DateTime.UtcNow;
            var then = now.AddMilliseconds(-(Constants.VidMeInterval));

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

                var chat = await DiscordHelper.GetMessageChannel(server.Id, server.OwnerPublishedChannel);

                if (chat == null)
                {
                    continue;
                }

                if (string.IsNullOrEmpty(server.OwnerVidMeChannel) || server.OwnerVidMeChannelId == 0)
                {
                    continue;
                }

                var channelId = server.OwnerVidMeChannelId;
                VidMeUserVideos videoResponse = null;

                try
                {
                    videoResponse = await vidMeManager.GetChannelVideosById(channelId);

                    if (videoResponse == null || videoResponse.videos == null || videoResponse.videos.Count < 1)
                    {
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    Logging.LogError("VidMe Published Error: " + ex.Message + " for user: " + channelId + " in Discord Server: " + server.Id);
                    continue;
                }

                foreach (var video in videoResponse.videos)
                {
                    var publishDate = DateTime.Parse(video.date_published, null, System.Globalization.DateTimeStyles.AdjustToUniversal);

                    if (!(publishDate > then && publishDate < now))
                    {
                        continue;
                    }

                    string url = video.full_url;

                    EmbedBuilder embed = new EmbedBuilder();
                    EmbedAuthorBuilder author = new EmbedAuthorBuilder();
                    EmbedFooterBuilder footer = new EmbedFooterBuilder();

                    if (server.PublishedMessage == null)
                    {
                        server.PublishedMessage = "%CHANNEL% just published a new video - %TITLE% - %URL%";
                    }

                    Color red = new Color(179, 18, 23);
                    author.IconUrl = client.CurrentUser.GetAvatarUrl() + "?_=" + Guid.NewGuid().ToString().Replace("-", "");
                    author.Name = Program.client.CurrentUser.Username;
                    author.Url = url;
                    footer.Text = "[" + Constants.VidMe + "] - " + DateTime.UtcNow.AddHours(server.TimeZoneOffset);
                    footer.IconUrl = "http://couchbot.io/img/vidme.png";
                    embed.Author = author;
                    embed.Color = red;
                    embed.Description = server.PublishedMessage.Replace("%CHANNEL%", video.user.username).Replace("%GAME%", "a video").Replace("%TITLE%", video.title).Replace("%URL%", url);
                    embed.Title = video.user.username + " published a new video!";
                    embed.ThumbnailUrl = video.user.avatar_url;
                    embed.ImageUrl = video.thumbnail_url;
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

                        message += "**[" + Constants.VidMe + "]** " +
                            server.PublishedMessage.Replace("%CHANNEL%", video.user.username).Replace("%GAME%", "a video").Replace("%TITLE%", video.title).Replace("%URL%", url);
                    }

                    Logging.LogVidMe(video.user.username + " has published a new video.");

                    await SendMessage(new BroadcastMessage()
                    {
                        GuildId = server.Id,
                        ChannelId = server.OwnerPublishedChannel,
                        UserId = video.user.username,
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

                        if (Constants.EnableYouTube)
                        {
                            await CleanUpLiveStreams(Constants.YouTubeGaming);
                        }

                        if (Constants.EnableTwitch)
                        {
                            await CleanUpLiveStreams(Constants.Twitch);
                        }

                        if (Constants.EnableSmashcast)
                        {
                            await CleanUpLiveStreams(Constants.Smashcast);
                        }

                        if (Constants.EnablePicarto)
                        {
                            await CleanUpLiveStreams(Constants.Picarto);
                        }

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
                            await CleanupMessages(stream.ChannelMessages);

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

                            await CleanupMessages(stream.ChannelMessages);

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

                foreach (var live in Directory.GetFiles(Constants.ConfigRootDirectory + Constants.LiveDirectory + Constants.SmashcastDirectory))
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
                            await CleanupMessages(stream.ChannelMessages);

                            File.Delete(Constants.ConfigRootDirectory + Constants.LiveDirectory + Constants.SmashcastDirectory + stream.Name + ".json");
                        }
                    }
                    catch (Exception wex)
                    {

                        Logging.LogError("Clean Up Smashcast Error: " + wex.Message + " for user: " + stream.Name);
                    }
                }
            }

            if (platform == Constants.Picarto)
            {
                var liveStreams = new List<LiveChannel>();

                foreach (var live in Directory.GetFiles(Constants.ConfigRootDirectory + Constants.LiveDirectory + Constants.PicartoDirectory))
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
                        var liveStream = await picartoManager.GetChannelByName(stream.Name);

                        if (liveStream == null || !liveStream.Online)
                        {
                            await CleanupMessages(stream.ChannelMessages);

                            File.Delete(Constants.ConfigRootDirectory + Constants.LiveDirectory + Constants.PicartoDirectory + stream.Name + ".json");
                        }
                    }
                    catch (Exception wex)
                    {

                        Logging.LogError("Clean Up Picarto Error: " + wex.Message + " for user: " + stream.Name);
                    }
                }
            }
        }

        private async Task CleanupMessages(List<ChannelMessage> channelMessages)
        {
            if (channelMessages != null && channelMessages.Count > 0)
            {
                foreach (var message in channelMessages)
                {
                    var serverFile = BotFiles.GetConfiguredServers().FirstOrDefault(x => x.Id == message.GuildId);

                    if (serverFile == null)
                        continue;

                    if (serverFile.DeleteWhenOffline)
                    {
                        await DiscordHelper.DeleteMessage(message.GuildId, message.ChannelId, message.MessageId);
                    }
                    else
                    {
                        await DiscordHelper.SetOfflineStream(message.GuildId, serverFile.StreamOfflineMessage, message.ChannelId, message.MessageId);
                    }
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

                    if (message.Platform.Equals(Constants.Mixer))
                    {
                        statisticsManager.AddToBeamAlertCount();
                    }

                    if (message.Platform.Equals(Constants.Smashcast))
                    {
                        statisticsManager.AddToHitboxAlertCount();
                    }

                    if (message.Platform.Equals(Constants.VidMe))
                    {
                        statisticsManager.AddToVidMeAlertCount();
                    }
                }
                catch (Exception ex)
                {
                    Logging.LogError("Send Message Error: " + ex.Message + " in server " + message.GuildId);
                }
            }

            return null; // we never get here :(
        }

        private void CheckGuildConfigurations()
        {
            var files = BotFiles.GetConfiguredServerPaths();
            var badConfigurations = new List<DiscordServer>();

            foreach(var file in files)
            {
                var path = Path.GetFileNameWithoutExtension(file);
                var server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

                if(server.Id != ulong.Parse(path))
                {
                    Logging.LogInfo("Bad Configuration Found: " + path);

                    var guild = client.GetGuild(ulong.Parse(path));

                    if(guild == null)
                    {
                        continue;
                    }

                    var guildOwner = client.GetUser(guild.OwnerId);

                    server.Id = guild.Id;
                    server.Name = guild.Name;
                    server.OwnerId = guild.OwnerId;
                    server.OwnerName = guildOwner == null ? "" : guildOwner.Username;

                    BotFiles.SaveDiscordServer(server);

                    Logging.LogInfo("Server Configuration Fixed: " + path);
                }
            }
        }
    }
}