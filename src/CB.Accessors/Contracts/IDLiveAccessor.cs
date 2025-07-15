using CB.Shared.Models.DLive;

namespace CB.Accessors.Contracts;

public interface IDLiveAccessor
{
    Task<DLiveUser> GetUserByDisplayNameAsync(string displayName);

    Task<DLiveUser> GetUserByUsernameAsync(string username);
}