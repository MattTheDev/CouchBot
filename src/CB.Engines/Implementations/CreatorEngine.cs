using CB.Accessors.Contracts;
using CB.Data.Entities;
using CB.Engines.Contracts;
using CB.Shared.Dtos;
using CB.Shared.Responses;
using Discord;
using Discord.WebSocket;
using ChannelType = CB.Shared.Enums.ChannelType;
using Platform = CB.Shared.Enums.Platform;

// ReSharper disable UnusedMember.Global

namespace CB.Engines.Implementations;

public class CreatorEngine(ICreatorAccessor creatorAccessor,
    IChannelAccessor channelAccessor,
    ICreatorChannelAccessor creatorChannelAccessor) : ICreatorEngine
{
    public async Task ToggleCreatorAsync(ChannelValidityResponse validChannel,
        Platform platform,
        GuildDto guild,
        string authorName,
        string creatorName,
        ChannelType channelType,
        string customMessage,
        // TODO This shouldnt be here.
        // Carrying this over for now .. just for ease ...
        // But all communication should be on the InteractionService layer. Just return 
        // messaging ...?
        SocketInteraction socketInteraction,
        IGuildChannel announcementChannel
        )
    {
        var creator =
            await creatorAccessor.GetByChannelIdAndPlatformAsync(validChannel.ChannelId, 
                platform);

        if (creator == null || creator.PlatformId != (int)platform)
        {
            creator = await creatorAccessor.CreateAsync(new Creator
            {
                ChannelId = validChannel.ChannelId,
                DisplayName = validChannel.DisplayName ?? "",
                PlatformId = (int)platform,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            });
        }

        var channel = guild.Channels.FirstOrDefault(c => c.Id.Equals(announcementChannel.Id.ToString())) ?? await channelAccessor.CreateAsync(new Channel
        {
            Id = announcementChannel.Id.ToString(),
            CreatedDate = DateTime.UtcNow,
            DisplayName = announcementChannel.Name,
            GuildId = guild.Id,
            ModifiedDate = DateTime.UtcNow
        });

        if (await RemoveCreator(
                creatorName,
                announcementChannel,
                channelType,
                customMessage,
                channel, 
                creator,
                platform,
                socketInteraction,
                authorName))
        {
            return;
        }

        if (await UpdateCreatorMessage(
                creatorName,
                channelType,
                customMessage, 
                channel,
                creator,
                socketInteraction,
                authorName))
        {
            return;
        }

        await creatorChannelAccessor.CreateAsync(new CreatorChannel
        {
            ChannelId = channel.Id,
            CreatorId = creator.Id,
            ChannelTypeId = (int)channelType,
            CustomMessage = customMessage
        });

        await socketInteraction.FollowupAsync(
            $"{Format.Sanitize(creatorName)} will now announce in {announcementChannel.Name}.", 
            ephemeral: true);
    }

    private async Task<bool> UpdateCreatorMessage(
        string channelName,
        ChannelType channelType,
        string customMessage,
        ChannelDto channel,
        CreatorDto creator,
        SocketInteraction socketInteraction,
        string authorName)
    {
        if (channel.CreatorChannels.Select(x => x.Creator).Any(x => x.Id == creator.Id) && !string.IsNullOrEmpty(customMessage))
        {
            var creatorChannel = await creatorChannelAccessor.GetAsync(creator.Id, channel.Id, (int)channelType);
            creatorChannel.CustomMessage = customMessage;
            await creatorChannelAccessor.UpdateAsync(channel.Id, creatorChannel);

            await socketInteraction.FollowupAsync($"{Format.Sanitize(channelName)}'s custom message has been updated.", ephemeral: true);

            return true;
        }

        return false;
    }

    private async Task<bool> RemoveCreator(
        string channelName,
        IGuildChannel guildChannel,
        ChannelType channelType,
        string customMessage,
        ChannelDto channel,
        CreatorDto creator,
        Platform platform,
        SocketInteraction socketInteraction,
        string authorName)
    {
        if (channel.CreatorChannels.Select(x => x.Creator).Any(x => x.Id == creator.Id) &&
            string.IsNullOrEmpty(customMessage))
        {
            // TODO Dont retrieve here. Handle in Delete call.
            var creatorChannel =
                await creatorChannelAccessor.GetAsync(creator.Id, channel.Id, (int)channelType);

            if (creatorChannel == null)
            {
                return false;
            }

            await creatorChannelAccessor.DeleteAsync(creator.Id,
                channel.Id,
                (int)channelType);

            await socketInteraction.FollowupAsync(
                $"{Format.Sanitize(channelName)} will no longer announce in {guildChannel.Name}.", ephemeral: true);

            return true;
        }

        return false;
    }
}