using CB.Shared.Dtos;
using CB.Shared.Enums;
using CB.Shared.Responses;
using Discord;
using Discord.WebSocket;
using ChannelType = CB.Shared.Enums.ChannelType;

namespace CB.Engines.Contracts;

public interface ICreatorEngine
{
    Task ToggleCreatorAsync(ChannelValidityResponse validChannel,
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
    );
}