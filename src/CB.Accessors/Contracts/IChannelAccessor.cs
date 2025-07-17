using CB.Data.Entities;
using CB.Shared.Dtos;

namespace CB.Accessors.Contracts;

public interface IChannelAccessor
{
    Task<List<ChannelDto>> GetAllAsync();

    Task<ChannelDto?> GetByIdAsync(string id);

    Task<ChannelDto> CreateAsync(Channel entity);

    Task<ChannelDto?> UpdateAsync(ChannelDto entity);

    Task<bool> DeleteAsync(string id);
}