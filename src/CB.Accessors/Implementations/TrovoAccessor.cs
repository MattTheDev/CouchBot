using CB.Accessors.Contracts;
using CB.Shared.Models.Trovo;
using CB.Shared.Utilities;
using Newtonsoft.Json;

namespace CB.Accessors.Implementations;

public class TrovoAccessor : ITrovoAccessor
{
    private readonly string _trovoClientId;

    public TrovoAccessor()
    {
        _trovoClientId = Environment.GetEnvironmentVariable("TrovoClientId");
        if (string.IsNullOrEmpty(_trovoClientId))
        {
            throw new InvalidOperationException("TrovoClientId Configuration Missing.");
        }
    }

    public async Task<TrovoUser> GetUserByNameAsync(string name)
    {
        var query = new TrovoUserQuery
        {
            User = [name]
        };

        return await ApiUtilities
            .PostApiHelper<TrovoUser>(
            "https://open-api.trovo.live/openplatform/getusers",
            [("Client-Id", _trovoClientId)],
            JsonConvert.SerializeObject(query))
            .ConfigureAwait(false);
    }

    public async Task<TrovoChannel> GetChannelByIdAsync(int id)
    {
        var query = new TrovoQuery
        {
            ChannelId = id
        };

        return await ApiUtilities
            .PostApiHelper<TrovoChannel>("https://open-api.trovo.live/openplatform/channels/id",
                [("Client-Id", _trovoClientId)],
            JsonConvert.SerializeObject(query))
            .ConfigureAwait(false);
    }
}

public class TrovoQuery
{
    [JsonProperty("channel_id")]
    public int ChannelId { get; set; }
}

public class TrovoUserQuery
{
    [JsonProperty("user")]
    public List<string> User { get; set; }
}