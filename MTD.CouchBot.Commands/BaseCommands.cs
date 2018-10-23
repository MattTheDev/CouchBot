using Discord.Commands;
using Microsoft.Extensions.Configuration;
using MTD.CouchBot.Domain.Dtos.Discord;
using MTD.CouchBot.Domain.Utilities;
using MTD.CouchBot.Localization;
using MTD.CouchBot.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTD.CouchBot.Commands
{
    public class BaseCommands : Command
    {
        private readonly IGuildManager _guildManager;
        private readonly IConfiguration _configuration;
        private List<Translation> _translations;

        public BaseCommands(List<Translation> translations, IGuildManager guildManager, IGroupManager groupManager, IConfiguration configuration) : base(translations, guildManager, groupManager, configuration)
        {
            _configuration = configuration;
            _guildManager = guildManager;
            _translations = translations;
        }

        public bool IsOwner => Context.Message.Author.Id != ulong.Parse(_configuration["Ids:OwnerId"]);

        [Command("Ping")]
        public async Task Ping()
        {
            await Context.Channel.SendMessageAsync(_translations.FirstOrDefault(t => t.LanguageCode.Equals("en-US")).Defaults.Pong);
        }

        [Command("Hello")]
        public async Task Hello()
        {
            var guildConfiguration = await _guildManager.GetGuildConfigurationByGuildId(Context.Guild.Id);
            var translation = _translations.FirstOrDefault(t => t.LanguageCode.Equals(guildConfiguration.LanguageCode));
            await Context.Channel.SendMessageAsync(translation.Defaults.Greeting);
        }

        [Command("Test")]
        public async Task Test()
        {
            await _guildManager.CreateGuild(new Guild()
            {
                GuildId = MTD.CouchBot.Domain.Utilities.Cryptography.Encrypt(Context.Guild.Id.ToString()),
                OwnerId = Cryptography.Encrypt(Context.Guild.OwnerId.ToString()),
                CreatedDate = DateTime.UtcNow
            });
        }
    }
}
