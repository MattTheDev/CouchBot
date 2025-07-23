using AutoMapper;
using AutoMapper.QueryableExtensions;
using CB.Accessors.Contracts;
using CB.Data;
using CB.Data.Entities;
using CB.Shared.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CB.Accessors.Implementations;

public class CreatorChannelAccessor(CbContext context, 
    IMapper mapper) 
    : ICreatorChannelAccessor
{
    public Task<List<CreatorChannelDto>> GetAllAsync() =>  context
            .CreatorChannels
            .AsNoTracking()
            .ProjectTo<CreatorChannelDto>(mapper.ConfigurationProvider)
            .ToListAsync();

    public Task<CreatorChannelDto> GetAsync(long creatorId,
        string channelId,
        int channelTypeId) => context.CreatorChannels
            .AsNoTracking()
            .Where(g => g.CreatorId == creatorId &&
                        g.ChannelId == channelId &&
                        g.ChannelTypeId == channelTypeId)
            .ProjectTo<CreatorChannelDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

    public async Task<CreatorChannelDto> CreateAsync(CreatorChannel entity)
    {
        context.CreatorChannels.Add(entity);
        await context
            .SaveChangesAsync()
            .ConfigureAwait(false);

        return mapper.Map<CreatorChannelDto>(entity);
    }

    public async Task<CreatorChannelDto> UpdateAsync(string id,
        CreatorChannelDto updated)
    {
        var creatorChannel = await context
            .CreatorChannels
            .FindAsync(id)
            .ConfigureAwait(false);

        if (creatorChannel == null)
        {
            return null;
        }

        creatorChannel.CustomMessage = updated.CustomMessage;
        creatorChannel.ChannelTypeId = updated.ChannelTypeId;

        await context
            .SaveChangesAsync()
            .ConfigureAwait(false);

        return mapper.Map<CreatorChannelDto>(creatorChannel);
    }

    public async Task<bool> DeleteAsync(long creatorId,
        string channelId,
        int channelTypeId)
    {
        var creatorChannel = await context
            .CreatorChannels
            .FirstOrDefaultAsync(x => x.CreatorId == creatorId &&
                x.ChannelId == channelId &&
                x.ChannelTypeId == channelTypeId)
            .ConfigureAwait(false);

        if (creatorChannel == null)
        {
            return false;
        }

        context.CreatorChannels.Remove(creatorChannel);
        await context
            .SaveChangesAsync()
            .ConfigureAwait(false);

        return true;
    }
}