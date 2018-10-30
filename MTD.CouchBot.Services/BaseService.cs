using Discord.WebSocket;

namespace MTD.CouchBot.Services
{
    public class BaseService
    {
        private readonly DiscordSocketClient _discord;

        public BaseService(DiscordSocketClient discord)
        {
            _discord = discord;
        }

        public string BotName => _discord.CurrentUser.Username;
    }
}