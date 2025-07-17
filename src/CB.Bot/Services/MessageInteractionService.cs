using System.Text;
using CB.Accessors.Contracts;
using CB.Engines.Contracts;
using CB.Shared;
using CB.Shared.Enums;
using CB.Shared.Models.Bot;
using CB.Shared.Responses;
using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace CB.Bot.Services;

public class MessageInteractionService(DiscordSocketClient discordSocketClient,
    IServiceScopeFactory serviceScopeFactory)
{
        private IAllowConfigurationAccessor _allowConfigurationAccessor;
        private IDropdownPayloadAccessor _dropdownPayloadAccessor;
        private IGuildAccessor _guildAccessor;
        private IGuildConfigurationAccessor _guildConfigurationAccessor;
        private IYouTubeAccessor _youTubeAccessor;
        private ICreatorEngine _creatorEngine;

    public void Init()
    {
        var scope = serviceScopeFactory.CreateScope();
        _allowConfigurationAccessor = scope.ServiceProvider.GetRequiredService<IAllowConfigurationAccessor>();
        _dropdownPayloadAccessor = scope.ServiceProvider.GetRequiredService<IDropdownPayloadAccessor>();
        _guildAccessor = scope.ServiceProvider.GetRequiredService<IGuildAccessor>();
        _guildConfigurationAccessor = scope.ServiceProvider.GetRequiredService<IGuildConfigurationAccessor>();
        _youTubeAccessor = scope.ServiceProvider.GetRequiredService<IYouTubeAccessor>();
        _creatorEngine = scope.ServiceProvider.GetRequiredService<ICreatorEngine>();

        discordSocketClient.SelectMenuExecuted += DiscordSelectMenuChanged;
    }

    private async Task DiscordSelectMenuChanged(SocketMessageComponent arg)
    {
        var guildChannel = (IGuildChannel)arg.Channel;

        await arg.DeferAsync();

        var creatorId = ulong.Parse(arg.Data.CustomId.Split(".")[1]);

        if (creatorId != arg.User.Id)
        {
            await arg.DeferAsync();
            return;
        }

        var dropdownPayloadId = int.Parse(arg.Data.CustomId.Split(".")[2]);

        var payload = await _dropdownPayloadAccessor.GetByIdAsync(dropdownPayloadId);

        switch (Enum.Parse<DropdownType>(payload.DropdownType))
        {
            case DropdownType.AllowConfiguration:
                await ProcessAllowDropdownAsync(arg.Channel,
                    guildChannel.Guild,
                    arg.Data.Values.ToList(),
                    arg)
                    .ConfigureAwait(false);

                break;
            case DropdownType.YouTubeCreator:
                var creatorPayload =
                    JsonConvert.DeserializeObject<YouTubeCreatorDropdownPayload>(payload.Payload);

                await ProcessYouTubeCreatorDropdownAsync(arg, arg, creatorPayload);

                break;
        }
    }

    private async Task ProcessAllowDropdownAsync(ISocketMessageChannel channel,
        IGuild discordGuild,
        IReadOnlyCollection<string> options,
        SocketMessageComponent message)
    {
        var guild = await _guildAccessor.GetByIdAsync(discordGuild.Id.ToString());
        if (guild == null)
        {
            return;
        }

        var response = new StringBuilder();
        response.AppendLine(" ");

        foreach (var option in options)
        {
            switch (option)
            {
                case Constants.AllowOptions.GreetingsOption:
                    guild.AllowConfiguration.AllowGreetings = !guild.AllowConfiguration.AllowGreetings;
                    response.AppendLine(GetStatusOutput(
                        discordGuild,
                        !guild.AllowConfiguration.AllowGreetings,
                        guild.AllowConfiguration.AllowGreetings,
                        "Greetings"));
                    break;
                case Constants.AllowOptions.GoodbyesOption:
                    guild.AllowConfiguration.AllowGoodbyes = !guild.AllowConfiguration.AllowGoodbyes;
                    response.AppendLine(GetStatusOutput(
                        discordGuild,
                        !guild.AllowConfiguration.AllowGoodbyes,
                        guild.AllowConfiguration.AllowGoodbyes,
                        "Goodbyes"));
                    break;
                case Constants.AllowOptions.LiveOption:
                    guild.AllowConfiguration.AllowLive = !guild.AllowConfiguration.AllowLive;
                    response.AppendLine(GetStatusOutput(
                        discordGuild,
                        !guild.AllowConfiguration.AllowLive,
                        guild.AllowConfiguration.AllowLive,
                        "Live"));
                    break;
                case Constants.AllowOptions.PublishedOption:
                    guild.AllowConfiguration.AllowPublished = !guild.AllowConfiguration.AllowPublished;
                    response.AppendLine(GetStatusOutput(
                        discordGuild,
                        !guild.AllowConfiguration.AllowPublished,
                        guild.AllowConfiguration.AllowPublished,
                        "Published"));
                    break;
                case Constants.AllowOptions.StreamVodOption:
                    guild.AllowConfiguration.AllowStreamVod = !guild.AllowConfiguration.AllowStreamVod;
                    response.AppendLine(GetStatusOutput(
                        discordGuild,
                        !guild.AllowConfiguration.AllowStreamVod,
                        guild.AllowConfiguration.AllowStreamVod,
                        "Stream VODs"));
                    break;
                case Constants.AllowOptions.ThumbnailsOption:
                    guild.AllowConfiguration.AllowThumbnails = !guild.AllowConfiguration.AllowThumbnails;
                    response.AppendLine(GetStatusOutput(
                        discordGuild,
                        !guild.AllowConfiguration.AllowThumbnails,
                        guild.AllowConfiguration.AllowThumbnails,
                        "Thumbnails"));
                    break;
                case Constants.AllowOptions.FfaOption:
                    guild.AllowConfiguration.AllowFfa = !guild.AllowConfiguration.AllowFfa;
                    response.AppendLine(GetStatusOutput(
                        discordGuild,
                        !guild.AllowConfiguration.AllowFfa,
                        guild.AllowConfiguration.AllowFfa,
                        "FreeForAll"));
                    break;
                case Constants.AllowOptions.CrosspostOption:
                    guild.AllowConfiguration.AllowCrosspost = !guild.AllowConfiguration.AllowCrosspost;
                    response.AppendLine(GetStatusOutput(
                        discordGuild,
                        !guild.AllowConfiguration.AllowCrosspost,
                        guild.AllowConfiguration.AllowCrosspost,
                        "Crosspost"));
                    break;
                case Constants.AllowOptions.DeleteOfflineOption:
                    guild.GuildConfiguration.DeleteOffline = !guild.GuildConfiguration.DeleteOffline;
                    response.AppendLine(GetStatusOutput(
                        discordGuild,
                        !guild.GuildConfiguration.DeleteOffline,
                        guild.GuildConfiguration.DeleteOffline,
                        "Delete Offline Streams"));
                    break;
                case Constants.AllowOptions.TextAnnouncements:
                    guild.GuildConfiguration.TextAnnouncements = !guild.GuildConfiguration.TextAnnouncements;
                    response.AppendLine(GetStatusOutput(
                        discordGuild,
                        !guild.GuildConfiguration.TextAnnouncements,
                        guild.GuildConfiguration.TextAnnouncements,
                        "Plain Text Announcements"));
                    break;
                case Constants.AllowOptions.DiscordLiveAnnouncements:
                    guild.AllowConfiguration.AllowDiscordLive = !guild.AllowConfiguration.AllowDiscordLive;
                    response.AppendLine(GetStatusOutput(
                        discordGuild,
                        !guild.AllowConfiguration.AllowDiscordLive,
                        guild.AllowConfiguration.AllowDiscordLive,
                        "Discord Live Announcements"));
                    break;
            }
        }

        await _allowConfigurationAccessor.UpdateAsync(guild.AllowConfiguration);
        await _guildConfigurationAccessor.UpdateAsync(guild.Id, guild.GuildConfiguration);

        await message.FollowupAsync(response.ToString(), ephemeral: true);
        await message.DeleteOriginalResponseAsync();
    }

    private string GetStatusOutput(IGuild guild, bool oldValue, bool newValue, string setting)
    {
        var couchBotGuild = discordSocketClient.GetGuild(263688866978988032);
        var redcross = couchBotGuild.Emotes.FirstOrDefault(x => x.Id == 921804940698087504);
        var greentick = couchBotGuild.Emotes.FirstOrDefault(x => x.Id == 921804940383494176);

        var oldStatus = !oldValue ? redcross : greentick;
        var newStatus = newValue ? greentick : redcross;
        return $"Allow {setting} - {oldStatus} -> {newStatus}";
    }

    private async Task ProcessYouTubeCreatorDropdownAsync(
        SocketInteraction arg,
        SocketMessageComponent comp,
        YouTubeCreatorDropdownPayload creatorPayload)
    {
        var guildChannel = (IGuildChannel)arg.Channel;
        var discordUser = (IGuildUser)arg.User;
        var guild = await _guildAccessor.GetByIdAsync(guildChannel.GuildId.ToString());
        var youTubeChannel = await _youTubeAccessor.GetYouTubeChannelByIdAsync(comp.Data.Values.FirstOrDefault());
        var discordGuildChannel = discordSocketClient.GetChannel(creatorPayload.ChannelId);

        await _creatorEngine.ToggleCreatorAsync(
            new ChannelValidityResponse
            {
                ChannelId = comp.Data.Values.FirstOrDefault(),
                DisplayName = youTubeChannel?.Items?.FirstOrDefault()?.Snippet.Title, Valid = true
            },
            Platform.YouTube,
            guild,
            discordUser.Nickname ?? discordUser.Username,
            comp.Data.Values.FirstOrDefault(),
            (Shared.Enums.ChannelType)creatorPayload.ChannelType,
            creatorPayload.CustomMessage,
            arg,
            (IGuildChannel)discordGuildChannel
        );
    }
}