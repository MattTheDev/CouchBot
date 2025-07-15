using CB.Accessors.Contracts;
using CB.Shared.Models.Piczel;
using CB.Shared.Utilities;

namespace CB.Accessors.Implementations;

public class PiczelAccessor : IPiczelAccessor
{
    public async Task<PiczelChannel> GetChannelByNameAsync(string channelName)
    {
        return await ApiUtilities.ApiHelper<PiczelChannel>($"https://piczel.tv/api/streams/{channelName}");
    }
}