using CB.Accessors.Contracts;
using CB.Shared.Models.Picarto;
using CB.Shared.Utilities;

namespace CB.Accessors.Implementations;

public class PicartoAccessor : IPicartoAccessor
{
    public async Task<PicartoUser> RetrieveAsync(string accessToken)
    {
        return await ApiUtilities
            .ApiHelper<PicartoUser>("https://api.picarto.tv/api/v1/user", 
            [("Authorization", $"Bearer {accessToken}"),
                ("Content-Type", "application/json"),
                ("User-Agent", "PostmanRuntime/7.26.8")])
            .ConfigureAwait(false);
    }

    public async Task<PicartoChannel> GetChannelByNameAsync(string name)
    {
        return await ApiUtilities.ApiHelper<PicartoChannel>($"https://ptvintern.picarto.tv/api/channel/detail/{name}", 
            [("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:88.0) Gecko/20100101 Firefox/88.0 - CouchBot - me@mattthedev.codes"),
            ("Accept", "*/*")])
        .ConfigureAwait(false);
    }
}