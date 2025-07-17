using CB.Data.Entities;
using CB.Shared.Dtos;

namespace CB.Accessors.Contracts;

public interface IDiscordLiveConfigurationAccessor
{
    Task<DiscordLiveConfigurationDto> CreateAsync(DiscordLiveConfiguration entity);
    Task<DiscordLiveConfigurationDto> GetByIdAsync(string guildId);
    Task<DiscordLiveConfigurationDto> UpdateAsync(DiscordLiveConfigurationDto updated);
}