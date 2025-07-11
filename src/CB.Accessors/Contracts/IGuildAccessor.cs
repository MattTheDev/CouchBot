using CB.Data.Entities;
using CB.Shared.Dtos;

namespace CB.Accessors.Contracts;

public interface IGuildAccessor
{
    Task<List<GuildDto>> GetAllAsync();
    Task<GuildDto?> GetByIdAsync(string id);
    Task<GuildDto> CreateAsync(Guild entity);
    Task<GuildDto?> UpdateAsync(string id, Guild entity);
    Task<bool> DeleteAsync(string id);
}