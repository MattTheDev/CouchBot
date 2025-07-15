using CB.Shared.Models.Twitch;
using CouchBot.Domain.Models.Twitch;

namespace CB.Accessors.Contracts;

public interface ITwitchAccessor
{
    Task<TwitchStreamResponse> GetStreamsAsync(string twitchIdList);

    Task<TwitchUserResponse> GetUsersAsync(string twitchIdList);

    Task<TwitchTeam> GetTwitchTeamByNameAsync(string name);

    Task<TwitchStreamResponse> GetStreamsByGameNameAsync(string gameName);

    Task<TwitchGameSearchResponse> SearchForGameByNameAsync(string gameName);

    Task<TwitchClipsResponse> GetClipsAsync(string channelId);
}