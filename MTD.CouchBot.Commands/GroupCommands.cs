using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using MTD.CouchBot.Domain.Dtos.Discord;
using MTD.CouchBot.Domain.Utilities;
using MTD.CouchBot.Localization;
using MTD.CouchBot.Managers;

namespace MTD.CouchBot.Commands
{
    [Group("Group")]
    public class GroupCommands : Command
    {
        private readonly IConfiguration _configuration;
        private readonly List<Translation> _translations;
        private readonly IGroupManager _groupManager;
        private readonly DiscordSocketClient _discord;

        public GroupCommands(List<Translation> translations, IGuildManager guildManager, IGroupManager groupManager, IConfiguration configuration, DiscordSocketClient discord)
            : base(translations, guildManager, groupManager, configuration)
        {
            _translations = translations;
            _groupManager = groupManager;
            _discord = discord;
        }

        [Command("List")]
        public async Task List()
        {
            var translation = await GetTranslation();

            var builder = new EmbedBuilder
            {
                Description = $"{BotName} {translation.GroupCommands.List} ..."
            };

            builder.AddField("Group List", string.Join(", ", (await GetGuildGroups()).Select(gg => gg.Name)), true);

            builder.Footer = new EmbedFooterBuilder
            {
                Text =
                    $"\r\n{translation.GroupCommands.ListFooter} '{Prefix} group info \"GroupName\"'"
            };

            await Context.Channel.SendMessageAsync("", false, builder.Build());
        }

        [Command("Info")]
        public async Task Info(string groupName)
        {
            if (!IsOwner)
            {
                return;
            }
            var translation = await GetTranslation();
            var guildGroup = await _groupManager.GetGuildGroupByGuildIdAndName(Context.Guild.Id, groupName);

            if (guildGroup == null)
            {
                await Context.Channel.SendMessageAsync($"{translation.GroupCommands.InvalidGroupName} '{Prefix} group list'");
            }

            var mentionRole = string.IsNullOrEmpty(guildGroup.MentionRoleId)
                ? "Everyone"
                : _discord.GetGuild(Context.Guild.Id)
                    ?.GetRole(ulong.Parse(Cryptography.Decrypt(guildGroup.MentionRoleId)))?.Name;
            var liveChannel = string.IsNullOrEmpty(guildGroup.StreamChannelId)
                ? "Not Set"
                : _discord.GetGuild(Context.Guild.Id)
                    ?.GetChannel(ulong.Parse(Cryptography.Decrypt(guildGroup.StreamChannelId)))?.Name;
            var vodChannel = string.IsNullOrEmpty(guildGroup.VodChannelId)
                ? "Not Set"
                : _discord.GetGuild(Context.Guild.Id)
                    ?.GetChannel(ulong.Parse(Cryptography.Decrypt(guildGroup.VodChannelId)))?.Name;
            var liveMessage = string.IsNullOrEmpty(guildGroup.LiveMessage)
                ? translation.Defaults.LiveMessage
                : guildGroup.LiveMessage;
            var vodMessage = string.IsNullOrEmpty(guildGroup.VodMessage)
                ? translation.Defaults.VodMessage
                : guildGroup.VodMessage;


            var details = new StringBuilder();
            details.AppendLine($"**{translation.Labels.Name}:** {guildGroup?.Name}");
            details.AppendLine($"**{translation.Labels.MentionRole}:** {mentionRole}");
            details.AppendLine($"**{translation.Labels.LiveChannel}:** {liveChannel}");
            details.AppendLine($"**{translation.Labels.VodChannel}:** {vodChannel}");
            details.AppendLine($"**{translation.Labels.LiveMessage}:** {liveMessage}");
            details.AppendLine($"**{translation.Labels.VodMessage}:** {vodMessage}");
            
            var builder = new EmbedBuilder();
            builder.AddField($"{translation.Labels.Details}",details.ToString());
            var footer = new EmbedFooterBuilder();
            footer.Text = $"{translation.GroupCommands.InfoFooter} '{Prefix} group list'";
            builder.Footer = footer;

            await Context.Channel.SendMessageAsync("", false, builder.Build());
        }

        [Command("Add")]
        public async Task Add(string groupName)
        {
            var translation = await GetTranslation();

            if(!IsOwner)
            {
                return;
            }

            if(groupName.Equals("Default", StringComparison.CurrentCultureIgnoreCase))
            {
                await Context.Channel.SendMessageAsync(translation.GroupCommands.NoCreateDefault);
                return;
            }

            var guildGroup = await _groupManager.GetGuildGroupByGuildIdAndName(Context.Guild.Id, groupName);

            if(guildGroup != null)
            {
                await Context.Channel.SendMessageAsync(translation.GroupCommands.NoCreateExists);
            }

            guildGroup = new GuildGroup
            {
                GuildId = Cryptography.Encrypt(Context.Guild.Id.ToString()),
                Name = groupName,
                StreamChannelId = null,
                VodChannelId = null,
                MentionRoleId = null,
                LiveMessage = translation.Defaults.LiveMessage,
                VodMessage = translation.Defaults.VodMessage
            };

            await _groupManager.CreateGuildGroup(guildGroup);
            await Context.Channel.SendMessageAsync(translation.GroupCommands.CreatedSuccessfully);
        }

        [Command("Remove")]
        public async Task Remove(string groupName)
        {
            var translation = await GetTranslation();

            if (!IsOwner)
            {
                return;
            }

            if (groupName.Equals("Default", StringComparison.CurrentCultureIgnoreCase))
            {
                await Context.Channel.SendMessageAsync(translation.GroupCommands.DefaultNoRemove);
                return;
            }

            var guildGroup = await _groupManager.GetGuildGroupByGuildIdAndName(Context.Guild.Id, groupName);

            if (guildGroup == null)
            {
                await Context.Channel.SendMessageAsync($"{translation.GroupCommands.InvalidGroupName} '{Prefix} group list'");
                return;
            }

            await _groupManager.DeleteGuildGroup(guildGroup);
            await Context.Channel.SendMessageAsync(translation.GroupCommands.RemovedSuccessfully);
        }
    }
}
