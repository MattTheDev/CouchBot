using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace CB.Bot.Commands.Application;

/// <summary>
/// Base class to provide easier access to helper functions for our slash command implementations.
/// </summary>
public class BaseSlashCommands() : InteractionModuleBase
{
    public IGuildUser GuildUser => (IGuildUser)Context.User;
    public IGuildChannel GuildChannel => (IGuildChannel)Context.Channel;
    public SocketInteraction SocketInteraction => (SocketInteraction)Context.Interaction;
    public SocketSlashCommand SocketSlashCommand => (SocketSlashCommand)SocketInteraction;

    /// <summary>
    /// Validate if the user executing the slash command is an approved admin, or not.
    /// </summary>
    /// <returns>Is user admin? true or false</returns>
    public async Task<bool> IsUserAdmin(bool sendResponse = true)
    {
        // TODO MS - We need to add ApprovedAdmin code.
        // This'll do for now.
        var isAdmin = GuildUser.GuildPermissions.ManageGuild;
        
        if (!isAdmin && sendResponse)
        {
            await SocketInteraction.FollowupAsync("Sorry, you have to be an admin to use that command.",
                ephemeral: true)
                .ConfigureAwait(false);
        }

        return isAdmin;
    }
}