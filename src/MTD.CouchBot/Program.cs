using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MTD.CouchBot.Dals;
using MTD.CouchBot.Dals.Implementations;
using MTD.CouchBot.Domain.Models.Bot;
using MTD.CouchBot.Domain.Utilities;
using MTD.CouchBot.Managers;
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
            var builder = new ConfigurationBuilder()    
                .SetBasePath(AppContext.BaseDirectory)  
                .AddJsonFile("BotSettings.json"); 
            _config = builder.Build();

            var provider = ConfigureServices();
                        
            await provider.GetRequiredService<StartupService>().StartAsync();
            provider.GetRequiredService<CommandHandler>();
            provider.GetRequiredService<LoggingService>();
            _discord = provider.GetRequiredService<DiscordShardedClient>();

            while (_discord.CurrentUser == null)
            {
                 Thread.Sleep(1000);
            }

            provider.GetRequiredService<GuildInteractionService>().Init();
            await provider.GetRequiredService<TimerQueueService>().Init();
 
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
                .AddSingleton<LoggingService>()
                .AddSingleton<IMixerDal, MixerDal>()
                .AddSingleton<IMobcrushDal, MobcrushDal>()
                .AddSingleton<IPicartoDal, PicartoDal>()
                .AddSingleton<ISmashcastDal, SmashcastDal>()
                .AddSingleton<IStrawpollDal, StrawPollDal>()
                .AddSingleton<ITwitchDal, TwitchDal>()
                .AddSingleton<IYouTubeDal, YouTubeDal>()
                .AddSingleton<IPiczelDal, PiczelDal>()
                .AddSingleton<MixerManager>()
                .AddSingleton<MobcrushManager>()
                .AddSingleton<PicartoManager>()
                .AddSingleton<SmashcastManager>()
                .AddSingleton<StrawPollManager>()
                .AddSingleton<TwitchManager>()
                .AddSingleton<YouTubeManager>()
                .AddSingleton<PiczelManager>()
                .AddSingleton<Random>();

            services.AddOptions();
            services.Configure<BotSettings>(_config);

            return services.BuildServiceProvider();
        }
    }
}