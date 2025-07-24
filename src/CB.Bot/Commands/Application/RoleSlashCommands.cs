using CB.Accessors.Contracts;
using Discord;
using Discord.Interactions;
using RunMode = Discord.Interactions.RunMode;
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace CB.Bot.Commands.Application;

[Group("role", "Role commands")]
public class RoleSlashCommands(IGuildAccessor guildAccessor,
    IRoleConfigurationAccessor roleConfigurationAccessor)
    : BaseSlashCommands
{
    [SlashCommand("live",
        "Set your discovery live role command.",
        false,
        RunMode.Async)]
    private async Task SetLiveRoleAsync(IRole role)
    {
        await SocketInteraction.DeferAsync(true);

        if (!await IsUserAdmin())
        {
            return;
        }

        var guild = await guildAccessor.GetConfigurationSummaryByIdAsync(Context.Guild.Id.ToString());

        if (!await ValidateBotRoleFunctionality(role))
        {
            return;
        }

        guild.RoleConfiguration.LiveDiscoveryRoleId = role.Id.ToString();

        await roleConfigurationAccessor.UpdateAsync(guild.RoleConfiguration);
        await SocketInteraction.FollowupAsync($"When users go live they will get the {role.Name} role.");
    }

    [SlashCommand("join",
        "Set your join role command.",
        false,
        RunMode.Async)]
    private async Task SetJoinRoleAsync(IRole role)
    {
        await SocketInteraction.DeferAsync(true);

        if (!await IsUserAdmin())
        {
            return;
        }

        var guild = await guildAccessor.GetConfigurationSummaryByIdAsync(Context.Guild.Id.ToString());

        if (!await ValidateBotRoleFunctionality(role))
        {
            return;
        }

        guild.RoleConfiguration.JoinRoleId = role.Id.ToString();

        await roleConfigurationAccessor.UpdateAsync(guild.RoleConfiguration);
        await SocketInteraction.FollowupAsync($"When users join they will get the {role.Name} role.");
    }

    [SlashCommand("livenone",
        "Reset your discovery live role.",
        false,
        RunMode.Async)]
    private async Task ClearLiveRoleAsync()
    {
        await SocketInteraction.DeferAsync(true);

        if (!await IsUserAdmin())
        {
            return;
        }

        var guild = await guildAccessor.GetConfigurationSummaryByIdAsync(Context.Guild.Id.ToString());
        guild.RoleConfiguration.LiveDiscoveryRoleId = null;

        await roleConfigurationAccessor.UpdateAsync(guild.RoleConfiguration);
        await SocketInteraction.FollowupAsync($"Your live role has been cleared.");
    }

    [SlashCommand("joinnone",
        "Reset your discovery live role.",
        false,
        RunMode.Async)]
    private async Task ClearJoinRoleAsync()
    {
        await SocketInteraction.DeferAsync(true);

        if (!await IsUserAdmin())
        {
            return;
        }

        var guild = await guildAccessor.GetConfigurationSummaryByIdAsync(Context.Guild.Id.ToString());
        guild.RoleConfiguration.JoinRoleId = null;

        await roleConfigurationAccessor.UpdateAsync(guild.RoleConfiguration);
        await SocketInteraction.FollowupAsync("Your join role has been cleared.");
    }

    private async Task<bool> ValidateBotRoleFunctionality(IRole role)
    {
        if (!await BotHasManageRoles(Context.Guild, Context.Client.CurrentUser))
        {
            await SocketInteraction.FollowupAsync("Sorry, I do not have 'Manage Roles' permission on this server. Fix that, and retry.");

            return false;
        }

        if (await BotHasHigherRole(Context.Guild, Context.Client.CurrentUser, role))
        {
            return true;
        }

        await SocketInteraction.FollowupAsync("Sorry, my current role is lower in the server role list than the role I will be assigning. Fix that, and retry.");

        return false;
    }

    private async Task<bool> BotHasManageRoles(IGuild guild, IUser user)
    {
        var u = await guild.GetUserAsync(user.Id);

        return u is { GuildPermissions.ManageRoles: true };
    }

    private async Task<bool> BotHasHigherRole(IGuild guild, IUser user, IRole role)
    {
        var u = await guild.GetUserAsync(user.Id);

        return u.RoleIds.Select(guild.GetRole).Any(guildRole => guildRole.CompareTo(role) > 0);
    }
}