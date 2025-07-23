using AutoMapper;
using AutoMapper.QueryableExtensions;
using CB.Accessors.Contracts;
using CB.Data;
using CB.Data.Entities;
using CB.Shared.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CB.Accessors.Implementations;

public class GuildConfigurationAccessor(CbContext context, 
    IMapper mapper) 
    : IGuildConfigurationAccessor
{
    public Task<List<GuildConfigurationDto>> GetAllAsync() =>  context
            .GuildConfigurations
            .AsNoTracking()
            .ProjectTo<GuildConfigurationDto>(mapper.ConfigurationProvider)
            .ToListAsync();

    public Task<GuildConfigurationDto> GetByIdAsync(string id) => context.GuildConfigurations
            .AsNoTracking()
            .Where(g => g.GuildId == id)
            .ProjectTo<GuildConfigurationDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

    public async Task<GuildConfigurationDto> CreateAsync(GuildConfiguration entity)
    {
        context.GuildConfigurations.Add(entity);
        await context
            .SaveChangesAsync()
            .ConfigureAwait(false);

        return mapper.Map<GuildConfigurationDto>(entity);
    }

    public async Task<GuildConfigurationDto> UpdateAsync(string id, 
        GuildConfigurationDto updated)
    {
        var guildConfiguration = await context
            .GuildConfigurations
            .FindAsync(id)
            .ConfigureAwait(false);

        if (guildConfiguration == null)
        {
            return null;
        }

        guildConfiguration.DeleteOffline = updated.DeleteOffline;
        guildConfiguration.TextAnnouncements = updated.TextAnnouncements;

        await context
            .SaveChangesAsync()
            .ConfigureAwait(false);

        return mapper.Map<GuildConfigurationDto>(guildConfiguration);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var guildConfiguration = await context
            .GuildConfigurations
            .FindAsync(id)
            .ConfigureAwait(false);

        if (guildConfiguration == null)
        {
            return false;
        }

        context.GuildConfigurations.Remove(guildConfiguration);
        await context
            .SaveChangesAsync()
            .ConfigureAwait(false);

        return true;
    }
}