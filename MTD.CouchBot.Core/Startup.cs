using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MTD.CouchBot.Dals;
using MTD.CouchBot.Dals.Implementations;
using MTD.CouchBot.Localization;
using MTD.CouchBot.Managers;
using MTD.CouchBot.Managers.Implementations;
using MTD.CouchBot.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace MTD.CouchBot.Core
{
    public class Startup
    {
        private IConfiguration Configuration { get; }
        private List<Translation> Translations { get; set; }

        public Startup()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("BotConfig.json");
            Configuration = builder.Build();

            Translations = new List<Translation>();
        }

        public static async Task RunAsync(string[] args)
        {
            var startup = new Startup();
            await startup.RunAsync().ConfigureAwait(false);
        }

        public async Task RunAsync()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);

            // Download and Initialized Localization
            DownloadLatestLocalizationFiles();
            SetupLocalizationFolder();
            CleanupTranslationSetup();
            LoadTranslations();

            var provider = services.BuildServiceProvider();
            var startup = provider.GetRequiredService<StartupService>();
            startup.CreateRequiredDirectories();
            var logger = provider.GetRequiredService<LoggingService>();
            provider.GetRequiredService<CommandHandler>();

            await startup.StartAsync();
            var discord = provider.GetRequiredService<DiscordSocketClient>();

            while (discord.CurrentUser == null)
            {
                logger.LogToConsole("Waiting for User to Log In...");
                Thread.Sleep(1000);
            }

            provider.GetRequiredService<GuildInteractionService>().Init();

            await Task.Delay(-1).ConfigureAwait(false);
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose,
                MessageCacheSize = 1000
            }))
            .AddSingleton(new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Verbose,
                DefaultRunMode = RunMode.Async,
                CaseSensitiveCommands = false
            }))
            .AddSingleton<StartupService>()
            .AddSingleton<CommandHandler>()
            .AddSingleton<LoggingService>()
            .AddSingleton<GuildInteractionService>()
            .AddSingleton<IGuildDal, GuildDal>()
            .AddSingleton<IGuildManager, GuildManager>()
            .AddSingleton<ITwitchDal, TwitchDal>()
            .AddSingleton<IGroupDal, GroupDal>()
            .AddSingleton<IGroupManager, GroupManager>()
            .AddSingleton<ITwitchManager, TwitchManager>()
            .AddSingleton<IMixerManager, MixerManager>()
            .AddSingleton<IMixerDal, MixerDal>()
            .AddSingleton<IYouTubeDal, YouTubeDal>()
            .AddSingleton<IYouTubeManager, YouTubeManager>()
            .AddSingleton(Configuration)
            .AddSingleton(Translations);
        }

        private void DownloadLatestLocalizationFiles()
        {
            using (var client = new WebClient())
            {
                client.DownloadFile(new Uri(Configuration["Localization:TranslationFiles"]), "LocalizationFiles.zip");
            }
        }

        private void SetupLocalizationFolder()
        {
            Directory.Delete("./translations", true);
            ZipFile.ExtractToDirectory("LocalizationFiles.zip", "TranslationTemp", true);
            Directory.Move("./TranslationTemp/CouchBot-Localization-master/translations", "translations");
        }

        private void CleanupTranslationSetup()
        {
            Directory.Delete("./TranslationTemp", true);
            File.Delete("LocalizationFiles.zip");
        }

        private void LoadTranslations()
        {
            foreach (var file in Directory.GetFiles("./translations"))
            {
                Translations.Add(JsonConvert.DeserializeObject<Translation>(File.ReadAllText(file)));
            }
        }
    }
}