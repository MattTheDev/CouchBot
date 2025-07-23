using AutoMapper;
using AutoMapper.QueryableExtensions;
using CB.Accessors.Contracts;
using CB.Data;
using CB.Data.Entities;
using CB.Shared.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CB.Accessors.Implementations;

public class ChannelConfigurationAccessor(CbContext context, 
    IMapper mapper) 
    : IChannelConfigurationAccessor
{
    public Task<List<ChannelConfigurationDto>> GetAllAsync() =>  context
            .ChannelConfigurations
            .AsNoTracking()
            .ProjectTo<ChannelConfigurationDto>(mapper.ConfigurationProvider)
            .ToListAsync();

    public Task<ChannelConfigurationDto> GetByIdAsync(string id) => context.ChannelConfigurations
            .AsNoTracking()
            .Where(g => g.GuildId == id)
            .ProjectTo<ChannelConfigurationDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

    public async Task<ChannelConfigurationDto> CreateAsync(ChannelConfiguration entity)
    {
        context.ChannelConfigurations.Add(entity);
        await context
            .SaveChangesAsync()
            .ConfigureAwait(false);

        return mapper.Map<ChannelConfigurationDto>(entity);
    }

    public async Task<ChannelConfigurationDto> UpdateAsync(string id, 
        ChannelConfigurationDto dto)
    {
        var channelConfiguration = await context
            .ChannelConfigurations
            .FindAsync(id)
            .ConfigureAwait(false);

        if (channelConfiguration == null)
        {
            return null;
        }

        channelConfiguration.GreetingChannelId = dto.GreetingChannelId;
        channelConfiguration.GoodbyeChannelId = dto.GoodbyeChannelId;
        channelConfiguration.LiveChannelId = dto.LiveChannelId;
        channelConfiguration.DiscordLiveChannelId = dto.DiscordLiveChannelId;

        await context
            .SaveChangesAsync()
            .ConfigureAwait(false);

        return mapper.Map<ChannelConfigurationDto>(channelConfiguration);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var channelConfiguration = await context
            .ChannelConfigurations
            .FindAsync(id)
            .ConfigureAwait(false);

        if (channelConfiguration == null)
        {
            return false;
        }

        context.ChannelConfigurations.Remove(channelConfiguration);
        await context
            .SaveChangesAsync()
            .ConfigureAwait(false);

        return true;
    }
}