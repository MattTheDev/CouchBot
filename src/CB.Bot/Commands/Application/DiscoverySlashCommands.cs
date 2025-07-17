using CB.Accessors.Contracts;
using Discord;
using Discord.Interactions;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace CB.Bot.Commands.Application;

[Group("discovery", "Discovery commands")]
public class DiscoverySlashCommands(IGuildAccessor guildAccessor,
    IRoleConfigurationAccessor roleConfigurationAccessor,
    IAllowConfigurationAccessor allowConfigurationAccessor) : BaseSlashCommands
{
[SlashCommand(
        "enable",
        "Configure server 'Discovery' setting",
        false,
        RunMode.Async)]
    private async Task DiscoveryEnableConfigurationAsync(IRole role = null)
    {
        await DoStuff(true, role);
    }

    [SlashCommand(
        "disable",
        "Configure server 'Discovery' setting",
        false,
        RunMode.Async)]
    private async Task DiscoveryDisableConfigurationAsync()
    {
        await DoStuff(false);
    }

    private async Task DoStuff(bool isEnabled,
        IRole role = null)
    {
        await SocketInteraction.DeferAsync(true);

        if (!await IsUserAdmin())
        {
            return;
        }

        var guildChannel = (IGuildChannel)SocketInteraction.Channel;
        var guild = await guildAccessor.GetByIdAsync(guildChannel.Guild.Id.ToString());
        if (guild == null)
        {
            await SocketInteraction.FollowupAsync(
                "Sorry, unable to configure Discovery for this guild. Contact support.", ephemeral: true);
            return;
        }

        string message;
        if (!isEnabled)
        {
            guild.RoleConfiguration.DiscoveryRoleId = null;
            guild.AllowConfiguration.AllowLiveDiscovery = false;
            message = "Discovery has been set to `Disabled`.";
        }
        else
        {
            if (role != null)
            {
                guild.RoleConfiguration.DiscoveryRoleId = role.Id.ToString();
                guild.AllowConfiguration.AllowLiveDiscovery = true;

                message = $"Discovery has been set to role {role.Name}.";
            }
            else
            {
                guild.RoleConfiguration.DiscoveryRoleId = null;
                guild.AllowConfiguration.AllowLiveDiscovery = true;

                message = "Discovery has been set to .. literally everyone!";
            }
        }

        await roleConfigurationAccessor.UpdateAsync(guild.RoleConfiguration);
        await allowConfigurationAccessor.UpdateAsync(guild.AllowConfiguration);

        await SocketInteraction.FollowupAsync(message, ephemeral: true);
    }
}