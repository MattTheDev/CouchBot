using Discord;
using Discord.Commands;
using Microsoft.Extensions.Configuration;
using MTD.CouchBot.Localization;
using MTD.CouchBot.Managers;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MTD.CouchBot.Commands
{
    [Group("Language")]
    public class LanguageCommands : Command
    {
        private readonly IGuildManager _guildManager;
        private readonly List<Translation> _translations;
        private readonly IConfiguration _configuration;

        public LanguageCommands(List<Translation> translations, IGuildManager guildManager, IGroupManager groupManager, IConfiguration configuration) : base(translations, guildManager, groupManager, configuration)
        {
            _guildManager = guildManager;
            _configuration = configuration;
            _translations = translations;
        }


        [Command("Set")]
        public async Task Set(string languageCode)
        {
            if (!IsOwner)
            {
                return;
            }

            var guildConfiguration = await GetGuildConfiguration();

            guildConfiguration.LanguageCode = languageCode;
            await _guildManager.UpdateGuildConfiguration(guildConfiguration);
            await Context.Channel.SendMessageAsync($"{(await GetTranslation()).SetCommands.Language}: {languageCode}");
        }

        [Command("List")]
        public async Task List()
        {
            var translation = await GetTranslation();

            var builder = new EmbedBuilder
            {
                Description = $"{BotName} {translation.LanguageCommands.List} ..."
            };

            builder.AddField("Language", string.Join("\r\n", _translations.Select(t => t.Language)), true);
            builder.AddField("Language Code", string.Join("\r\n", _translations.Select(t => t.LanguageCode)), true);
            builder.Footer = new EmbedFooterBuilder
            {
                Text =
                    $"\r\n{translation.LanguageCommands.ListFooter} '{Prefix} language set languageCode'"
            };

            await Context.Channel.SendMessageAsync("", false, builder.Build());
        }
    }
}