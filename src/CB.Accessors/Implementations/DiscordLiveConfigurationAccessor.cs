using AutoMapper;
using AutoMapper.QueryableExtensions;
using CB.Accessors.Contracts;
using CB.Data;
using CB.Data.Entities;
using CB.Shared.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CB.Accessors.Implementations;

public class DiscordLiveConfigurationAccessor(CbContext context,
    IMapper mapper) 
    : IDiscordLiveConfigurationAccessor
{
    public async Task<DiscordLiveConfigurationDto> CreateAsync(DiscordLiveConfiguration entity)
    {
        context.DiscordLiveConfigurations.Add(entity);
        await context
            .SaveChangesAsync()
            .ConfigureAwait(false);

        return mapper.Map<DiscordLiveConfigurationDto>(entity);
    }

    public Task<DiscordLiveConfigurationDto> GetByIdAsync(string guildId) => 
        context.DiscordLiveConfigurations
        .AsNoTracking()
        .Where(g => g.GuildId == guildId)
        .ProjectTo<DiscordLiveConfigurationDto>(mapper.ConfigurationProvider)
        .FirstOrDefaultAsync();


    public async Task<DiscordLiveConfigurationDto> UpdateAsync(DiscordLiveConfigurationDto updated)
    {
        var entity = await context
            .DiscordLiveConfigurations
            .FirstOrDefaultAsync(x => x.GuildId == updated.GuildId)
            .ConfigureAwait(false);

        if (entity == null)
        {
            return null;
        }

        entity.Description = updated.Description;
        entity.Footer = updated.Footer;
        entity.Header = updated.Header;
        entity.MentionRoleId = updated.MentionRoleId;
        entity.Message = updated.Message;

        await context
            .SaveChangesAsync()
            .ConfigureAwait(false);

        return mapper.Map<DiscordLiveConfigurationDto>(entity);
    }
}