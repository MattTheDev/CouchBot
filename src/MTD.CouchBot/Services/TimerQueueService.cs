using System;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using MTD.CouchBot.Domain;
using MTD.CouchBot.Domain.Models.Bot;
using MTD.CouchBot.Domain.Utilities;
using System.Threading;
using System.Threading.Tasks;
using Discord;

namespace MTD.CouchBot.Services
{
    public class TimerQueueService
    {
        private readonly BotSettings _botSettings;

        private Timer _mixerPingTimer;

        private Timer _etaTimer;

        private Timer _mobcrushTimer;
        private Timer _mobcrushOwnerTimer;
        private DateTime _mobcrushLastCompleted;

        private Timer _picartoTimer;
        private Timer _picartoOwnerTimer;
        private DateTime _picartoLastCompleted;

        private Timer _piczelTimer;
        private Timer _piczelOwnerTimer;
        private DateTime _piczelLastCompleted;

        private Timer _smashcastTimer;
        private Timer _smashcastOwnerTimer;
        private DateTime _smashcastLastCompleted;

        private Timer _twitchTeamTimer;
        private Timer _twitchGameTimer;
        private Timer _twitchTimer;
        private DateTime _twitchLastCompleted;

        private Timer _youtubeTimer;
        private Timer _youtubePublishedTimer;
        private Timer _youtubePublishedOwnerTimer;
        private DateTime _youtubeLiveLastCompleted;
        private DateTime _youtubePublishedLastCompleted;

        private Timer _guildCheckTimer;
        private Timer _twitchServerTimer;
        private Timer _cleanupTimer;
        private Timer _customCommandTimer;

        private readonly DiscordShardedClient _discord;
        private readonly MixerConstellationService _mixerService;
        private readonly PlatformService _platformServices;
        private readonly GuildInteractionService _guildServices;
        private readonly FileService _fileService;
        private readonly CustomCommandService _commandService;
        private readonly LoggingService _loggingService;

        private bool _initialServicesRan = false;

        public TimerQueueService(IOptions<BotSettings> botSettings, MixerConstellationService mixerService,
            PlatformService platformServices, GuildInteractionService guildServices, FileService fileService,
            DiscordShardedClient discord, CustomCommandService commandService, LoggingService loggingService)
        {
            _botSettings = botSettings.Value;
            _mixerService = mixerService;
            _platformServices = platformServices;
            _guildServices = guildServices;
            _fileService = fileService;
            _discord = discord;
            _commandService = commandService;
            _loggingService = loggingService;
        }

        public async Task Init()
        {
            if (_botSettings.PlatformSettings.EnableMixer)
            {
                await _mixerService.ResubscribeToBeamEvents();
                QueueBeamClientCheck();
            }

            if (_botSettings.PlatformSettings.EnableTwitch)
            {
                QueueTwitchChecks();
                QueueTwitchServerCheck();
            }

            if (_botSettings.PlatformSettings.EnableSmashcast)
            {
                QueueHitboxChecks();
            }

            if (_botSettings.PlatformSettings.EnablePicarto)
            {
                QueuePicartoChecks();
            }

            if (_botSettings.PlatformSettings.EnablePiczel)
            {
                QueuePiczelChecks();
            }

            if (_botSettings.PlatformSettings.EnableYouTube)
            {
                QueueYouTubeChecks();
            }

            if (_botSettings.PlatformSettings.EnableMobcrush)
            {
                QueueMobcrushChecks();
            }

            QueueCleanUp();
            QueueHealthChecks();

            if (_botSettings.BotConfig.EnableCustomTimerCommands)
            {
                QueueCustomCommands();
            }

            return; 
            // TODO MS
            _etaTimer = new Timer(async (e) =>
            {
                var mobcrushEta = (int)(DateTime.UtcNow - _mobcrushLastCompleted).TotalSeconds;
                var picartoEta = (int)(DateTime.UtcNow - _picartoLastCompleted).TotalSeconds;
                var piczelEta = (int)(DateTime.UtcNow - _piczelLastCompleted).TotalSeconds;
                var smashcastEta = (int)(DateTime.UtcNow - _smashcastLastCompleted).TotalSeconds;
                var twitchEta = (int)(DateTime.UtcNow - _twitchLastCompleted).TotalSeconds;
                var youtubeLiveEta = (int)(DateTime.UtcNow - _youtubeLiveLastCompleted).TotalSeconds;
                var youtubePublishedEta = (int)(DateTime.UtcNow - _youtubePublishedLastCompleted).TotalSeconds;

                if (_discord == null)
                {
                    return;
                }

                try
                {

                    var statusChannel = (IMessageChannel) _discord.GetChannel(601514799612035092);

                    if (statusChannel == null)
                    {
                        return;
                    }

                    var statusMessage =
                        (IUserMessage) await statusChannel.GetMessageAsync(601797024870039552);

                    if (statusMessage == null)
                    {
                        return;
                    }

                    await statusMessage.ModifyAsync(m =>
                        m.Content = $"**Bot Timer Status** \r\n" +
                                    $"**[Mixer]:** Constellation Status: {_mixerService.Status()}\r\n" +
                                    $"**[Mobcrush]:** {_botSettings.IntervalSettings.Mobcrush / 1000 - mobcrushEta} seconds remain. (Checked every {_botSettings.IntervalSettings.Mobcrush / 1000} seconds.)\r\n" +
                                    $"**[Picarto]:** {_botSettings.IntervalSettings.Picarto / 1000 - picartoEta} seconds remain. (Checked every {_botSettings.IntervalSettings.Picarto / 1000} seconds.)\r\n" +
                                    $"**[Piczel]:** {_botSettings.IntervalSettings.Piczel / 1000 - piczelEta} seconds remain. (Checked every {_botSettings.IntervalSettings.Piczel / 1000} seconds.)\r\n" +
                                    $"**[Smashcast]:** {_botSettings.IntervalSettings.Smashcast / 1000 - smashcastEta} seconds remain. (Checked every {_botSettings.IntervalSettings.Smashcast / 1000} seconds.)\r\n" +
                                    $"**[Twitch]:** {_botSettings.IntervalSettings.Twitch / 1000 - twitchEta} seconds remain. (Checked every {_botSettings.IntervalSettings.Twitch / 1000} seconds.)\r\n" +
                                    $"**[YouTube Live]:** {_botSettings.IntervalSettings.YouTubeLive / 1000 - youtubeLiveEta} seconds remain. (Checked every {_botSettings.IntervalSettings.YouTubeLive / 1000} seconds.)\r\n" +
                                    $"**[YouTube Published]:** {_botSettings.IntervalSettings.YouTubePublished / 1000 - youtubePublishedEta} seconds remain. (Checked every {_botSettings.IntervalSettings.YouTubePublished / 1000} seconds.)\r\n\r\n" +
                                    $"*(Last Checked: {DateTime.UtcNow})*");
                }
                catch (Exception)
                {
                    // Throwaway for now. TODO MS
                }
            }, null, 0, 5000);
        }

        public void QueueCustomCommands()
        {
            _customCommandTimer = new Timer(async (e) =>
            {
                foreach (var server in _fileService.GetServersWithCustomCommandsWithRepeat())
                {
                    await _commandService.ProcessRepeatedCustomCommand(server);
                }
            }, null, 0, 5000);
        }

        public void QueueBeamClientCheck()
        {
            _mixerPingTimer = new Timer(async (e) =>
            {
                _loggingService.LogMixer("Pinging Mixer.");
                await _mixerService.Ping();
            }, null, 0, 30000);
        }

        public void QueueHitboxChecks()
        {
            _smashcastTimer = new Timer(async (e) =>
            {
                _loggingService.LogSmashcast("Checking Smashcast Channels.");
                await _platformServices.CheckHitboxLive();
                _loggingService.LogSmashcast("Smashcast Check Complete - Elapsed Runtime: " + " " + " milliseconds.");
                _smashcastLastCompleted = DateTime.UtcNow;
            }, null, 0, _botSettings.IntervalSettings.Smashcast);

            _smashcastOwnerTimer = new Timer(async (e) =>
            {
                _loggingService.LogSmashcast("Checking Owner Smashcast Channels.");
                await _platformServices.CheckOwnerHitboxLive();
                _loggingService.LogSmashcast("Owner Smashcast Check Complete - Elapsed Runtime: " + " " + " milliseconds.");
            }, null, 0, _botSettings.IntervalSettings.Smashcast);
        }

        public void QueueTwitchChecks()
        {
            _twitchTimer = new Timer(async (e) =>
            {
                _loggingService.LogTwitch("Checking Twitch Channels.");
                await _platformServices.CheckTwitchLive();
                _loggingService.LogTwitch("Twitch Check Complete - Elapsed Runtime: " + " " + " milliseconds.");
                _twitchLastCompleted = DateTime.UtcNow;
                _initialServicesRan = true;
            }, null, 0, _botSettings.IntervalSettings.Twitch);
            
            _twitchTeamTimer = new Timer(async (e) =>
            {
                _loggingService.LogTwitch("Checking Twitch Teams.");
                await _platformServices.CheckTwitchTeams();
                _loggingService.LogTwitch("Checking Twitch Teams Check Complete - Elapsed Runtime: " + " " + " milliseconds.");
            }, null, 0, _botSettings.IntervalSettings.Twitch);

            _twitchGameTimer = new Timer(async (e) =>
            {
                _loggingService.LogTwitch("Checking Twitch Games.");
                await _platformServices.CheckTwitchGames();
                _loggingService.LogTwitch("Checking Twitch Games Check Complete - Elapsed Runtime: " + " " + " milliseconds.");
            }, null, 0, _botSettings.IntervalSettings.Twitch);
        }

        public void QueueYouTubeChecks()
        {
            _youtubeTimer = new Timer(async (e) =>
            {
                _loggingService.LogYouTubeGaming("Checking YouTube Gaming Channels.");
                await _platformServices.CheckYouTubeLive();
                _loggingService.LogYouTubeGaming("YouTube Gaming Check Complete - Elapsed Runtime: " + " " + " milliseconds.");
                _youtubeLiveLastCompleted = DateTime.UtcNow;
            }, null, 0, _botSettings.IntervalSettings.YouTubeLive);

            _youtubePublishedTimer = new Timer(async (e) =>
            {
                _loggingService.LogYouTube("Checking YouTube Published");
                await _platformServices.CheckPublishedYouTube();
                _loggingService.LogYouTube("YouTube Published Complete - Elapsed Runtime: " + " " + " milliseconds.");
                _youtubePublishedLastCompleted = DateTime.UtcNow;
            }, null, 0, _botSettings.IntervalSettings.YouTubePublished);

            _youtubePublishedOwnerTimer = new Timer(async (e) =>
            {
                _loggingService.LogYouTube("Checking Owner YouTube Published");
                await _platformServices.CheckOwnerPublishedYouTube();
                _loggingService.LogYouTube("Owner YouTube Published Complete - Elapsed Runtime: " + " " + " milliseconds.");
            }, null, 0, _botSettings.IntervalSettings.YouTubePublished);
        }
        
        public void QueuePicartoChecks()
        {
            _picartoTimer = new Timer(async (e) =>
            {
                _loggingService.LogPicarto("Checking Picarto Channels.");
                await _platformServices.CheckPicartoLive();
                _loggingService.LogPicarto("Picarto Check Complete - Elapsed Runtime: " + " " + " milliseconds.");
                _picartoLastCompleted = DateTime.UtcNow;
            }, null, 0, _botSettings.IntervalSettings.Picarto);

            _picartoOwnerTimer = new Timer(async (e) =>
            {
                _loggingService.LogPicarto("Checking Picarto Smashcast Channels.");
                await _platformServices.CheckOwnerPicartoLive();
                _loggingService.LogPicarto("Owner Picarto Check Complete - Elapsed Runtime: " + " " + " milliseconds.");
            }, null, 0, _botSettings.IntervalSettings.Picarto);
        }

        public void QueuePiczelChecks()
        {
            _piczelTimer = new Timer(async (e) =>
            {
                _loggingService.LogPiczel("Checking piczel Channels.");
                await _platformServices.CheckPiczelLive();
                _loggingService.LogPiczel("piczel Check Complete - Elapsed Runtime: " + " " + " milliseconds.");
                _piczelLastCompleted = DateTime.UtcNow;
            }, null, 0, _botSettings.IntervalSettings.Piczel);

            _piczelOwnerTimer = new Timer(async (e) =>
            {
                _loggingService.LogPiczel("Checking piczel Smashcast Channels.");
                await _platformServices.CheckOwnerPiczelLive();
                _loggingService.LogPiczel("Owner piczel Check Complete - Elapsed Runtime: " + " " + " milliseconds.");
            }, null, 0, _botSettings.IntervalSettings.Piczel);
        }

        public void QueueMobcrushChecks()
        {
            _mobcrushTimer = new Timer(async (e) =>
            {
                _loggingService.LogMobcrush("Checking Mobcrush Channels.");
                await _platformServices.CheckMobcrushLive();
                _loggingService.LogMobcrush("Mobcrush Check Complete - Elapsed Runtime: " + " " + " milliseconds.");
                _mobcrushLastCompleted = DateTime.UtcNow;
            }, null, 0, _botSettings.IntervalSettings.Mobcrush);

            _mobcrushOwnerTimer = new Timer(async (e) =>
            {
                _loggingService.LogMobcrush("Checking Owner Mobcrush Channels.");
                await _platformServices.CheckOwnerMobcrushLive();
                _loggingService.LogMobcrush("Owner Mobcrush Check Complete - Elapsed Runtime: " + " " + " milliseconds.");
            }, null, 0, _botSettings.IntervalSettings.Mobcrush);
        }

        public void QueueHealthChecks()
        {
            _guildCheckTimer = new Timer(async (e) =>
            {
                _loggingService.LogInfo("Checking Guild Configurations.");
                await _guildServices.CheckGuildConfigurations();
                _loggingService.LogInfo("Guild Configuration Check Complete - Elapsed Runtime: " + " " + " milliseconds.");
            }, null, 0, 600000);
        }

        public void QueueTwitchServerCheck()
        {
            _twitchServerTimer = new Timer(async (e) =>
            {
                //var sw = new Stopwatch();
                //sw.Start();
                _loggingService.LogTwitch("Checking Twitch Server Channels.");
                await _platformServices.CheckTwitchServer();
                //sw.Stop();
                _loggingService.LogTwitch("Twitch Server Check Complete - Elapsed Runtime: " + " " + " milliseconds.");
            }, null, 0, _botSettings.IntervalSettings.TwitchServer);
        }

        public void QueueCleanUp()
        {
            _cleanupTimer = new Timer(async (e) =>
            {
                if (_initialServicesRan)
                {
                    _loggingService.LogInfo("Cleaning Up Live Files.");

                    if (_botSettings.PlatformSettings.EnableYouTube)
                    {
                        await _platformServices.CleanUpLiveStreams(Constants.YouTubeGaming);
                    }

                    if (_botSettings.PlatformSettings.EnableTwitch)
                    {
                        await _platformServices.CleanUpLiveStreams(Constants.Twitch);
                    }

                    if (_botSettings.PlatformSettings.EnableSmashcast)
                    {
                        await _platformServices.CleanUpLiveStreams(Constants.Smashcast);
                    }

                    if (_botSettings.PlatformSettings.EnablePicarto)
                    {
                        await _platformServices.CleanUpLiveStreams(Constants.Picarto);
                    }

                    if (_botSettings.PlatformSettings.EnableMobcrush)
                    {
                        await _platformServices.CleanUpLiveStreams(Constants.Mobcrush);
                    }

                    _loggingService.LogInfo("Cleaning Up Live Files Complete.");
                }
            }, null, 0, 300000);
        }
    }
}
