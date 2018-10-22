using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace MTD.CouchBot.Services
{
    public class TimerService
    {
        private readonly IConfiguration _configuration;
        private readonly LoggingService _loggingService;

        private Timer _twitchTimer;

        public TimerService(IConfiguration configuration, LoggingService loggingService)
        {
            _configuration = configuration;
            _loggingService = loggingService;
        }

        public async Task Init()
        {
            _twitchTimer = new Timer(async (e) =>
            {
                _loggingService.LogToConsole("Checking Twitch Creators.");
            }, null, 0, int.Parse(_configuration["Timers:TwitchTimer"]) * 1000);
        }
    }
}