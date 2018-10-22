using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace MTD.CouchBot.Services
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private readonly IServiceProvider _provider;
        private readonly IConfiguration _configuration;

        public CommandHandler(
            DiscordSocketClient discord,
            CommandService commands,
            IServiceProvider provider, IConfiguration configuration)
        {
            _discord = discord;
            _commands = commands;
            _provider = provider;
            _configuration = configuration;

            _discord.MessageReceived += OnMessageReceivedAsync;
        }

        private async Task OnMessageReceivedAsync(SocketMessage s)
        {
            if (!(s is SocketUserMessage msg))
            {
                return;
            }

            if (msg.Author.Id == _discord.CurrentUser.Id)
            {
                return;
            }

            var context = new SocketCommandContext(_discord, msg);

            var argPos = 0;
            if (msg.HasStringPrefix($"{_configuration["Prefix"]} ", ref argPos))
            {
                var result = await _commands.ExecuteAsync(context, argPos, _provider);

                if (!result.IsSuccess)
                {
                    await context.Channel.SendMessageAsync(result.ToString());
                }
            }
        }
    }
}
