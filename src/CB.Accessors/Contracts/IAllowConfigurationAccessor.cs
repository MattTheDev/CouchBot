using CB.Data.Entities;
using CB.Shared.Dtos;

namespace CB.Accessors.Contracts;

public interface IAllowConfigurationAccessor
{
    Task<List<AllowConfigurationDto>> GetAllAsync();

    Task<AllowConfigurationDto?> GetByIdAsync(string id);

    Task<AllowConfigurationDto> CreateAsync(AllowConfiguration entity);

    Task<AllowConfigurationDto?> UpdateAsync(string id, 
        AllowConfigurationDto entity);

    Task<bool> DeleteAsync(string id);
}