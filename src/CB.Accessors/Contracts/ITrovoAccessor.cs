using CB.Shared.Models.Trovo;

namespace CB.Accessors.Contracts;

public interface ITrovoAccessor
{
    Task<TrovoUser> GetUserByNameAsync(string name);

    Task<TrovoChannel> GetChannelByIdAsync(int id);
}