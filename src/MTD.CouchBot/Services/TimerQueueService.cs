using Discord.WebSocket;
using Microsoft.Extensions.Options;
using MTD.CouchBot.Domain;
using MTD.CouchBot.Domain.Models.Bot;
using MTD.CouchBot.Domain.Utilities;
using System.Threading;
using System.Threading.Tasks;

namespace MTD.CouchBot.Services
{
    public class TimerQueueService
    {
        private readonly BotSettings _botSettings;

        private Timer _mixerPingTimer;
        private Timer _hitboxTimer;
        private Timer _hitboxOwnerTimer;
        private Timer _mobcrushTimer;
        private Timer _mobcrushOwnerTimer;
        private Timer _twitchTimer;
        private Timer _youtubeTimer;
        private Timer _youtubePublishedTimer;
        private Timer _youtubePublishedOwnerTimer;
        private Timer _twitchTeamTimer;
        private Timer _twitchGameTimer;
        private Timer _picartoTimer;
        private Timer _picartoOwnerTimer;
        private Timer _piczelTimer;
        private Timer _piczelOwnerTimer;
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
            _hitboxTimer = new Timer(async (e) =>
            {
                ////var sw = new Stopwatch();
                ////sw.Start();
                _loggingService.LogSmashcast("Checking Smashcast Channels.");
                await _platformServices.CheckHitboxLive();
                ////sw.Stop();
                _loggingService.LogSmashcast("Smashcast Check Complete - Elapsed Runtime: " + " " + " milliseconds.");
            }, null, 0, _botSettings.IntervalSettings.Smashcast);

            _hitboxOwnerTimer = new Timer(async (e) =>
            {
                //var sw = new Stopwatch();
                //sw.Start();
                _loggingService.LogSmashcast("Checking Owner Smashcast Channels.");
                await _platformServices.CheckOwnerHitboxLive();
                //sw.Stop();
                _loggingService.LogSmashcast("Owner Smashcast Check Complete - Elapsed Runtime: " + " " + " milliseconds.");
            }, null, 0, _botSettings.IntervalSettings.Smashcast);
        }

        public void QueueTwitchChecks()
        {
            _twitchTimer = new Timer(async (e) =>
            {
                //var sw = new Stopwatch();
                //sw.Start();
                _loggingService.LogTwitch("Checking Twitch Channels.");
                await _platformServices.CheckTwitchLive();
                //sw.Stop();
                _loggingService.LogTwitch("Twitch Check Complete - Elapsed Runtime: " + " " + " milliseconds.");

                _initialServicesRan = true;
            }, null, 0, _botSettings.IntervalSettings.Twitch);
            
            _twitchTeamTimer = new Timer(async (e) =>
            {
                //var sw = new Stopwatch();
                //sw.Start();
                _loggingService.LogTwitch("Checking Twitch Teams.");
                await _platformServices.CheckTwitchTeams();
                //sw.Stop();
                _loggingService.LogTwitch("Checking Twitch Teams Check Complete - Elapsed Runtime: " + " " + " milliseconds.");
            }, null, 0, _botSettings.IntervalSettings.Twitch);

            _twitchGameTimer = new Timer(async (e) =>
            {
                //var sw = new Stopwatch();
                //sw.Start();
                _loggingService.LogTwitch("Checking Twitch Games.");
                await _platformServices.CheckTwitchGames();
                //sw.Stop();
                _loggingService.LogTwitch("Checking Twitch Games Check Complete - Elapsed Runtime: " + " " + " milliseconds.");
            }, null, 0, _botSettings.IntervalSettings.Twitch);
        }

        public void QueueYouTubeChecks()
        {
            _youtubeTimer = new Timer(async (e) =>
            {
                //var sw = new Stopwatch();
                //sw.Start();
                _loggingService.LogYouTubeGaming("Checking YouTube Gaming Channels.");
                await _platformServices.CheckYouTubeLive();
                //sw.Stop();
                _loggingService.LogYouTubeGaming("YouTube Gaming Check Complete - Elapsed Runtime: " + " " + " milliseconds.");
            }, null, 0, _botSettings.IntervalSettings.YouTubeLive);

            _youtubePublishedTimer = new Timer(async (e) =>
            {
                //var sw = new Stopwatch();
                //sw.Start();
                _loggingService.LogYouTube("Checking YouTube Published");
                await _platformServices.CheckPublishedYouTube();
                //sw.Stop();
                _loggingService.LogYouTube("YouTube Published Complete - Elapsed Runtime: " + " " + " milliseconds.");
            }, null, 0, _botSettings.IntervalSettings.YouTubePublished);

            _youtubePublishedOwnerTimer = new Timer(async (e) =>
            {
                //var sw = new Stopwatch();
                //sw.Start();
                _loggingService.LogYouTube("Checking Owner YouTube Published");
                await _platformServices.CheckOwnerPublishedYouTube();
                //sw.Stop();
                _loggingService.LogYouTube("Owner YouTube Published Complete - Elapsed Runtime: " + " " + " milliseconds.");
            }, null, 0, _botSettings.IntervalSettings.YouTubePublished);
        }
        
        public void QueuePicartoChecks()
        {
            _picartoTimer = new Timer(async (e) =>
            {
                //var sw = new Stopwatch();
                //sw.Start();
                _loggingService.LogPicarto("Checking Picarto Channels.");
                await _platformServices.CheckPicartoLive();
                //sw.Stop();
                _loggingService.LogPicarto("Picarto Check Complete - Elapsed Runtime: " + " " + " milliseconds.");
            }, null, 0, _botSettings.IntervalSettings.Picarto);

            _picartoOwnerTimer = new Timer(async (e) =>
            {
                //var sw = new Stopwatch();
                //sw.Start();
                _loggingService.LogPicarto("Checking Picarto Smashcast Channels.");
                await _platformServices.CheckOwnerPicartoLive();
                //sw.Stop();
                _loggingService.LogPicarto("Owner Picarto Check Complete - Elapsed Runtime: " + " " + " milliseconds.");
            }, null, 0, _botSettings.IntervalSettings.Picarto);
        }

        public void QueuePiczelChecks()
        {
            _piczelTimer = new Timer(async (e) =>
            {
                //var sw = new Stopwatch();
                //sw.Start();
                _loggingService.LogPiczel("Checking piczel Channels.");
                await _platformServices.CheckPiczelLive();
                //sw.Stop();
                _loggingService.LogPiczel("piczel Check Complete - Elapsed Runtime: " + " " + " milliseconds.");
            }, null, 0, _botSettings.IntervalSettings.Piczel);

            _piczelOwnerTimer = new Timer(async (e) =>
            {
                //var sw = new Stopwatch();
                //sw.Start();
                _loggingService.LogPiczel("Checking piczel Smashcast Channels.");
                await _platformServices.CheckOwnerPiczelLive();
                //sw.Stop();
                _loggingService.LogPiczel("Owner piczel Check Complete - Elapsed Runtime: " + " " + " milliseconds.");
            }, null, 0, _botSettings.IntervalSettings.Piczel);
        }

        public void QueueMobcrushChecks()
        {
            _mobcrushTimer = new Timer(async (e) =>
            {
                //var sw = new Stopwatch();
                //sw.Start();
                _loggingService.LogMobcrush("Checking Mobcrush Channels.");
                await _platformServices.CheckMobcrushLive();
                //sw.Stop();
                _loggingService.LogMobcrush("Mobcrush Check Complete - Elapsed Runtime: " + " " + " milliseconds.");
            }, null, 0, _botSettings.IntervalSettings.Mobcrush);

            _mobcrushOwnerTimer = new Timer(async (e) =>
            {
                //var sw = new Stopwatch();
                //sw.Start();
                _loggingService.LogMobcrush("Checking Owner Mobcrush Channels.");
                await _platformServices.CheckOwnerMobcrushLive();
                //sw.Stop();
                _loggingService.LogMobcrush("Owner Mobcrush Check Complete - Elapsed Runtime: " + " " + " milliseconds.");
            }, null, 0, _botSettings.IntervalSettings.Mobcrush);
        }

        public void QueueHealthChecks()
        {
            _guildCheckTimer = new Timer(async (e) =>
            {
                //var sw = new Stopwatch();
                //sw.Start();
                _loggingService.LogInfo("Checking Guild Configurations.");
                await _guildServices.CheckGuildConfigurations();
                //sw.Stop();
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
