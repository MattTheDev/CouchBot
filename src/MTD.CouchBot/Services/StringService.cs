using Discord;
using System.Threading.Tasks;

namespace MTD.CouchBot.Services
{
    public class StringService
    {
        private readonly DiscordService _discordService;

        public StringService (DiscordService discordService)
        {
            _discordService = discordService;
        }

        public async Task<string> CommandText(string input, IGuild guild)
        {
            return input.Replace("%RANDOMUSER%", await _discordService.GetRandomGuildUser(guild));
        }

        public string AnnouncementText(string input, string channelName, string streamTitle, string streamUrl, string youTubeLiveUrl)
        {
            return input.Replace("%CHANNEL%", Format.Sanitize(channelName))
                .Replace("%TITLE%", streamTitle)
                .Replace("%URL%", streamUrl)
                .Replace("%YTLIVEURL%", youTubeLiveUrl);
        }
    }
}
