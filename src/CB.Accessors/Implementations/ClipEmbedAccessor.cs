using AutoMapper;
using CB.Accessors.Contracts;
using CB.Data;
using CB.Shared.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CB.Accessors.Implementations;

public class ClipEmbedAccessor(CbContext context, 
    IMapper mapper) 
    : IClipEmbedAccessor
{
    public async Task<ClipEmbedDto> GetByIdAsync(string guildId)
    {
        var entity = await context
            .ClipEmbeds
            .FirstOrDefaultAsync(x => x.GuildId == guildId);

        if (entity == null)
        {
            entity = new()
            {
                GuildId = guildId,
                Header = "New Clip @ %CHANNEL%",
                Description = "%DESCRIPTION%",
                Footer = "Powered by 100% Couch • Created by %CHANNEL%",
                WatchButton = "Watch this Clip!",
                MoreButton = "And More!",
            };

            await context
                .ClipEmbeds
                .AddAsync(entity);
            await context
                .SaveChangesAsync();
        }

        return mapper.Map<ClipEmbedDto>(entity);
    }

    public async Task<ClipEmbedDto> UpdateAsync(ClipEmbedDto entity)
    {
        var embed = await context
            .ClipEmbeds
            .FirstOrDefaultAsync(x => x.GuildId == entity.GuildId)
            .ConfigureAwait(false);

        if (embed == null)
        {
            return null;
        }

        embed.Description = entity.Description;
        embed.Footer = entity.Description;
        embed.Header = entity.Description;
        embed.MoreButton = entity.Description;
        embed.WatchButton = entity.WatchButton;

        await context
            .SaveChangesAsync()
            .ConfigureAwait(false);

        return mapper.Map<ClipEmbedDto>(embed);
    }
}