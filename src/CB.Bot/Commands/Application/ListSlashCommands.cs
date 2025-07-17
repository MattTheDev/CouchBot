using System.Text;
using CB.Accessors.Contracts;
using CB.Shared.Dtos;
using CB.Shared.Models.Bot;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace CB.Bot.Commands.Application;

/// <summary>
/// List Slash Commands
/// </summary>
public class ListSlashCommands(DiscordSocketClient discordSocketClient,
    IGuildAccessor guildAccessor,
    IFilterAccessor filterAccessor,
    IChannelAccessor channelAccessor) 
    : BaseSlashCommands
{
    /// <summary>
    /// List games slash command
    /// </summary>
    /// <returns></returns>
    [SlashCommand(
        "games",
        "List your servers configured games.",
        false,
        RunMode.Async)]
    private async Task ListGamesAsync(int page = 1,
        IGuildChannel channel = null)
    {
        await SocketInteraction.DeferAsync(true);

        if (!await IsUserAdmin())
        {
            return;
        }

        await ProcessGameList(page, channel?.Id.ToString(), Context.Guild, SocketInteraction);
    }

    /// <summary>
    /// List teams slash command
    /// </summary>
    /// <returns></returns>
    [SlashCommand(
        "teams",
        "List your servers configured teams.",
        false,
        RunMode.Async)]
    private async Task ListTeamsAsync(int page = 1,
        IGuildChannel channel = null)
    {
        await SocketInteraction.DeferAsync(true);

        if (!await IsUserAdmin())
        {
            return;
        }

        await ProcessTeamList(page, channel?.Id.ToString(), Context.Guild, SocketInteraction);
    }

    /// <summary>
    /// Creators slash command
    /// </summary>
    /// <returns></returns>
    [SlashCommand(
        "creators",
        "List your servers configured creators.",
        false,
        RunMode.Async)]
    private async Task ListCreatorsAsync(int page = 1,
        IGuildChannel channel = null)
    {
        await SocketInteraction.DeferAsync(true);

        if (!await IsUserAdmin())
        {
            return;
        }

        await ProcessCreatorList(page, channel?.Id.ToString(), Context.Guild, SocketInteraction);
    }

    /// <summary>
    /// Admins slash command
    /// </summary>
    /// <returns></returns>
    [SlashCommand(
        "filters",
        "List your servers configured filters.",
        false,
        RunMode.Async)]
    private async Task ListFiltersAsync(int page = 1)
    {
        await SocketInteraction.DeferAsync(true);

        if (!await IsUserAdmin())
        {
            return;
        }

        await ProcessFilterList(page, Context.Guild, SocketInteraction);
    }

    private async Task ProcessCreatorList(
        long page,
        string channelId,
        IGuild discordGuild,
        SocketInteraction socketInteraction)
    {
        var guildCreators = await guildAccessor.GetByIdAsync(discordGuild.Id.ToString());
        var creators = new List<ListViewModel>();

        if (string.IsNullOrWhiteSpace(channelId))
        {
            guildCreators?.Channels.ToList().ForEach(async x =>
            {
                await ValidateChannelDisplayName(socketInteraction, x);
                creators.AddRange(x.CreatorChannels.Select(y => y.Creator).Select(creator => new ListViewModel
                {
                    DisplayName = creator.DisplayName,
                    CreatorChannelId = creator.ChannelId,
                    PlatformId = creator.PlatformId,
                    ChannelName = x.DisplayName
                }));
            });
        }
        else
        {
            guildCreators?.Channels.Where(x => x.Id.Equals(channelId)).ToList().ForEach(x =>
            {
                ValidateChannelDisplayName(socketInteraction, x).GetAwaiter().GetResult();
                creators.AddRange(x.CreatorChannels.Select(y=>y.Creator).Select(creator => new ListViewModel
                {
                    DisplayName = creator.DisplayName,
                    CreatorChannelId = creator.ChannelId,
                    PlatformId = creator.PlatformId,
                    ChannelName = x.DisplayName
                }));
            });
        }

        var totalCreators = creators.Count;
        var pageCount = (creators.Count + 20 - 1) / 20;

        if (page <= 0)
        {
            page = 1;
        }

        if (page > pageCount)
        {
            page = pageCount;
        }

        if (pageCount == 0)
        {
            pageCount++;
        }

        var creatorsToDisplay = page > 1 ?
            creators.OrderBy(x => x.DisplayName).ThenBy(x => x.PlatformId).Skip((int)(20 * (page - 1))).Take(20).ToList() :
            creators.OrderBy(x => x.DisplayName).ThenBy(x => x.PlatformId).Take(20).ToList();

        var builder = new EmbedBuilder();
        var authorBuilder = new EmbedAuthorBuilder();
        var footerBuilder = new EmbedFooterBuilder();

        authorBuilder.IconUrl = discordSocketClient.CurrentUser.GetAvatarUrl();
        authorBuilder.Name = discordSocketClient.CurrentUser.Username;
        authorBuilder.Url = "https://couch.bot";

        footerBuilder.IconUrl = discordSocketClient.CurrentUser.GetAvatarUrl();
        footerBuilder.Text = $"Page {page} of {pageCount} • {totalCreators} creators";

        builder.Author = authorBuilder;
        builder.Footer = footerBuilder;
        builder.Color = new Color(88, 101, 242);
        var strBuilder = new StringBuilder();

        foreach (var creatorOutput in creatorsToDisplay.Select(creator => creator.PlatformId switch
        {
            (int)Shared.Enums.Platform.DLive =>
                $"<:dlive:886103116494299136> [{creator.DisplayName}](https://dlive.tv/{creator.DisplayName}) - #{creator.ChannelName}",
            (int)Shared.Enums.Platform.Picarto =>
                $"<:picarto:844040506056638465> [{creator.DisplayName}](https://picarto.tv/{creator.DisplayName}) - #{creator.ChannelName}",
            (int)Shared.Enums.Platform.Piczel =>
                $"<:piczel:844040506265698304> [{creator.DisplayName}](https://piczel.tv/{creator.DisplayName}) - #{creator.ChannelName}",
            (int)Shared.Enums.Platform.Trovo =>
                $"<:trovo:844040506224803872> [{creator.DisplayName}](https://trovo.live/{creator.DisplayName}) - #{creator.ChannelName}",
            (int)Shared.Enums.Platform.Twitch =>
                $"<:twitch:844040506056376321> [{creator.DisplayName}](https://twitch.tv/{creator.DisplayName}) - #{creator.ChannelName}",
            (int)Shared.Enums.Platform.YouTube =>
                $"<:youtube:844040506321141800> [{creator.DisplayName}](https://youtube.com/channel/{creator.CreatorChannelId}) - #{creator.ChannelName}",
            _ => ""
        }))
        {
            strBuilder.AppendLine($"{creatorOutput}");
        }

        builder.Description = creators.Count == 0 ? "It doesn't look like you are announcing any creators." : strBuilder.ToString();
        await socketInteraction.FollowupAsync(embed: builder.Build(), ephemeral: true);
    }

    private async Task ProcessFilterList(
        long page,
        IGuild discordGuild,
        SocketInteraction socketInteraction)
    {
        var guild = await guildAccessor.GetByIdAsync(discordGuild.Id.ToString());
        var filters = await filterAccessor.GetAllAsync(guild.Id);

        var builder = new EmbedBuilder();
        var authorBuilder = new EmbedAuthorBuilder();
        var footerBuilder = new EmbedFooterBuilder();

        var totalFilters = filters.Count;
        var pageCount = (filters.Count + 20 - 1) / 20;

        if (page <= 0)
        {
            page = 1;
        }

        if (page > pageCount)
        {
            page = pageCount;
        }

        if (pageCount == 0)
        {
            pageCount++;
        }

        var filtersToDisplay = page > 1 ?
            filters.OrderBy(x => x.PlatformId).ThenBy(x => x.Text).Skip((int)(20 * (page - 1))).Take(20).ToList() :
            filters.OrderBy(x => x.PlatformId).ThenBy(x => x.Text).Take(20).ToList();

        authorBuilder.IconUrl = discordSocketClient.CurrentUser.GetAvatarUrl();
        authorBuilder.Name = discordSocketClient.CurrentUser.Username;
        authorBuilder.Url = "https://couch.bot";

        footerBuilder.IconUrl = discordSocketClient.CurrentUser.GetAvatarUrl();
        footerBuilder.Text = $"Page {page} of {pageCount} • {totalFilters} filters";

        builder.Author = authorBuilder;
        builder.Footer = footerBuilder;
        builder.Color = new Color(88, 101, 242);

        builder.AddField("Game Filters",
            filters.Count(x => x.FilterTypeId == (int)Shared.Enums.FilterType.Game) == 0 ? "None" :
                string.Join("\r\n",
                    filtersToDisplay
                    .Where(x => x.FilterTypeId == (int)Shared.Enums.FilterType.Game)
                    .OrderBy(x => x.PlatformId)
                    .Select(x => $"{Format.Sanitize(x.Text)} ({(Shared.Enums.Platform)x.PlatformId})")
                    )
                );

        builder.AddField("Title Filters",
            filters.Count(x => x.FilterTypeId == (int)Shared.Enums.FilterType.Title) == 0 ? "None" :
                string.Join("\r\n",
                    filtersToDisplay
                    .Where(x => x.FilterTypeId == (int)Shared.Enums.FilterType.Title)
                    .OrderBy(x => x.PlatformId)
                    .Select(x => $"{Format.Sanitize(x.Text)} ({(Shared.Enums.Platform)x.PlatformId})")
                    )
                );

        await socketInteraction.FollowupAsync(embed: builder.Build(), ephemeral: true);
    }

    private async Task ProcessGameList(
        long page,
        string channelId,
        IGuild discordGuild,
        SocketInteraction socketInteraction)
    {
        var guildCreators = await guildAccessor.GetByIdAsync(discordGuild.Id.ToString());

        var builder = new EmbedBuilder();
        var authorBuilder = new EmbedAuthorBuilder();
        var footerBuilder = new EmbedFooterBuilder();

        var games = new List<ListViewModel>();

        if (string.IsNullOrWhiteSpace(channelId))
        {
            guildCreators?.Channels.ToList().ForEach(async x =>
            {
                await ValidateChannelDisplayName(socketInteraction, x);
                games.AddRange(x.GameChannels.Select(y => y.Game).Select(game => new ListViewModel
                {
                    DisplayName = game.DisplayName,
                    ChannelName = x.DisplayName
                }));
            });
        }
        else
        {
            guildCreators?.Channels.Where(x => x.Id.Equals(channelId)).ToList().ForEach(x =>
            {
                ValidateChannelDisplayName(socketInteraction, x).GetAwaiter().GetResult();
                games.AddRange(x.GameChannels.Select(y => y.Game).Select(game => new ListViewModel
                {
                    DisplayName = game.DisplayName,
                    ChannelName = x.DisplayName
                }));
            });
        }

        var totalGames = games.Count;
        var pageCount = (games.Count + 20 - 1) / 20;

        if (page <= 0)
        {
            page = 1;
        }

        if (page > pageCount)
        {
            page = pageCount;
        }

        if (pageCount == 0)
        {
            pageCount++;
        }

        var gamesToDisplay = page > 1 ?
            games.OrderBy(x => x.DisplayName).Skip((int)(20 * (page - 1))).Take(20).ToList() :
            games.OrderBy(x => x.DisplayName).Take(20).ToList();

        authorBuilder.IconUrl = discordSocketClient.CurrentUser.GetAvatarUrl();
        authorBuilder.Name = discordSocketClient.CurrentUser.Username;
        authorBuilder.Url = "https://couch.bot";

        footerBuilder.IconUrl = discordSocketClient.CurrentUser.GetAvatarUrl();
        footerBuilder.Text = $"Page {page} of {pageCount} • {totalGames} games";

        builder.Author = authorBuilder;
        builder.Footer = footerBuilder;
        builder.Color = new Color(88, 101, 242);

        builder.Description = totalGames == 0 ? "It doesn't look like you are announcing any games." : string.Join("\r\n", gamesToDisplay.Select(x => $"<:twitch:844040506056376321> {x.DisplayName} - #{x.ChannelName}"));

        await socketInteraction.FollowupAsync(embed: builder.Build(), ephemeral: true);
    }

    private async Task ValidateChannelDisplayName(SocketInteraction socketInteraction,
        ChannelDto x)
    {
        if (string.IsNullOrEmpty(x.DisplayName))
        {
            var guild = discordSocketClient.GetGuild(socketInteraction.GuildId.Value);
            var channel = guild.GetChannel(ulong.Parse(x.Id));
            x.DisplayName = channel.Name;
            x.ModifiedDate = DateTime.UtcNow;
            await channelAccessor.UpdateAsync(x);
        }
    }

    private async Task ProcessTeamList(
        long page,
        string channelId,
        IGuild discordGuild,
        SocketInteraction socketInteraction)
    {
        var guildCreators = await guildAccessor.GetByIdAsync(discordGuild.Id.ToString());

        var builder = new EmbedBuilder();
        var authorBuilder = new EmbedAuthorBuilder();
        var footerBuilder = new EmbedFooterBuilder();

        var teams = new List<ListViewModel>();
        if (string.IsNullOrWhiteSpace(channelId))
        {
            guildCreators?.Channels.ToList().ForEach(async x =>
            {
                await ValidateChannelDisplayName(socketInteraction, x);
                teams.AddRange(x.TeamChannels.Select(y => y.Team).Select(team => new ListViewModel
                {
                    DisplayName = team.DisplayName,
                    ChannelName = x.DisplayName
                }));
            });
        }
        else
        {
            guildCreators?.Channels.Where(x => x.Id.Equals(channelId)).ToList().ForEach(x =>
            {
                ValidateChannelDisplayName(socketInteraction, x).GetAwaiter().GetResult();
                teams.AddRange(x.TeamChannels.Select(y => y.Team).Select(team => new ListViewModel
                {
                    DisplayName = team.DisplayName,
                    ChannelName = x.DisplayName
                }));
            });
        }

        var totalTeams = teams.Count;
        var pageCount = (teams.Count + 20 - 1) / 20;

        if (page <= 0)
        {
            page = 1;
        }

        if (page > pageCount)
        {
            page = pageCount;
        }

        if (pageCount == 0)
        {
            pageCount++;
        }

        var teamsToDisplay = page > 1 ?
            teams.OrderBy(x => x.DisplayName).Skip((int)(20 * (page - 1))).Take(20).ToList() :
            teams.OrderBy(x => x.DisplayName).Take(20).ToList();

        authorBuilder.IconUrl = discordSocketClient.CurrentUser.GetAvatarUrl();
        authorBuilder.Name = discordSocketClient.CurrentUser.Username;
        authorBuilder.Url = "https://couch.bot";

        footerBuilder.IconUrl = discordSocketClient.CurrentUser.GetAvatarUrl();
        footerBuilder.Text = $"Page {page} of {pageCount} • {totalTeams} teams";

        builder.Author = authorBuilder;
        builder.Footer = footerBuilder;
        builder.Color = new Color(88, 101, 242);

        builder.Description = totalTeams == 0 ? "It doesn't look like you are announcing any teams." : string.Join("\r\n", teamsToDisplay.Select(x => $"<:twitch:844040506056376321> {x.DisplayName} - #{x.ChannelName}"));

        await socketInteraction.FollowupAsync(embed: builder.Build(), ephemeral: true);
    }
}