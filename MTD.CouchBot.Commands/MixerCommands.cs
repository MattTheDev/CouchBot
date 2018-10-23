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
    [Group("Mixer")]
    public class MixerCommands : Command
    {
        private readonly IMixerManager _mixerManager;

        public MixerCommands(List<Translation> translations, IGuildManager guildManager, 
            IGroupManager groupManager, IConfiguration configuration, IMixerManager mixerManager) : base(translations, guildManager, groupManager, configuration)
        {
            _mixerManager = mixerManager;
        }

        [Command("Lookup")]
        public async Task Lookup(string channelName)
        {
            var stringBuilder = new StringBuilder();
            var response = await _mixerManager.GetMixerChannelByChannelName(channelName);

            if(response != null)
            {
                stringBuilder.AppendLine($"Name: {response._User.Username}");
                stringBuilder.AppendLine($"View Count: {response.ViewersTotal}");
                stringBuilder.AppendLine($"Follower Count: {response.NumFollowers}");
            }
            else
            {
                stringBuilder.AppendLine("Sorry, a Mixer channel with that name does not exist.");
            }

            var builder = new EmbedBuilder();
            builder.Description = stringBuilder.ToString();

            await Context.Channel.SendMessageAsync("", false, builder.Build());
        }
    }
}
