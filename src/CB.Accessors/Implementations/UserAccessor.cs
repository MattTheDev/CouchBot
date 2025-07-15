using AutoMapper;
using AutoMapper.QueryableExtensions;
using CB.Accessors.Contracts;
using CB.Data;
using CB.Data.Entities;
using CB.Shared.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CB.Accessors.Implementations;

public class UserAccessor(CbContext context, 
    IMapper mapper) 
    : IUserAccessor
{
    public Task<List<UserDto>> GetAllAsync() =>  context
            .Users
            .AsNoTracking()
            .ProjectTo<UserDto>(mapper.ConfigurationProvider)
            .ToListAsync();

    public Task<UserDto?> GetByIdAsync(string id) => context.Users
            .AsNoTracking()
            .Where(g => g.Id == id)
            .ProjectTo<UserDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

    public async Task<UserDto> CreateAsync(User entity)
    {
        entity.CreatedDate = DateTime.UtcNow;
        entity.ModifiedDate = DateTime.UtcNow;

        context.Users.Add(entity);
        await context
            .SaveChangesAsync()
            .ConfigureAwait(false);

        return mapper.Map<UserDto>(entity);
    }

    public async Task<UserDto?> UpdateAsync(string id, 
        User updated)
    {
        var user = await context
            .Users
            .FindAsync(id)
            .ConfigureAwait(false);

        if (user == null)
        {
            return null;
        }

        user.DisplayName = updated.DisplayName;
        user.Id = updated.Id;
        user.ModifiedDate = DateTime.UtcNow;

        await context
            .SaveChangesAsync()
            .ConfigureAwait(false);

        return mapper.Map<UserDto>(user);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var user = await context.Users.FindAsync(id);
        if (user == null)
        {
            return false;
        }

        context.Users.Remove(user);
        await context
            .SaveChangesAsync()
            .ConfigureAwait(false);

        return true;
    }
}