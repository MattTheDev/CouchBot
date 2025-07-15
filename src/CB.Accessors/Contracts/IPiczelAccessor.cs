using CB.Shared.Models.Piczel;

namespace CB.Accessors.Contracts;

public interface IPiczelAccessor
{
    Task<PiczelChannel> GetChannelByNameAsync(string name);
}