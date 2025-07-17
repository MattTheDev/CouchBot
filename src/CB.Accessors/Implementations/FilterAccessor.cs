using AutoMapper;
using AutoMapper.QueryableExtensions;
using CB.Accessors.Contracts;
using CB.Data;
using CB.Data.Entities;
using CB.Shared.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CB.Accessors.Implementations;

public class FilterAccessor(CbContext context,
    IMapper mapper) : IFilterAccessor
{
    public async Task<FilterDto> CreateAsync(Filter entity)
    {
        context.Filters.Add(entity);
        await context
            .SaveChangesAsync()
            .ConfigureAwait(false);

        return mapper.Map<FilterDto>(entity);
    }

    public Task<List<FilterDto>> GetAllAsync() =>
        context
            .Filters
            .AsNoTracking()
            .ProjectTo<FilterDto>(mapper.ConfigurationProvider)
            .ToListAsync();

    public Task<List<FilterDto>> GetAllAsync(string guildId) =>
        context
            .Filters
            .AsNoTracking()
            .Where(x => x.GuildId == guildId)
            .ProjectTo<FilterDto>(mapper.ConfigurationProvider)
            .ToListAsync();

    public async Task<bool> DeleteAsync(FilterDto filter)
    {
        var entity = await context
            .Filters
            .FirstOrDefaultAsync(x => x.Id == filter.Id)
            .ConfigureAwait(false);

        if (entity == null)
        {
            return false;
        }

        context.Filters.Remove(entity);
        await context
            .SaveChangesAsync()
            .ConfigureAwait(false);

        return true;
    }
}