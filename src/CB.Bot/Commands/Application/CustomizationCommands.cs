using CB.Accessors.Contracts;
using Discord;
using Discord.Interactions;

namespace CB.Bot.Commands.Application;

public enum CustomizeType
{
Clips,
Vod,
Live1,
Live2,
}

// TODO this is WIP
public class CustomizationCommands(IGuildAccessor guildAccessor,
    IClipEmbedAccessor clipEmbedAccessor,
    ILiveEmbedAccessor liveEmbedAccessor,
    IVodEmbedAccessor vodEmbedAccessor) : BaseSlashCommands
{
    //private readonly IGuildAccessor _guildAccessor;
    //private readonly IClipEmbedAccessor _clipEmbedAccessor;
    //private readonly ILiveEmbedAccessor _liveEmbedAccessor;
    //private readonly IVodEmbedAccessor _vodEmbedAccessor;

    //public CustomizationCommands(IGuildAccessor guildAccessor,
    //    IServiceScopeFactory serviceScopeFactory)
    //{
    //    _guildAccessor = guildAccessor;

    //    var scope = serviceScopeFactory.CreateScope();
    //    _clipEmbedAccessor = scope.ServiceProvider.GetRequiredService<IClipEmbedAccessor>();
    //    _liveEmbedAccessor = scope.ServiceProvider.GetRequiredService<ILiveEmbedAccessor>();
    //    _vodEmbedAccessor = scope.ServiceProvider.GetRequiredService<IVodEmbedAccessor>();
    //}

    [SlashCommand("customize", "Announcement customization")]
    public async Task CustomizeAsync(CustomizeType type)
    {
        await DeferAsync(true);
        if (Context.User.Id != 93015586698727424)
        {
            return;
        }

        switch (type)
        {
            case CustomizeType.Clips:
                var clipEmbed = await clipEmbedAccessor.GetByIdAsync(SocketInteraction.GuildId.ToString());
                await Context.Interaction.RespondWithModalAsync(new ModalBuilder()
                    .WithTitle("Customize Your Clips Embed!")
                    .WithCustomId($"clips_{Context.Guild.Id}_{Guid.NewGuid().ToString().Replace("-", "")}")
                    .AddTextInput("Header", "Header", placeholder: "New Clip @ %CHANNEL%", required: false, value: clipEmbed.Header)
                    .AddTextInput("Description", "Description", placeholder: "%DESCRIPTION%", required: false, value: clipEmbed.Description)
                    .AddTextInput("Footer", "Footer", placeholder: "Powered by 100% Couch • Created by %CHANNEL%", required: false, value: clipEmbed.Footer)
                    .AddTextInput("Watch Button", "WatchButton", placeholder: "Watch this Clip!", required: false, value: clipEmbed.WatchButton)
                    .AddTextInput("More Button", "MoreButton", placeholder: "And More!", required: false, value: clipEmbed.MoreButton).Build());
                break;
            case CustomizeType.Vod:
                var vodEmbed = await vodEmbedAccessor.GetByIdAsync(SocketInteraction.GuildId.ToString());
                await Context.Interaction.RespondWithModalAsync(new ModalBuilder()
                    .WithTitle("Customize Your VOD Embed!")
                    .WithCustomId($"vod_{Context.Guild.Id}_{Guid.NewGuid().ToString().Replace("-", "")}")
                    .AddTextInput("Header", "Header", placeholder: "%CHANNEL% has published a new video!", required: false, value: vodEmbed.Header)
                    .AddTextInput("Description Label", "DescriptionLabel", placeholder: "Video Description", required: false, value: vodEmbed.DescriptionLabel)
                    .AddTextInput("Description", "Description", placeholder: "%DESCRIPTION%", required: false, value: vodEmbed.Description)
                    .AddTextInput("Footer", "Footer", placeholder: "Powered by 100% Couch", required: false, value: vodEmbed.Footer)
                    .AddTextInput("Channel Button", "ChannelButton", placeholder: "Creator Channel!", required: false, value: vodEmbed.ChannelButton).Build());
                break;
            case CustomizeType.Live1:
                var liveEmbed1 = await liveEmbedAccessor.GetByIdAsync(SocketInteraction.GuildId.ToString());
                await Context.Interaction.RespondWithModalAsync(new ModalBuilder()
                    .WithTitle("Customize Your Live Embed (1/2)!")
                    .WithCustomId($"live_{Context.Guild.Id}_{Guid.NewGuid().ToString().Replace("-", "")}")
                    .AddTextInput("Header", "Header", placeholder: "%CHANNEL% is now streaming!", required: false, value: liveEmbed1.Header)
                    .AddTextInput("Description", "Description", placeholder: "%CHANNEL% is currently playing %GAME%", required: false, value: liveEmbed1.Description)
                    .AddTextInput("Last Streamed Label", "LastStreamed", placeholder: "Last Streamed", required: false, value: liveEmbed1.LastStreamed)
                    .AddTextInput("Average Stream Label", "AverageStream", placeholder: "Average Stream", required: false, value: liveEmbed1.AverageStream).Build());
                break;
            case CustomizeType.Live2:
                var liveEmbed2 = await liveEmbedAccessor.GetByIdAsync(SocketInteraction.GuildId.ToString());
                await Context.Interaction.RespondWithModalAsync(new ModalBuilder()
                    .WithTitle("Customize Your Live Embed (2/2)!")
                    .WithCustomId($"live_{Context.Guild.Id}_{Guid.NewGuid().ToString().Replace("-", "")}")
                    .AddTextInput("Description Label", "DescriptionLabel", placeholder: "Stream Description", required: false, value: liveEmbed2.DescriptionLabel)
                    .AddTextInput("Stream Description", "StreamDescription", placeholder: "%DESCRIPTION%", required: false, value: liveEmbed2.StreamDescription)
                    .AddTextInput("Footer Start", "FooterStart", placeholder: "Powered by 100% Couch • Started streaming", required: false, value: liveEmbed2.FooterStart)
                    .AddTextInput("Footer Stopped", "FooterStopped", placeholder: "Powered by 100% Couch • Stopped streaming", required: false, value: liveEmbed2.FooterStopped)
                    .AddTextInput("Channel Button", "ChannelButton", placeholder: "Creator Channel", required: false, value: liveEmbed2.ChannelButton).Build());
                break;
        }
    }
}