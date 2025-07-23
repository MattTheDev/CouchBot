using CB.Accessors.Contracts;
using CB.Data.Entities;
using Discord.Interactions;
using FilterType = CB.Shared.Enums.FilterType;
using Platform = CB.Shared.Enums.Platform;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace CB.Bot.Commands.Application;

public class FilterSlashCommands(IFilterAccessor filterAccessor) 
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

        var t = (int)platform;
        filterText = filterText.TrimStart('"').TrimEnd('"');
        var existingFilters = await filterAccessor.GetAllAsync(SocketInteraction.GuildId.ToString());
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
                GuildId = SocketInteraction.GuildId.ToString()
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