using CB.Data.Entities;
using CB.Shared.Dtos;

namespace CB.Accessors.Contracts;

public interface IChannelConfigurationAccessor
{
    Task<List<ChannelConfigurationDto>> GetAllAsync();

    Task<ChannelConfigurationDto> GetByIdAsync(string id);

    Task<ChannelConfigurationDto> CreateAsync(ChannelConfiguration entity);

    Task<ChannelConfigurationDto> UpdateAsync(string id, 
        ChannelConfigurationDto dto);

    Task<bool> DeleteAsync(string id);
}