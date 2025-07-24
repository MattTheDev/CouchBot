using CB.Shared.Dtos;

namespace CB.Accessors.Contracts;

public interface IMessageConfigurationAccessor
{
    Task<MessageConfigurationDto> UpdateAsync(MessageConfigurationDto updated);
}