using CB.Shared.Dtos;

namespace CB.Accessors.Contracts;

public interface IClipEmbedAccessor
{
    Task<ClipEmbedDto> GetByIdAsync(string guildId);

    Task<ClipEmbedDto> UpdateAsync(ClipEmbedDto entity);
}