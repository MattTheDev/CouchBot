using AutoMapper;
using CB.Accessors.Contracts;
using CB.Data;
using CB.Shared.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CB.Accessors.Implementations;

public class LiveEmbedAccessor(CbContext context, 
    IMapper mapper) 
    : ILiveEmbedAccessor
{
    public async Task<LiveEmbedDto> GetByIdAsync(string guildId)
    {
        var entity = await context
            .LiveEmbeds
            .FirstOrDefaultAsync(x => x.GuildId == guildId);

        if (entity == null)
        {
            entity = new()
            {
                GuildId = guildId,
                Header = "%CHANNEL% is now streaming!",
                DescriptionLabel = "Stream Description",
                Description = "%CHANNEL% is currently playing %GAME%",
                LastStreamed = "Last Streamed",
                AverageStream = "Average Stream",
                StreamDescription = "%DESCRIPTION%",
                FooterStopped = "Powered by 100% Couch • Stopped streaming",
                FooterStart = "Powered by 100% Couch • Started streaming",
                ChannelButton = "Creator Channel",
            };

            await context
                .LiveEmbeds
                .AddAsync(entity);
            await context
                .SaveChangesAsync();
        }

        return mapper.Map<LiveEmbedDto>(entity);
    }

    public async Task<LiveEmbedDto> UpdateAsync(LiveEmbedDto entity)
    {
        var embed = await context
            .LiveEmbeds
            .FirstOrDefaultAsync(x => x.GuildId == entity.GuildId)
            .ConfigureAwait(false);

        if (embed == null)
        {
            return null;
        }

        embed.AverageStream = entity.AverageStream;
        embed.ChannelButton = entity.ChannelButton;
        embed.Description = entity.Description;
        embed.DescriptionLabel = entity.DescriptionLabel;
        embed.FooterStart = entity.FooterStart;
        embed.FooterStopped = entity.FooterStopped;
        embed.Header = entity.Header;
        embed.LastStreamed = entity.LastStreamed;
        embed.StreamDescription = entity.StreamDescription;

        await context
            .SaveChangesAsync()
            .ConfigureAwait(false);

        return mapper.Map<LiveEmbedDto>(embed);
    }
}