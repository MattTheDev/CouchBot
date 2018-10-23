using Discord;
using Discord.Commands;
using Microsoft.Extensions.Configuration;
using MTD.CouchBot.Localization;
using MTD.CouchBot.Managers;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MTD.CouchBot.Commands
{
    [Group("Twitch")]
    public class TwitchCommands : Command
    {
        private readonly ITwitchManager _twitchManager;

        public TwitchCommands(List<Translation> translations, IGuildManager guildManager, 
            IGroupManager groupManager, IConfiguration configuration, ITwitchManager twitchManager) : base(translations, guildManager, groupManager, configuration)
        {
            _twitchManager = twitchManager;
        }

        [Command("Lookup")]
        public async Task Lookup(string loginName)
        {
            var stringBuilder = new StringBuilder();
            var response = await _twitchManager.GetTwitchUserByLoginName(loginName);

            if(response != null)
            {
                stringBuilder.AppendLine($"Name: {response.Users[0].DisplayName}");
                stringBuilder.AppendLine($"Login: {response.Users[0].Login}");
                stringBuilder.AppendLine($"View Count: {response.Users[0].ViewCount}");
            }
            else
            {
                stringBuilder.AppendLine("Sorry, a Twitch channel with that name does not exist.");
            }

            var builder = new EmbedBuilder();
            builder.Description = stringBuilder.ToString();

            await Context.Channel.SendMessageAsync("", false, builder.Build());
        }
    }
}
