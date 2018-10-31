using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace MTD.CouchBot.Services
{
    public class TimerService
    {
        private Timer _twitchTimer;
        //private Timer _teamTwitchTimer;
        //private Timer _gameTwitchTimer;
        //private Timer _clipTwitchTimer;
        //private Timer _discoveryTwitchTimer;
        //private Timer _vodTwitchTimer;
        // TODO Figure out if this is a thing.
        //private Timer _vodCastTwitchTimer;

        private readonly PlatformService _platformService;
        private readonly IConfiguration _configuration;

        public TimerService(IConfiguration configuration, PlatformService platformService)
        {
            _configuration = configuration;
            _platformService = platformService;
        }

        public async Task Init()
        {
            await QueueGroupTwitchCheck();
        }

        public async Task QueueGroupTwitchCheck()
        {
            _twitchTimer = new Timer(async e =>
            {
                await _platformService.CheckTwitchStreams();
            }, null, 0, int.Parse(_configuration["Timers:TwitchTimer"]) * 1000);
        }
    }
}