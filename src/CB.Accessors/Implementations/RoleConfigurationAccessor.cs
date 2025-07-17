using AutoMapper;
using AutoMapper.QueryableExtensions;
using CB.Accessors.Contracts;
using CB.Data;
using CB.Data.Entities;
using CB.Shared.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CB.Accessors.Implementations;

public class RoleConfigurationAccessor(CbContext context, 
    IMapper mapper) 
    : IRoleConfigurationAccessor
{
    public Task<List<RoleConfigurationDto>> GetAllAsync() =>  context
            .RoleConfigurations
            .AsNoTracking()
            .ProjectTo<RoleConfigurationDto>(mapper.ConfigurationProvider)
            .ToListAsync();

    public Task<RoleConfigurationDto?> GetByIdAsync(string id) => context.RoleConfigurations
            .AsNoTracking()
            .Where(g => g.GuildId == id)
            .ProjectTo<RoleConfigurationDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

    public async Task<RoleConfigurationDto> CreateAsync(RoleConfiguration entity)
    {
        context.RoleConfigurations.Add(entity);
        await context
            .SaveChangesAsync()
            .ConfigureAwait(false);

        return mapper.Map<RoleConfigurationDto>(entity);
    }

    public async Task<RoleConfigurationDto?> UpdateAsync(RoleConfigurationDto updated)
    {
        var roleConfiguration = await context
            .RoleConfigurations
            .FirstOrDefaultAsync(x => x.GuildId == updated.GuildId)
            .ConfigureAwait(false);

        if (roleConfiguration == null)
        {
            return null;
        }

        roleConfiguration.DiscoveryRoleId = updated.DiscoveryRoleId;
        roleConfiguration.JoinRoleId = updated.JoinRoleId;
        roleConfiguration.LiveDiscoveryRoleId = updated.LiveDiscoveryRoleId;

        await context
            .SaveChangesAsync()
            .ConfigureAwait(false);

        return mapper.Map<RoleConfigurationDto>(roleConfiguration);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var roleConfiguration = await context
            .RoleConfigurations
            .FindAsync(id)
            .ConfigureAwait(false);

        if (roleConfiguration == null)
        {
            return false;
        }

        context.RoleConfigurations.Remove(roleConfiguration);
        await context
            .SaveChangesAsync()
            .ConfigureAwait(false);

        return true;
    }
}