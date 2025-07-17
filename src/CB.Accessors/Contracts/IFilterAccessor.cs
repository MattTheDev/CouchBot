using CB.Data.Entities;
using CB.Shared.Dtos;

namespace CB.Accessors.Contracts;

public interface IFilterAccessor
{
    Task<FilterDto> CreateAsync(Filter entity);

    Task<List<FilterDto>> GetAllAsync();

    Task<List<FilterDto>> GetAllAsync(string guildId);

    Task<bool> DeleteAsync(FilterDto filter);
}