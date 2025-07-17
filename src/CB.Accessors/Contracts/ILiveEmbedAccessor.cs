using CB.Shared.Dtos;

namespace CB.Accessors.Contracts;

public interface ILiveEmbedAccessor
{
    Task<LiveEmbedDto> GetByIdAsync(string guildId);

    Task<LiveEmbedDto> UpdateAsync(LiveEmbedDto entity);
}