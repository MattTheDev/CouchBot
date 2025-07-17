using CB.Data.Entities;
using CB.Shared.Dtos;

namespace CB.Accessors.Contracts;

public interface IRoleConfigurationAccessor
{
    Task<List<RoleConfigurationDto>> GetAllAsync();

    Task<RoleConfigurationDto?> GetByIdAsync(string id);

    Task<RoleConfigurationDto> CreateAsync(RoleConfiguration entity);

    Task<RoleConfigurationDto?> UpdateAsync(RoleConfigurationDto entity);

    Task<bool> DeleteAsync(string id);
}