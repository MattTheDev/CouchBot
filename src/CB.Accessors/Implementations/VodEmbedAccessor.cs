using AutoMapper;
using CB.Accessors.Contracts;
using CB.Data;
using CB.Shared.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CB.Accessors.Implementations;

public class VodEmbedAccessor(CbContext context, 
    IMapper mapper) 
    : IVodEmbedAccessor
{
    public async Task<VodEmbedDto> GetByIdAsync(string guildId)
    {
        var entity = await context
            .VodEmbeds
            .FirstOrDefaultAsync(x => x.GuildId == guildId);

        if (entity == null)
        {
            entity = new()
            {
                GuildId = guildId,
                Header = "%CHANNEL% has published a new video!",
                Description = "%DESCRIPTION%",
                DescriptionLabel = "Video Description",
                Footer = "Powered by 100% Couch",
                ChannelButton = "Creator Channel!",
            };

            await context
                .VodEmbeds
                .AddAsync(entity);
            await context
                .SaveChangesAsync();
        }

        return mapper.Map<VodEmbedDto>(entity);
    }

    public async Task<VodEmbedDto> UpdateAsync(VodEmbedDto entity)
    {
        var embed = await context
            .VodEmbeds
            .FirstOrDefaultAsync(x => x.GuildId == entity.GuildId)
            .ConfigureAwait(false);

        if (embed == null)
        {
            return null;
        }

        embed.Header = entity.Header;
        embed.ChannelButton = entity.ChannelButton;
        embed.Description = entity.Description;
        embed.DescriptionLabel = entity.DescriptionLabel;
        embed.Footer = entity.Footer;

        await context
            .SaveChangesAsync()
            .ConfigureAwait(false);

        return mapper.Map<VodEmbedDto>(embed);
    }
}