using AutoMapper;
using AutoMapper.QueryableExtensions;
using CB.Accessors.Contracts;
using CB.Data;
using CB.Data.Entities;
using CB.Shared.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CB.Accessors.Implementations;

public class ChannelAccessor(CbContext context, 
    IMapper mapper) 
    : IChannelAccessor
{
    public Task<List<ChannelDto>> GetAllAsync() =>  context
            .Channels
            .AsNoTracking()
            .ProjectTo<ChannelDto>(mapper.ConfigurationProvider)
            .ToListAsync();

    public Task<ChannelDto> GetByIdAsync(string id) => context.Channels
            .AsNoTracking()
            .Where(g => g.Id == id)
            .ProjectTo<ChannelDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

    public Task<ChannelConfigurationSummaryDto> GetChannelConfigurationSummaryByIdAsync(string id) => context.Channels
        .AsNoTracking()
        .Where(g => g.Id == id)
        .ProjectTo<ChannelConfigurationSummaryDto>(mapper.ConfigurationProvider)
        .FirstOrDefaultAsync();

    public async Task<ChannelConfigurationSummaryDto> CreateAsync(Channel entity)
    {
        entity.CreatedDate = DateTime.UtcNow;
        entity.ModifiedDate = DateTime.UtcNow;

        context.Channels.Add(entity);
        await context
            .SaveChangesAsync()
            .ConfigureAwait(false);

        return mapper.Map<ChannelConfigurationSummaryDto>(entity);
    }

    public async Task<ChannelDto> UpdateAsync(ChannelDto updated)
    {
        var channel = await context
            .Channels
            .FirstOrDefaultAsync(x => x.Id == updated.Id)
            .ConfigureAwait(false);

        if (channel == null)
        {
            return null;
        }

        channel.DisplayName = updated.DisplayName;
        channel.ModifiedDate = DateTime.UtcNow;

        await context
            .SaveChangesAsync()
            .ConfigureAwait(false);

        return mapper.Map<ChannelDto>(channel);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var channel = await context
            .Channels
            .FindAsync(id)
            .ConfigureAwait(false);

        if (channel == null)
        {
            return false;
        }

        context.Channels.Remove(channel);
        await context
            .SaveChangesAsync()
            .ConfigureAwait(false);

        return true;
    }
}