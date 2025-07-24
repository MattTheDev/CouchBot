using CB.Accessors.Contracts;
using CB.Accessors.Implementations;
using CB.Bot.Services;
using CB.Data;
using CB.Engines.Contracts;
using CB.Engines.Implementations;
using CB.Shared.Dtos;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = Host.CreateApplicationBuilder(args);
var socketConfig = new DiscordSocketConfig
{
    LogLevel = LogSeverity.Verbose,
    GatewayIntents =
        GatewayIntents.GuildVoiceStates |
        GatewayIntents.GuildScheduledEvents |
        GatewayIntents.DirectMessages |
        GatewayIntents.GuildIntegrations |
        GatewayIntents.GuildMessageReactions |
        GatewayIntents.Guilds |
        GatewayIntents.GuildMessages,
    UseInteractionSnowflakeDate = false,
    AlwaysDownloadUsers = true,
};

var client = new DiscordSocketClient(socketConfig);
builder.Services.AddSingleton(client);
builder.Services.AddSingleton<CommandService>();
builder.Services.AddSingleton(new InteractionService(client));
builder.Services.AddSingleton<GuildInteractionService>();
builder.Services.AddSingleton<MessageInteractionService>();
builder.Services.AddScoped<IAllowConfigurationAccessor, AllowConfigurationAccessor>();
builder.Services.AddScoped<IChannelAccessor, ChannelAccessor>();
builder.Services.AddScoped<IChannelConfigurationAccessor, ChannelConfigurationAccessor>();
builder.Services.AddScoped<IClipEmbedAccessor, ClipEmbedAccessor>();
builder.Services.AddScoped<ICreatorAccessor, CreatorAccessor>();
builder.Services.AddScoped<ICreatorChannelAccessor, CreatorChannelAccessor>();
builder.Services.AddScoped<IDiscordLiveConfigurationAccessor, DiscordLiveConfigurationAccessor>();
builder.Services.AddScoped<IDropdownPayloadAccessor, DropdownPayloadAccessor>();
builder.Services.AddScoped<IFilterAccessor, FilterAccessor>();
builder.Services.AddScoped<IGuildAccessor, GuildAccessor>();
builder.Services.AddScoped<IGuildConfigurationAccessor, GuildConfigurationAccessor>();
builder.Services.AddScoped<ILiveEmbedAccessor, LiveEmbedAccessor>();
builder.Services.AddScoped<IMessageConfigurationAccessor, MessageConfigurationAccessor>();
builder.Services.AddScoped<IRoleConfigurationAccessor, RoleConfigurationAccessor>();
builder.Services.AddScoped<IUserAccessor, UserAccessor>();
builder.Services.AddScoped<IVodEmbedAccessor, VodEmbedAccessor>();
builder.Services.AddScoped<IYouTubeAccessor, YouTubeAccessor>();
builder.Services.AddScoped<ICreatorEngine, CreatorEngine>();

builder.Services.AddDbContext<CbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAutoMapper(cfg => { }, typeof(CbProfile));

builder.Services.AddHostedService<DiscordBotService>();
builder.Services.AddHttpClient();
var host = builder.Build();
host.Run();
