using System.Text;
using CB.Accessors.Contracts;
using CB.Data.Entities;
using CB.Shared.Dtos;
using CB.Shared.Enums;
using CB.Shared.Models.Bot;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Newtonsoft.Json;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace CB.Bot.Commands.Application;

public class ConfigurationSlashCommands(IGuildAccessor guildAccessor,
    IDropdownPayloadAccessor dropdownPayloadAccessor,
    DiscordSocketClient discordSocketClient) : BaseSlashCommands
{
    [SlashCommand(
        "allow",
        "Configure server 'allow' settings",
        false,
        RunMode.Async)]
    private async Task AllowConfigurationAsync()
    {
        await SocketInteraction.DeferAsync(true);

        if (!await IsUserAdmin())
        {
            return;
        }

        var guild = await guildAccessor
            .GetByIdAsync(Context.Guild.Id.ToString())
            .ConfigureAwait(false);

        if (guild == null)
        {
            await FollowupAsync("There was an issue retrieving your guild. Contact support.", ephemeral: true)
                .ConfigureAwait(false);
            return;
        }

        var payload = JsonConvert.SerializeObject(new GenericDropdownPayload
        {
            AuthorId = SocketInteraction.User.Id
        });

        var payloadDto = await dropdownPayloadAccessor.CreateAsync(
            new DropdownPayload
            {
                DropdownType = DropdownType.AllowConfiguration.ToString(),
                Payload = payload,
                OriginalMessageId = ""
            });

        var couchBotGuild = discordSocketClient.GetGuild(263688866978988032);
        var redcross = couchBotGuild.Emotes.FirstOrDefault(x => x.Id == 921804940698087504);
        var greentick = couchBotGuild.Emotes.FirstOrDefault(x => x.Id == 921804940383494176);

        var options = new List<SelectMenuOptionBuilder>
        {
            new()
            {
                Value = "deleteoffline", Description = "Allow deletion of completed stream announcements.",
                Label = "Delete Offline Streams", Emote = guild.GuildConfiguration.DeleteOffline ? greentick : redcross
            },
            new()
            {
                Value = "greetings", Description = "Allow greetings on new member joining server.", Label = "Greetings",
                Emote = guild.AllowConfiguration.AllowGreetings ? greentick : redcross
            },
            new()
            {
                Value = "goodbyes", Description = "Allow goodbyes on member leaving server.", Label = "Goodbyes",
                Emote = guild.AllowConfiguration.AllowGoodbyes ? greentick : redcross
            },
            new()
            {
                Value = "live", Description = "Allow livestreams to be announced.", Label = "Live",
                Emote = guild.AllowConfiguration.AllowLive ? greentick : redcross
            },
            new()
            {
                Value = "discordlive", Description = "Allow live Discord streams to be displayed in announcements.",
                Label = "DiscordLive", Emote = guild.AllowConfiguration.AllowDiscordLive ? greentick : redcross
            },
            new()
            {
                Value = "textannouncements", Description = "Allow only text announcements, no styled embeds.",
                Label = "Plain Text Announcements",
                Emote = guild.GuildConfiguration.TextAnnouncements ? greentick : redcross
            },
            new()
            {
                Value = "published", Description = "Allow published content to be announced.", Label = "Published",
                Emote = guild.AllowConfiguration.AllowPublished ? greentick : redcross
            },
            new()
            {
                Value = "thumbnails", Description = "Allow thumbnails to be displayed in announcements.",
                Label = "Thumbnails", Emote = guild.AllowConfiguration.AllowThumbnails ? greentick : redcross
            }
        };

        var component = new ComponentBuilder().WithSelectMenu($"Allow.{SocketInteraction.User.Id}.{payloadDto.Id}",
            options, "Choose an option to toggle ...",
            maxValues: options.Count);

        await SocketInteraction.FollowupAsync("Choose an `Allow` option to toggle on / off: ",
            components: component.Build(),
            ephemeral: true);
    }

    [SlashCommand(
        "config",
        "List servers configuration",
        false,
        RunMode.Async)]
    private async Task ConfigurationAsync()
    {
        await SocketInteraction.DeferAsync(true).ConfigureAwait(false);

        if (!await IsUserAdmin())
        {
            return;
        }

        var guild = await guildAccessor.GetByIdAsync(Context.Guild.Id.ToString()).ConfigureAwait(false);

        var builder = new EmbedBuilder();
        var authorBuilder = new EmbedAuthorBuilder();
        var footerBuilder = new EmbedFooterBuilder();

        await BuildEmbed(builder, authorBuilder, footerBuilder, Context.Guild).ConfigureAwait(false);
        PopulateEmbedFields(builder, guild);
        await SocketInteraction.FollowupAsync(embed: builder.Build()).ConfigureAwait(false);
    }

    private async Task BuildEmbed(
        EmbedBuilder builder,
        EmbedAuthorBuilder authorBuilder,
        EmbedFooterBuilder footerBuilder,
        IGuild guild)
    {
        authorBuilder.IconUrl = discordSocketClient.CurrentUser.GetAvatarUrl();
        authorBuilder.Name = discordSocketClient.CurrentUser.Username;
        authorBuilder.Url = "https://couch.bot";

        footerBuilder.IconUrl = discordSocketClient.CurrentUser.GetAvatarUrl();
        footerBuilder.Text = $"{guild.Name} Configuration • Established by {(await guild.GetOwnerAsync().ConfigureAwait(false)).Username} on {guild.CreatedAt:d}";

        builder.Author = authorBuilder;
        builder.Footer = footerBuilder;
        builder.Color = new Color(88, 101, 242);
    }

    private void PopulateEmbedFields(EmbedBuilder builder, GuildDto guild)
    {
        builder.AddField("Allows", GetAllowConfigurationString(guild, 0), true);

        GetAllowConfigurationString(guild, 0);
        builder.AddField("-", GetAllowConfigurationString(guild, 1), true);

        var channelConfigurationString = GetChannelConfigurationString(guild);

        if (!string.IsNullOrEmpty(channelConfigurationString))
        {
            builder.AddField("Channels", channelConfigurationString, true);
        }

        builder.AddField("Server", GetServerConfigurationString(guild), true);

        var roleConfigurationString = GetRoleConfigurationString(guild);

        if (!string.IsNullOrEmpty(roleConfigurationString))
        {
            builder.AddField("Roles", roleConfigurationString, true);
        }

        builder.AddField("Messages", GetMessageConfigurationString(guild));
    }

    private string GetAllowConfigurationString(GuildDto guild, int section)
    {
        var strBuilder = new StringBuilder();

        if (section == 0)
        {
            var allowLive = guild.AllowConfiguration.AllowLive ? "🟢" : "🔴";
            var allowLiveDiscovery = guild.AllowConfiguration.AllowLiveDiscovery ? "🟢" : "🔴";
            var allowPublished = guild.AllowConfiguration.AllowPublished ? "🟢" : "🔴";
            var allowCrosspost = guild.AllowConfiguration.AllowCrosspost ? "🟢" : "🔴";
            strBuilder.AppendLine($"{allowLive} Live");
            strBuilder.AppendLine($"{allowPublished} Published");
            strBuilder.AppendLine($"{allowLiveDiscovery} Discovery");
            strBuilder.AppendLine($"{allowCrosspost} Crosspost");
        }
        else
        {
            var allowGoodbyes = guild.AllowConfiguration.AllowGoodbyes ? "🟢" : "🔴";
            var allowGreetings = guild.AllowConfiguration.AllowGreetings ? "🟢" : "🔴";
            var allowThumbnails = guild.AllowConfiguration.AllowThumbnails ? "🟢" : "🔴";
            var allowFfa = guild.AllowConfiguration.AllowFfa ? "🟢" : "🔴";

            strBuilder.AppendLine($"{allowGoodbyes} Goodbyes");
            strBuilder.AppendLine($"{allowGreetings} Greetings");
            strBuilder.AppendLine($"{allowThumbnails} Thumbnails");
            strBuilder.AppendLine($"{allowFfa} FreeForAll");
        }

        return strBuilder.ToString();
    }

    private string GetChannelConfigurationString(GuildDto guild)
    {
        var strBuilder = new StringBuilder();

        if (!string.IsNullOrEmpty(guild.ChannelConfiguration.GoodbyeChannelId))
        {
            strBuilder.AppendLine($"Goodbye: <#{guild.ChannelConfiguration.GoodbyeChannelId}>");
        }

        if (!string.IsNullOrEmpty(guild.ChannelConfiguration.GreetingChannelId))
        {
            strBuilder.AppendLine($"Greeting: <#{guild.ChannelConfiguration.GreetingChannelId}>");
        }

        if (!string.IsNullOrEmpty(guild.ChannelConfiguration.LiveChannelId))
        {
            strBuilder.AppendLine($"Discovery: <#{guild.ChannelConfiguration.LiveChannelId}>");
        }

        if (!string.IsNullOrEmpty(guild.ChannelConfiguration.DiscordLiveChannelId))
        {
            strBuilder.AppendLine($"Discord Live: <#{guild.ChannelConfiguration.DiscordLiveChannelId}>");
        }

        return strBuilder.ToString();
    }

    private string GetServerConfigurationString(GuildDto guild)
    {
        var strBuilder = new StringBuilder();

        var useTextAnnouncements = guild.GuildConfiguration.TextAnnouncements ? "🟢" : "🔴";
        var streamCleanup = guild.GuildConfiguration.DeleteOffline ? "🟢" : "🔴";

        strBuilder.AppendLine($"{useTextAnnouncements} Text Announcements");
        strBuilder.AppendLine($"{streamCleanup} Stream Cleanup");

        return strBuilder.ToString();
    }

    private string GetRoleConfigurationString(GuildDto guild)
    {
        var strBuilder = new StringBuilder();

        if (!string.IsNullOrEmpty(guild.RoleConfiguration.DiscoveryRoleId))
        {
            strBuilder.AppendLine($"Discovery Role: <@&{guild.RoleConfiguration.DiscoveryRoleId}>");
        }

        if (!string.IsNullOrEmpty(guild.RoleConfiguration.LiveDiscoveryRoleId))
        {
            strBuilder.AppendLine($"Live Role: <@&{guild.RoleConfiguration.LiveDiscoveryRoleId}>");
        }

        if (!string.IsNullOrEmpty(guild.RoleConfiguration.JoinRoleId))
        {
            strBuilder.AppendLine($"Join Role: <@&{guild.RoleConfiguration.JoinRoleId}>");
        }

        return strBuilder.ToString();
    }

    private string GetMessageConfigurationString(GuildDto guild)
    {
        var strBuilder = new StringBuilder();

        strBuilder.AppendLine($"Live: `{guild.MessageConfiguration.LiveMessage}`");
        strBuilder.AppendLine($"Published: `{guild.MessageConfiguration.PublishedMessage}`");
        strBuilder.AppendLine($"Offline: `{guild.MessageConfiguration.StreamOfflineMessage}`");
        strBuilder.AppendLine($"Greeting: `{guild.MessageConfiguration.GreetingMessage}`");
        strBuilder.AppendLine($"Goodbye: `{guild.MessageConfiguration.GoodbyeMessage}`");

        return strBuilder.ToString();
    }
}