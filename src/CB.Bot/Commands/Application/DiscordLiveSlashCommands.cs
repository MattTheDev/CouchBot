using CB.Accessors.Contracts;
using CB.Data.Entities;
using Discord;
using Discord.Interactions;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace CB.Bot.Commands.Application;

public class DiscordLiveSlashCommands(IDiscordLiveConfigurationAccessor discordLiveConfigurationAccessor)
    : BaseSlashCommands
{
    [SlashCommand("discordlive",
        "Configure the Discord Live notification.",
        false,
        RunMode.Async)]
    private async Task DiscordLiveAsync(string description,
        string footer,
        string header,
        string message,
        IRole mentionRole = null)
    {
        await SocketInteraction.DeferAsync(true);

        if (!await IsUserAdmin())
        {
            return;
        }

        var config = await discordLiveConfigurationAccessor.GetByIdAsync(GuildUser.GuildId.ToString()) ?? await discordLiveConfigurationAccessor.CreateAsync(new DiscordLiveConfiguration
        {
            GuildId = GuildUser.GuildId.ToString()
        });

        config.Description = description;
        config.Footer = footer;
        config.Header = header;
        config.Message = message;
        config.MentionRoleId = mentionRole?.Id.ToString();

        await discordLiveConfigurationAccessor.UpdateAsync(config);
        await SocketInteraction.FollowupAsync("Your configuration for your Discord Live announcements has been set!");
    }
}