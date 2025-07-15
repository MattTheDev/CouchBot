using CB.Accessors.Contracts;
using CB.Data.Entities;
using Discord.WebSocket;

namespace CB.Bot.Services;

public class GuildInteractionService(DiscordSocketClient discordSocketClient,
    IServiceScopeFactory serviceScopeFactory)
{
    private IGuildAccessor _guildAccessor;
    private IUserAccessor _userAccessor;

    public void Init()
    {
        var scope = serviceScopeFactory.CreateScope();
        _guildAccessor = scope.ServiceProvider.GetRequiredService<IGuildAccessor>();
        _userAccessor = scope.ServiceProvider.GetRequiredService<IUserAccessor>();

        discordSocketClient.JoinedGuild += JoinedGuild;
    }

    private async Task JoinedGuild(SocketGuild arg)
    {
        var existingUser = await _userAccessor
            .GetByIdAsync(arg.OwnerId.ToString())
            .ConfigureAwait(false);
        var existingGuild = await _guildAccessor
            .GetByIdAsync(arg.Id.ToString())
            .ConfigureAwait(false);

        existingUser ??= await _userAccessor
            .CreateAsync(new()
            {
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,
                DisplayName = arg.Owner?.Username ?? "",
                Id = arg.OwnerId.ToString()
            })
            .ConfigureAwait(false);

        if (existingGuild == null)
        {
            var newGuild = new Guild
            {
                AllowConfiguration = new()
                {
                    AllowCrosspost = false,
                    AllowDiscordLive = false,
                    AllowFfa = false,
                    AllowGoodbyes = false,
                    AllowGreetings = false,
                    AllowLive = true,
                    AllowLiveDiscovery = false,
                    AllowPublished = true,
                    AllowStreamVod = false,
                    AllowThumbnails = true,
                },
                ChannelConfiguration = new()
                {
                    DiscordLiveChannelId = null,
                    GoodbyeChannelId = null,
                    GreetingChannelId = null,
                    LiveChannelId = null,
                },
                GuildConfiguration = new()
                {
                    DeleteOffline = true,
                    TextAnnouncements = false,
                },
                MessageConfiguration = new()
                {
                    GoodbyeMessage = "Good bye, %USER%, thanks for hanging out!",
                    GreetingMessage = "Welcome to the server, %USER%",
                    LiveMessage = "%CHANNEL% just went live with %GAME% - %TITLE% - %URL%",
                    PublishedMessage = "%CHANNEL% just published a new video - %TITLE% - %URL%",
                    StreamOfflineMessage = "This stream is now offline.",
                },
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,
                OwnerId = arg.OwnerId.ToString(),
                DisplayName = arg.Name,
                RoleConfiguration = new()
                {
                    DiscoveryRoleId = null,
                    JoinRoleId = null,
                    LiveDiscoveryRoleId = null,
                },
                Id = arg.Id.ToString()
            };

            existingGuild = await _guildAccessor
                .CreateAsync(newGuild)
                .ConfigureAwait(false);
        }
    }
}