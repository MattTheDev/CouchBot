using CB.Shared.Dtos;

namespace CB.Accessors.Contracts;

public interface IVodEmbedAccessor
{
    Task<VodEmbedDto> GetByIdAsync(string guildId);

    Task<VodEmbedDto> UpdateAsync(VodEmbedDto entity);
}