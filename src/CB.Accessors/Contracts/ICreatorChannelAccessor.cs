using CB.Data.Entities;
using CB.Shared.Dtos;

// ReSharper disable UnusedMember.Global

namespace CB.Accessors.Contracts;

public interface ICreatorChannelAccessor
{
    Task<List<CreatorChannelDto>> GetAllAsync();

    Task<CreatorChannelDto?> GetAsync(long creatorId, 
        string channelId, 
        int channelTypeId);

    Task<CreatorChannelDto> CreateAsync(CreatorChannel entity);

    Task<CreatorChannelDto?> UpdateAsync(string id, 
        CreatorChannelDto entity);

    Task<bool> DeleteAsync(long creatorId,
        string channelId,
        int channelTypeId);
}