using CB.Data.Entities;
using CB.Shared.Dtos;

namespace CB.Accessors.Contracts;

public interface IGuildConfigurationAccessor
{
    Task<List<GuildConfigurationDto>> GetAllAsync();

    Task<GuildConfigurationDto> GetByIdAsync(string id);

    Task<GuildConfigurationDto> CreateAsync(GuildConfiguration entity);

    Task<GuildConfigurationDto> UpdateAsync(string id, 
        GuildConfigurationDto entity);

    Task<bool> DeleteAsync(string id);
}