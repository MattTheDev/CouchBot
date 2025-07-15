using CB.Shared.Models.Picarto;

namespace CB.Accessors.Contracts;

public interface IPicartoAccessor
{
    Task<PicartoUser> RetrieveAsync(string accessToken);

    Task<PicartoChannel> GetChannelByNameAsync(string name);
}