using CB.Accessors.Contracts;
using CB.Data.Entities;
using Discord.Interactions;
using FilterType = CB.Shared.Enums.FilterType;
using Platform = CB.Shared.Enums.Platform;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace CB.Bot.Commands.Application;

public class FilterSlashCommands(IGuildAccessor guildAccessor, 
    IFilterAccessor filterAccessor) 
    : BaseSlashCommands
{
    [SlashCommand(
        "filter",
        "Manage your filters.",
        false,
        RunMode.Async)]
    private async Task ToggleFilter(string filterText,
        FilterType filterType,
        Platform platform)
    {
        await SocketInteraction.DeferAsync(true);

        if (!await IsUserAdmin())
        {
            return;
        }

        filterText = filterText.TrimStart('"').TrimEnd('"');
        var guild = await guildAccessor.GetByIdAsync(SocketInteraction.GuildId.ToString());
        var existingFilters = await filterAccessor.GetAllAsync(guild?.Id);
        var existingFilter = existingFilters.FirstOrDefault(
            x => x.PlatformId == (int)platform &&
                 x.FilterTypeId == (int)filterType &&
                 x.Text.Equals(filterText, StringComparison.OrdinalIgnoreCase));

        if (existingFilter == null)
        {
            await filterAccessor.CreateAsync(new Filter
            {
                Text = filterText,
                PlatformId = (int)platform,
                FilterTypeId = (int)filterType,
                GuildId = guild?.Id.ToString()
            });

            await SocketInteraction.FollowupAsync($"{filterType} filter has been created.");
        }
        else
        {
            await filterAccessor.DeleteAsync(existingFilter);
            await SocketInteraction.FollowupAsync($"{filterType} filter has been removed.");
        }
    }
}