using CB.Accessors.Contracts;
using CB.Accessors.Implementations;
using CB.Bot.Services;
using CB.Data;
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
builder.Services.AddScoped<IGuildAccessor, GuildAccessor>();
builder.Services.AddScoped<IUserAccessor, UserAccessor>();

builder.Services.AddDbContext<CbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAutoMapper(cfg => { }, typeof(CbProfile));

builder.Services.AddHostedService<DiscordBotService>();

var host = builder.Build();
host.Run();
