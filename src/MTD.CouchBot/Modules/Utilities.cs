using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using MTD.CouchBot.Domain.Models.Bot;
using MTD.CouchBot.Services;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MTD.CouchBot.Modules
{
    [Group("Utilities")]
    public class Utilities : BaseModule
    {
        private readonly DiscordShardedClient _discord;
        private readonly IOptions<BotSettings> _botSettings;
        private readonly FileService _fileService;

        public Utilities(IOptions<BotSettings> botSettings, DiscordShardedClient discord, FileService fileService) :
            base(botSettings)
        {
            _discord = discord;
            _botSettings = botSettings;
            _fileService = fileService;
        }
    }
}