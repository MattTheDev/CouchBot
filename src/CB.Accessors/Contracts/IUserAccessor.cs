using CB.Data.Entities;
using CB.Shared.Dtos;

namespace CB.Accessors.Contracts;

public interface IUserAccessor
{
    Task<List<UserDto>> GetAllAsync();
    Task<UserDto?> GetByIdAsync(string id);
    Task<UserDto> CreateAsync(User entity);
    Task<UserDto?> UpdateAsync(string id, User entity);
    Task<bool> DeleteAsync(string id);
}