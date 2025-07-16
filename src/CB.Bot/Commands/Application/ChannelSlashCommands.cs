using CB.Accessors.Contracts;
using CB.Shared.Enums;
using Discord;
using Discord.Interactions;
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace CB.Bot.Commands.Application;

[Group("channel", "Channel configuration")]
public class ChannelSlashCommands(IGuildAccessor guildAccessor,
    IChannelAccessor channelAccessor,
    IChannelConfigurationAccessor channelConfigurationAccessor) : BaseSlashCommands
{
    [SlashCommand(
        "live",
        "Configure server 'Channel' settings",
        false,
        RunMode.Async)]
    private async Task LiveChannelConfigurationAsync(IGuildChannel channel)
    {
        await ConfigureChannelAsync(ConfiguredChannelType.Live, 
            channel);
    }

    [SlashCommand(
        "greetings",
        "Configure server 'Channel' settings",
        false,
        RunMode.Async)]
    private async Task GreetingChannelConfigurationAsync(IGuildChannel channel)
    {
        await ConfigureChannelAsync(ConfiguredChannelType.Greetings,
            channel);
    }

    [SlashCommand(
        "goodbyes",
        "Configure server 'Channel' settings",
        false,
        RunMode.Async)]
    private async Task GoodbyeChannelConfigurationAsync(IGuildChannel channel)
    {
        await ConfigureChannelAsync(ConfiguredChannelType.Goodbyes, 
            channel);
    }

    [SlashCommand(
        "discordlive",
        "Configure server 'Discord Live Channel' setting",
        false,
        RunMode.Async)]
    private async Task DiscordLiveChannelConfigurationAsync(IGuildChannel channel)
    {
        await ConfigureChannelAsync(ConfiguredChannelType.DiscordLive, 
            channel);
    }

    private async Task ConfigureChannelAsync(ConfiguredChannelType configuredChannelType,
        IGuildChannel discordChannel)
    {
        await SocketInteraction
            .DeferAsync(true)
            .ConfigureAwait(false);

        if (!await IsUserAdmin())
        {
            return;
        }

        var guild = await guildAccessor
            .GetByIdAsync(Context.Guild.Id.ToString())
            .ConfigureAwait(false);

        if (guild == null)
        {
            await FollowupAsync($"There was an issue setting your '{configuredChannelType}' channel. Contact support.", ephemeral: true)
                .ConfigureAwait(false);
            return;
        }

        var existingChannel = await channelAccessor.GetByIdAsync(discordChannel.Id.ToString()).ConfigureAwait(false)
                      ?? await channelAccessor.CreateAsync(new()
                      {
                          CreatedDate = DateTime.UtcNow,
                          ModifiedDate = DateTime.UtcNow,
                          DisplayName = discordChannel.Name,
                          GuildId = guild.Id,
                          Id = discordChannel.Id.ToString()
                      }).ConfigureAwait(false);

        switch (configuredChannelType)
        {
            case ConfiguredChannelType.Greetings:
                guild.ChannelConfiguration.GreetingChannelId = existingChannel.Id;
                break;
            case ConfiguredChannelType.Goodbyes:
                guild.ChannelConfiguration.GoodbyeChannelId = existingChannel.Id;
                break;
            case ConfiguredChannelType.Live:
                guild.ChannelConfiguration.LiveChannelId = existingChannel.Id;
                break;
            case ConfiguredChannelType.DiscordLive:
                guild.ChannelConfiguration.DiscordLiveChannelId = existingChannel.Id;
                break;
        }

        await channelConfigurationAccessor.UpdateAsync(guild.ChannelConfiguration.GuildId,
            guild.ChannelConfiguration)
            .ConfigureAwait(false);

        await FollowupAsync($"Your '{configuredChannelType}' channel is now #{existingChannel.DisplayName}",
                ephemeral: true)
            .ConfigureAwait(false);
    }
}