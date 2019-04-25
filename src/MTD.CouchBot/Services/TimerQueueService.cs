using Discord.WebSocket;
using Microsoft.Extensions.Options;
using MTD.CouchBot.Domain;
using MTD.CouchBot.Domain.Models.Bot;
using MTD.CouchBot.Domain.Utilities;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace MTD.CouchBot.Services
{
    public class TimerQueueService
    {
        private readonly BotSettings _botSettings;

        private Timer _beamClientTimer;
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
        private Timer _twitchCommunityTimer;
        private Timer _picartoTimer;
        private Timer _picartoOwnerTimer;
        private Timer _piczelTimer;
        private Timer _piczelOwnerTimer;
        private Timer _guildCheckTimer;
        private Timer _twitchServerTimer;
        private Timer _cleanupTimer;
        private Timer _uptimeTimer;
        private Timer _customCommandTimer;

        private readonly DiscordShardedClient _discord;
        private readonly MixerConstellationService _mixerService;
        private readonly PlatformService _platformServices;
        private readonly GuildInteractionService _guildServices;
        private readonly FileService _fileService;
        private readonly CustomCommandService _commandService;

        private bool _initialServicesRan = false;

        public TimerQueueService(IOptions<BotSettings> botSettings, MixerConstellationService mixerService,
            PlatformService platformServices, GuildInteractionService guildServices, FileService fileService,
            DiscordShardedClient discord, CustomCommandService commandService)
        {
            _botSettings = botSettings.Value;
            _mixerService = mixerService;
            _platformServices = platformServices;
            _guildServices = guildServices;
            _fileService = fileService;
            _discord = discord;
            _commandService = commandService;
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
            QueueCustomCommands();
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
                Logging.LogMixer("Pinging Mixer.");
                await _mixerService.Ping();
            }, null, 0, 30000);

        }

        public void QueueHitboxChecks()
        {
            _hitboxTimer = new Timer(async (e) =>
            {
                ////var sw = new Stopwatch();
                ////sw.Start();
                Logging.LogSmashcast("Checking Smashcast Channels.");
                await _platformServices.CheckHitboxLive();
                ////sw.Stop();
                Logging.LogSmashcast("Smashcast Check Complete - Elapsed Runtime: " + " " + " milliseconds.");
            }, null, 0, _botSettings.IntervalSettings.Smashcast);

            _hitboxOwnerTimer = new Timer(async (e) =>
            {
                //var sw = new Stopwatch();
                //sw.Start();
                Logging.LogSmashcast("Checking Owner Smashcast Channels.");
                await _platformServices.CheckOwnerHitboxLive();
                //sw.Stop();
                Logging.LogSmashcast("Owner Smashcast Check Complete - Elapsed Runtime: " + " " + " milliseconds.");
            }, null, 0, _botSettings.IntervalSettings.Smashcast);
        }

        public void QueueTwitchChecks()
        {
            _twitchTimer = new Timer(async (e) =>
            {
                //var sw = new Stopwatch();
                //sw.Start();
                Logging.LogTwitch("Checking Twitch Channels.");
                await _platformServices.CheckTwitchLive();
                //sw.Stop();
                Logging.LogTwitch("Twitch Check Complete - Elapsed Runtime: " + " " + " milliseconds.");

                _initialServicesRan = true;
            }, null, 0, _botSettings.IntervalSettings.Twitch);
            
            _twitchTeamTimer = new Timer(async (e) =>
            {
                //var sw = new Stopwatch();
                //sw.Start();
                Logging.LogTwitch("Checking Twitch Teams.");
                await _platformServices.CheckTwitchTeams();
                //sw.Stop();
                Logging.LogTwitch("Checking Twitch Teams Check Complete - Elapsed Runtime: " + " " + " milliseconds.");
            }, null, 0, _botSettings.IntervalSettings.Twitch);

            _twitchGameTimer = new Timer(async (e) =>
            {
                //var sw = new Stopwatch();
                //sw.Start();
                Logging.LogTwitch("Checking Twitch Games.");
                await _platformServices.CheckTwitchGames();
                //sw.Stop();
                Logging.LogTwitch("Checking Twitch Games Check Complete - Elapsed Runtime: " + " " + " milliseconds.");
            }, null, 0, _botSettings.IntervalSettings.Twitch);
        }

        public void QueueYouTubeChecks()
        {
            _youtubeTimer = new Timer(async (e) =>
            {
                //var sw = new Stopwatch();
                //sw.Start();
                Logging.LogYouTubeGaming("Checking YouTube Gaming Channels.");
                await _platformServices.CheckYouTubeLive();
                //sw.Stop();
                Logging.LogYouTubeGaming("YouTube Gaming Check Complete - Elapsed Runtime: " + " " + " milliseconds.");
            }, null, 0, _botSettings.IntervalSettings.YouTubeLive);

            _youtubePublishedTimer = new Timer(async (e) =>
            {
                //var sw = new Stopwatch();
                //sw.Start();
                Logging.LogYouTube("Checking YouTube Published");
                await _platformServices.CheckPublishedYouTube();
                //sw.Stop();
                Logging.LogYouTube("YouTube Published Complete - Elapsed Runtime: " + " " + " milliseconds.");
            }, null, 0, _botSettings.IntervalSettings.YouTubePublished);

            _youtubePublishedOwnerTimer = new Timer(async (e) =>
            {
                //var sw = new Stopwatch();
                //sw.Start();
                Logging.LogYouTube("Checking Owner YouTube Published");
                await _platformServices.CheckOwnerPublishedYouTube();
                //sw.Stop();
                Logging.LogYouTube("Owner YouTube Published Complete - Elapsed Runtime: " + " " + " milliseconds.");
            }, null, 0, _botSettings.IntervalSettings.YouTubePublished);
        }
        
        public void QueuePicartoChecks()
        {
            _picartoTimer = new Timer(async (e) =>
            {
                //var sw = new Stopwatch();
                //sw.Start();
                Logging.LogPicarto("Checking Picarto Channels.");
                await _platformServices.CheckPicartoLive();
                //sw.Stop();
                Logging.LogPicarto("Picarto Check Complete - Elapsed Runtime: " + " " + " milliseconds.");
            }, null, 0, _botSettings.IntervalSettings.Picarto);

            _picartoOwnerTimer = new Timer(async (e) =>
            {
                //var sw = new Stopwatch();
                //sw.Start();
                Logging.LogPicarto("Checking Picarto Smashcast Channels.");
                await _platformServices.CheckOwnerPicartoLive();
                //sw.Stop();
                Logging.LogPicarto("Owner Picarto Check Complete - Elapsed Runtime: " + " " + " milliseconds.");
            }, null, 0, _botSettings.IntervalSettings.Picarto);
        }

        public void QueuePiczelChecks()
        {
            _piczelTimer = new Timer(async (e) =>
            {
                //var sw = new Stopwatch();
                //sw.Start();
                Logging.LogPiczel("Checking piczel Channels.");
                await _platformServices.CheckPiczelLive();
                //sw.Stop();
                Logging.LogPiczel("piczel Check Complete - Elapsed Runtime: " + " " + " milliseconds.");
            }, null, 0, _botSettings.IntervalSettings.Piczel);

            _piczelOwnerTimer = new Timer(async (e) =>
            {
                //var sw = new Stopwatch();
                //sw.Start();
                Logging.LogPiczel("Checking piczel Smashcast Channels.");
                await _platformServices.CheckOwnerPiczelLive();
                //sw.Stop();
                Logging.LogPiczel("Owner piczel Check Complete - Elapsed Runtime: " + " " + " milliseconds.");
            }, null, 0, _botSettings.IntervalSettings.Piczel);
        }

        public void QueueMobcrushChecks()
        {
            _mobcrushTimer = new Timer(async (e) =>
            {
                //var sw = new Stopwatch();
                //sw.Start();
                Logging.LogMobcrush("Checking Mobcrush Channels.");
                await _platformServices.CheckMobcrushLive();
                //sw.Stop();
                Logging.LogMobcrush("Mobcrush Check Complete - Elapsed Runtime: " + " " + " milliseconds.");
            }, null, 0, _botSettings.IntervalSettings.Mobcrush);

            _mobcrushOwnerTimer = new Timer(async (e) =>
            {
                //var sw = new Stopwatch();
                //sw.Start();
                Logging.LogMobcrush("Checking Owner Mobcrush Channels.");
                await _platformServices.CheckOwnerMobcrushLive();
                //sw.Stop();
                Logging.LogMobcrush("Owner Mobcrush Check Complete - Elapsed Runtime: " + " " + " milliseconds.");
            }, null, 0, _botSettings.IntervalSettings.Mobcrush);
        }

        public void QueueHealthChecks()
        {
            _guildCheckTimer = new Timer((e) =>
            {
                //var sw = new Stopwatch();
                //sw.Start();
                Logging.LogInfo("Checking Guild Configurations.");
                _guildServices.CheckGuildConfigurations();
                //sw.Stop();
                Logging.LogInfo("Guild Configuration Check Complete - Elapsed Runtime: " + " " + " milliseconds.");
            }, null, 0, 600000);
        }

        public void QueueTwitchServerCheck()
        {
            _twitchServerTimer = new Timer(async (e) =>
            {
                //var sw = new Stopwatch();
                //sw.Start();
                Logging.LogTwitch("Checking Twitch Server Channels.");
                await _platformServices.CheckTwitchServer();
                //sw.Stop();
                Logging.LogTwitch("Twitch Server Check Complete - Elapsed Runtime: " + " " + " milliseconds.");
            }, null, 0, _botSettings.IntervalSettings.TwitchServer);
        }

        public void QueueCleanUp()
        {
            _cleanupTimer = new Timer(async (e) =>
            {
                if (_initialServicesRan)
                {
                    Logging.LogInfo("Cleaning Up Live Files.");

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

                    Logging.LogInfo("Cleaning Up Live Files Complete.");
                }
            }, null, 0, 300000);
        }
    }
}
