using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.Extensions.Configuration;
using MTD.CouchBot.Domain.Dtos.Discord;
using MTD.CouchBot.Localization;
using MTD.CouchBot.Managers;

namespace MTD.CouchBot.Commands
{
    public class Command : ModuleBase
    {
        private readonly IConfiguration _configuration;
        private readonly List<Translation> _translations;
        private readonly IGuildManager _guildManager;
        private readonly IGroupManager _groupManager;

        public Command(List<Translation> translations, IGuildManager guildManager, IGroupManager groupManager, IConfiguration configuration)
        {
            _translations = translations;
            _guildManager = guildManager;
            _groupManager = groupManager;
            _configuration = configuration;
        }
        
        public bool IsOwner => Context.Guild.OwnerId == Context.Message.Author.Id;
        public string Prefix => _configuration["Prefix"];
        public string BotName => _configuration["Name"];

        public async Task<Guild> GetGuild()
        {
            return await _guildManager.GetGuildById(Context.Guild.Id);
        }

        public async Task<GuildConfiguration> GetGuildConfiguration()
        {
            return await _guildManager.GetGuildConfigurationByGuildId(Context.Guild.Id);
        }

        public async Task<List<GuildGroup>> GetGuildGroups()
        {
            return await _groupManager.GetGuildGroupsByGuildId(Context.Guild.Id);
        }

        public async Task<Translation> GetTranslation()
        {
            var languageCode = (await GetGuildConfiguration()).LanguageCode;

            return _translations.FirstOrDefault(t => t.LanguageCode == languageCode);
        }
    }
}