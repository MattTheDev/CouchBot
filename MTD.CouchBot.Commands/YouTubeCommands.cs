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
    [Group("YouTube")]
    public class YouTubeCommands : Command
    {
        private readonly IYouTubeManager _youTubeManager;

        public YouTubeCommands(List<Translation> translations, IGuildManager guildManager, IGroupManager groupManager, 
            IConfiguration configuration, IYouTubeManager youTubeManager) : base(translations, guildManager, groupManager, configuration)
        {
            _youTubeManager = youTubeManager;
        }

        [Command("Lookup")]
        public async Task Lookup(string channelId)
        {
            var stringBuilder = new StringBuilder();
            var response = await _youTubeManager.GetYouTubeChannelByChannelId(channelId);

            if (response?.Items != null && response.Items.Count > 0)
            {
                stringBuilder.AppendLine($"Name: {response.Items[0].Snippet.Title}");

                int viewCount = response.Items[0].Statistics == null ? 0 : int.Parse(response.Items[0].Statistics.ViewCount);

                stringBuilder.AppendLine($"View Count: {viewCount}");
            }
            else
            {
                stringBuilder.AppendLine("Sorry, a YouTube channel with that ID does not exist.");
            }

            var builder = new EmbedBuilder();
            builder.Description = stringBuilder.ToString();

            await Context.Channel.SendMessageAsync("", false, builder.Build());
        }
    }
}