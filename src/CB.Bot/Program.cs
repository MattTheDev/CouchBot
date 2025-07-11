using CB.Bot.Services;
using CB.Data;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;

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
};

var client = new DiscordSocketClient(socketConfig);
builder.Services.AddSingleton(client);
builder.Services.AddSingleton<CommandService>();
builder.Services.AddSingleton(new InteractionService(client));

builder.Services.AddDbContext<CbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHostedService<DiscordBotService>();

var host = builder.Build();
host.Run();
