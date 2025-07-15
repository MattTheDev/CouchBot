using System.Xml;
using CB.Accessors.Contracts;
using CB.Shared.Models.Twitch;
using CB.Shared.Utilities;
using CouchBot.Domain.Models.Twitch;

namespace CB.Accessors.Implementations;

public class TwitchAccessor : ITwitchAccessor
{
    private const string BaseApiUrl = "https://api.twitch.tv/helix/";
    private List<(string name, string value)> _twitchHeaders = [];
    private DateTime TokenRetrieved { get; }

    public TwitchAccessor()
    {
        GetTwitchClientCredentials().GetAwaiter().GetResult();
        TokenRetrieved = DateTime.UtcNow;
    }

    public async Task<TwitchStreamResponse> GetStreamsAsync(string twitchIdList)
    {
        await GetTwitchClientCredentials()
            .ConfigureAwait(false);

        return await ApiUtilities
            .ApiHelper<TwitchStreamResponse>($"{BaseApiUrl}streams?{twitchIdList}&first=100", 
                _twitchHeaders)
            .ConfigureAwait(false);
    }

    public async Task<TwitchUserResponse> GetUsersAsync(string twitchIdList)
    {
        await GetTwitchClientCredentials()
            .ConfigureAwait(false);

        return await ApiUtilities
            .ApiHelper<TwitchUserResponse>($"{BaseApiUrl}users?{twitchIdList}", 
                _twitchHeaders)
            .ConfigureAwait(false);
    }

    public async Task<TwitchTeam> GetTwitchTeamByNameAsync(string name)
    {
        await GetTwitchClientCredentials()
            .ConfigureAwait(false);

        return await ApiUtilities
            .ApiHelper<TwitchTeam>($"{BaseApiUrl}teams?name={name}", 
                _twitchHeaders)
            .ConfigureAwait(false);
    }

    public async Task<TwitchStreamResponse> GetStreamsByGameNameAsync(string gameId)
    {
        await GetTwitchClientCredentials()
            .ConfigureAwait(false);

        return await ApiUtilities
            .ApiHelper<TwitchStreamResponse>($"{BaseApiUrl}streams?game_id={gameId}", 
                _twitchHeaders)
            .ConfigureAwait(false);
    }

    public async Task<TwitchGameSearchResponse> SearchForGameByNameAsync(string gameName)
    {
        await GetTwitchClientCredentials()
            .ConfigureAwait(false);

        return await ApiUtilities
            .ApiHelper<TwitchGameSearchResponse>($"{BaseApiUrl}search/categories?query={gameName}", 
                _twitchHeaders)
            .ConfigureAwait(false);
    }

    public async Task<TwitchClipsResponse> GetClipsAsync(string channelId)
    {
        await GetTwitchClientCredentials()
            .ConfigureAwait(false);

        var start = XmlConvert.ToString(DateTime.UtcNow.AddMinutes(-10), XmlDateTimeSerializationMode.Utc);
        var end = XmlConvert.ToString(DateTime.UtcNow, XmlDateTimeSerializationMode.Utc);

        return await ApiUtilities
            .ApiHelper<TwitchClipsResponse>($"{BaseApiUrl}clips?" +
                                                                 $"broadcaster_id={channelId}&" +
                                                                 $"started_at={start}&" +
                                                                 $"ended_at={end}", 
                _twitchHeaders)
            .ConfigureAwait(false);
    }

    private async Task GetTwitchClientCredentials()
    {
        if ((DateTime.UtcNow - TokenRetrieved).TotalMinutes > 90)
        {
            var twitchClientId = Environment.GetEnvironmentVariable("TwitchClientId");
            var twitchClientSecret = Environment.GetEnvironmentVariable("TwitchClientSecret");

            if (string.IsNullOrEmpty(twitchClientId) ||
                string.IsNullOrEmpty(twitchClientSecret))
            {
                throw new InvalidOperationException("TwitchClientId or TwitchClientSecret Configuration Missing.");
            }

            var clientCredentials = await ApiUtilities.PostApiHelper<TwitchClientCredentials>(
                "https://id.twitch.tv/oauth2/token?" +
                $"client_id={twitchClientId}&" +
                $"client_secret={twitchClientSecret}&" +
                "grant_type=client_credentials", 
                _twitchHeaders)
                .ConfigureAwait(false);

            if (clientCredentials == null)
            {
                return;
            }

            _twitchHeaders.Clear();
            _twitchHeaders =
            [
                ("Client-Id", twitchClientId),
                ("Authorization", $"{clientCredentials.TokenType.FirstLetterToUpper()} {clientCredentials.AccessToken}")
            ];
        }
    }
}