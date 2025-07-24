using AutoMapper;
using CB.Accessors.Contracts;
using CB.Data;
using CB.Shared.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CB.Accessors.Implementations;

public class MessageConfigurationAccessor(CbContext context, 
    IMapper mapper) 
    : IMessageConfigurationAccessor
{
    public async Task<MessageConfigurationDto> UpdateAsync(MessageConfigurationDto updated)
    {
        var messageConfiguration = await context
            .MessageConfigurations
            .FirstOrDefaultAsync(x => x.GuildId == updated.GuildId)
            .ConfigureAwait(false);

        if (messageConfiguration == null)
        {
            return null;
        }

        messageConfiguration.GoodbyeMessage = updated.GoodbyeMessage;
        messageConfiguration.GreetingMessage = updated.GreetingMessage;
        messageConfiguration.LiveMessage = updated.LiveMessage;
        messageConfiguration.PublishedMessage = updated.PublishedMessage;
        messageConfiguration.StreamOfflineMessage = updated.StreamOfflineMessage;

        await context
            .SaveChangesAsync()
            .ConfigureAwait(false);

        return mapper.Map<MessageConfigurationDto>(messageConfiguration);
    }
}