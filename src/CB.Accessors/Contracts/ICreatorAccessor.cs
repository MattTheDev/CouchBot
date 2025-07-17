using CB.Data.Entities;
using CB.Shared.Dtos;
using CB.Shared.Enums;
using Platform = CB.Shared.Enums.Platform;

namespace CB.Accessors.Contracts;

public interface ICreatorAccessor
{
    Task<List<CreatorDto>> GetAllAsync();

    Task<CreatorDto?> GetByIdAsync(long id);

    Task<CreatorDto?> GetByChannelIdAndPlatformAsync(string channelId, Platform platform);

    Task<CreatorDto> CreateAsync(Creator entity);

    Task<CreatorDto?> UpdateAsync(string id, 
        Creator entity);

    Task<bool> DeleteAsync(string id);
}