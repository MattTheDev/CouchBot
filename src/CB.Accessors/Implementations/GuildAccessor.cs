using AutoMapper;
using AutoMapper.QueryableExtensions;
using CB.Accessors.Contracts;
using CB.Data;
using CB.Data.Entities;
using CB.Shared.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CB.Accessors.Implementations;

public class GuildAccessor(CbContext context, 
    IMapper mapper) 
    : IGuildAccessor
{
    public Task<List<GuildDto>> GetAllAsync() =>  context
            .Guilds
            .AsNoTracking()
            .ProjectTo<GuildDto>(mapper.ConfigurationProvider)
            .ToListAsync();

    public Task<GuildDto> GetByIdAsync(string id) => context.Guilds
            .AsNoTracking()
            .Where(g => g.Id == id)
            .ProjectTo<GuildDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

    public Task<GuildConfigurationSummaryDto> GetConfigurationSummaryByIdAsync(string id) => context.Guilds
        .AsNoTracking()
        .Where(g => g.Id == id)
        .ProjectTo<GuildConfigurationSummaryDto>(mapper.ConfigurationProvider)
        .FirstOrDefaultAsync();

    public async Task<GuildDto> CreateAsync(Guild entity)
    {
        entity.CreatedDate = DateTime.UtcNow;
        entity.ModifiedDate = DateTime.UtcNow;

        context.Guilds.Add(entity);
        await context
            .SaveChangesAsync()
            .ConfigureAwait(false);

        return mapper.Map<GuildDto>(entity);
    }

    public async Task<GuildDto> UpdateAsync(string id, 
        Guild updated)
    {
        var guild = await context
            .Guilds
            .FindAsync(id)
            .ConfigureAwait(false);

        if (guild == null)
        {
            return null;
        }

        guild.DisplayName = updated.DisplayName;
        guild.OwnerId = updated.OwnerId;
        guild.ModifiedDate = DateTime.UtcNow;

        await context
            .SaveChangesAsync()
            .ConfigureAwait(false);

        return mapper.Map<GuildDto>(guild);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var guild = await context
            .Guilds
            .FindAsync(id)
            .ConfigureAwait(false);

        if (guild == null)
        {
            return false;
        }

        context.Guilds.Remove(guild);
        await context
            .SaveChangesAsync()
            .ConfigureAwait(false);

        return true;
    }
}