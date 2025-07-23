using AutoMapper;
using AutoMapper.QueryableExtensions;
using CB.Accessors.Contracts;
using CB.Data;
using CB.Data.Entities;
using CB.Shared.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CB.Accessors.Implementations;

public class AllowConfigurationAccessor(CbContext context, 
    IMapper mapper) 
    : IAllowConfigurationAccessor
{
    public Task<List<AllowConfigurationDto>> GetAllAsync() =>  context
            .AllowConfigurations
            .AsNoTracking()
            .ProjectTo<AllowConfigurationDto>(mapper.ConfigurationProvider)
            .ToListAsync();

    public Task<AllowConfigurationDto> GetByIdAsync(string id) => context.AllowConfigurations
            .AsNoTracking()
            .Where(g => g.GuildId == id)
            .ProjectTo<AllowConfigurationDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

    public async Task<AllowConfigurationDto> CreateAsync(AllowConfiguration entity)
    {
        context.AllowConfigurations.Add(entity);
        await context
            .SaveChangesAsync()
            .ConfigureAwait(false);

        return mapper.Map<AllowConfigurationDto>(entity);
    }

    public async Task<AllowConfigurationDto> UpdateAsync(AllowConfigurationDto updated)
    {
        var allowConfiguration = await context
            .AllowConfigurations
            .FirstOrDefaultAsync( x => x.GuildId == updated.GuildId)
            .ConfigureAwait(false);

        if (allowConfiguration == null)
        {
            return null;
        }

        allowConfiguration.AllowCrosspost = updated.AllowCrosspost;
        allowConfiguration.AllowDiscordLive = updated.AllowDiscordLive;
        allowConfiguration.AllowFfa = updated.AllowFfa;
        allowConfiguration.AllowGoodbyes = updated.AllowGoodbyes;
        allowConfiguration.AllowGreetings = updated.AllowGreetings;
        allowConfiguration.AllowLive = updated.AllowLive;
        allowConfiguration.AllowLiveDiscovery = updated.AllowLiveDiscovery;
        allowConfiguration.AllowPublished = updated.AllowPublished;
        allowConfiguration.AllowStreamVod = updated.AllowStreamVod;
        allowConfiguration.AllowThumbnails = updated.AllowThumbnails;

        await context
            .SaveChangesAsync()
            .ConfigureAwait(false);

        return mapper.Map<AllowConfigurationDto>(allowConfiguration);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var allowConfiguration = await context
            .AllowConfigurations
            .FindAsync(id)
            .ConfigureAwait(false);

        if (allowConfiguration == null)
        {
            return false;
        }

        context.AllowConfigurations.Remove(allowConfiguration);
        await context
            .SaveChangesAsync()
            .ConfigureAwait(false);

        return true;
    }
}