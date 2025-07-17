using AutoMapper;
using AutoMapper.QueryableExtensions;
using CB.Accessors.Contracts;
using CB.Data;
using CB.Data.Entities;
using CB.Shared.Dtos;
using CB.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace CB.Accessors.Implementations;

public class CreatorAccessor(CbContext context, 
    IMapper mapper) 
    : ICreatorAccessor
{
    public Task<List<CreatorDto>> GetAllAsync() =>  context
            .Creators
            .AsNoTracking()
            .ProjectTo<CreatorDto>(mapper.ConfigurationProvider)
            .ToListAsync();

    public Task<CreatorDto?> GetByIdAsync(long id) => context.Creators
            .AsNoTracking()
            .Where(g => g.Id == id)
            .ProjectTo<CreatorDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

    public Task<CreatorDto?> GetByChannelIdAndPlatformAsync(string channelId, Platform platform) => context.Creators
        .AsNoTracking()
        .Where(g => g.ChannelId == channelId &&
                    g.PlatformId == (int)platform)
        .ProjectTo<CreatorDto>(mapper.ConfigurationProvider)
        .FirstOrDefaultAsync();

    public async Task<CreatorDto> CreateAsync(Creator entity)
    {
        entity.CreatedDate = DateTime.UtcNow;
        entity.ModifiedDate = DateTime.UtcNow;

        context.Creators.Add(entity);
        await context
            .SaveChangesAsync()
            .ConfigureAwait(false);

        return mapper.Map<CreatorDto>(entity);
    }

    public async Task<CreatorDto?> UpdateAsync(string id, 
        Creator updated)
    {
        var creator = await context
            .Creators
            .FindAsync(id)
            .ConfigureAwait(false);

        if (creator == null)
        {
            return null;
        }

        creator.DisplayName = updated.DisplayName;
        creator.ModifiedDate = DateTime.UtcNow;

        await context
            .SaveChangesAsync()
            .ConfigureAwait(false);

        return mapper.Map<CreatorDto>(creator);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var creator = await context
            .Creators
            .FindAsync(id)
            .ConfigureAwait(false);

        if (creator == null)
        {
            return false;
        }

        context.Creators.Remove(creator);
        await context
            .SaveChangesAsync()
            .ConfigureAwait(false);

        return true;
    }
}