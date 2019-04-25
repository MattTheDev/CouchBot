using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MTD.CouchBot.Dals;
using MTD.CouchBot.Dals.Implementations;
using MTD.CouchBot.Domain.Models.Bot;
using MTD.CouchBot.Domain.Utilities;
using MTD.CouchBot.Managers;
using MTD.CouchBot.Managers.Implementations;
using MTD.CouchBot.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MTD.CouchBot
{
    public class Program
    {
        private IConfigurationRoot _config;
        private DiscordShardedClient _discord;

        static void Main() => new Program().Start().GetAwaiter().GetResult();

        public async Task Start()
        {
            Logging.LogInfo("Starting the Bot.");
            Logging.LogInfo("Building the Configuration.");
            var builder = new ConfigurationBuilder()    
                .SetBasePath(AppContext.BaseDirectory)  
                .AddJsonFile("BotSettings.json"); 
            _config = builder.Build();
            Logging.LogInfo("Completed - Building the Configuration.");

            Logging.LogInfo("Configuring the Services");
            var provider = ConfigureServices();
            Logging.LogInfo("Completed - Configuring the Services");
                        
            await provider.GetRequiredService<StartupService>().StartAsync();
            provider.GetRequiredService<CommandHandler>();
            _discord = provider.GetRequiredService<DiscordShardedClient>();

            while (_discord.CurrentUser == null)
            {
                Logging.LogInfo("Waiting for User to Log In...");
                Thread.Sleep(1000);
            }

            Logging.LogInfo("Setting Up Event Handlers.");
            provider.GetRequiredService<GuildInteractionService>().Init();
            Logging.LogInfo("Completed - Setting Up Event Handlers.");

            Logging.LogInfo("Initializing Timers.");
            await provider.GetRequiredService<TimerQueueService>().Init();
            Logging.LogInfo("Completed - Initializing Timers.");

            await Task.Delay(-1);
        }

        public IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection()
                .AddSingleton(new DiscordShardedClient(new DiscordSocketConfig()))
                .AddSingleton(new CommandService())
                .AddSingleton<CommandHandler>()
                .AddSingleton<StartupService>()
                .AddSingleton<MixerConstellationService>()
                .AddSingleton<FileService>()
                .AddSingleton<GuildInteractionService>()
                .AddSingleton<DiscordService>()
                .AddSingleton<MessagingService>()
                .AddSingleton<PlatformService>()
                .AddSingleton<TimerQueueService>()
                .AddSingleton<StringService>()
                .AddSingleton<CustomCommandService>()
                .AddSingleton<IMixerDal, MixerDal>()
                .AddSingleton<IMobcrushDal, MobcrushDal>()
                .AddSingleton<IPicartoDal, PicartoDal>()
                .AddSingleton<ISmashcastDal, SmashcastDal>()
                .AddSingleton<IStrawpollDal, StrawPollDal>()
                .AddSingleton<ITwitchDal, TwitchDal>()
                .AddSingleton<IYouTubeDal, YouTubeDal>()
                .AddSingleton<IPiczelDal, PiczelDal>()
                .AddSingleton<IMixerManager, MixerManager>()
                .AddSingleton<IMobcrushManager, MobcrushManager>()
                .AddSingleton<IPicartoManager, PicartoManager>()
                .AddSingleton<ISmashcastManager, SmashcastManager>()
                .AddSingleton<IStrawPollManager, StrawPollManager>()
                .AddSingleton<ITwitchManager, TwitchManager>()
                .AddSingleton<IYouTubeManager, YouTubeManager>()
                .AddSingleton<IPiczelManager, PiczelManager>()
                .AddSingleton<Random>();

            services.AddOptions();
            services.Configure<BotSettings>(_config);

            return services.BuildServiceProvider();
        }
    }
}