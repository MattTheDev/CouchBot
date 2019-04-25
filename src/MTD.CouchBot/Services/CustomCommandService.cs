using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using MTD.CouchBot.Domain.Enums;
using MTD.CouchBot.Domain.Models.Bot;

namespace MTD.CouchBot.Services
{
    public class CustomCommandService
    {
        private readonly FileService _fileService;
        private readonly StringService _stringService;
        private readonly BotSettings _botSettings;
        private readonly GuildInteractionService _guildInteractionService;
        private readonly DiscordShardedClient _discord;

        public CustomCommandService(FileService fileService, 
            StringService stringService, 
            IOptions<BotSettings> botSettings, 
            GuildInteractionService guildInteractionService,
            DiscordShardedClient discord)
        {
            _fileService = fileService;
            _stringService = stringService;
            _botSettings = botSettings.Value;
            _guildInteractionService = guildInteractionService;
            _discord = discord;
        }

        private async Task ProcessCommand(DiscordServer server, CustomCommand c, int cooldown, SocketCommandContext context = null)
        {
            var ts = DateTime.UtcNow - c.LastRun;

            if (ts.TotalSeconds > cooldown)
            {
                c.LastRun = DateTime.UtcNow;

                if (context != null)
                {
                    await context.Channel.SendMessageAsync(await _stringService.CommandText(c.Output, context.Guild));

                    await _fileService.SaveDiscordServer(server, context.Guild);
                }
                else
                {
                    var channel = (IMessageChannel) _discord.GetChannel(c.ChannelId);
                    channel.SendMessageAsync(c.Output).Wait();

                    _fileService.SaveDiscordServer(server);
                }
            }
        }

        public async Task ProcessCustomCommands(DiscordServer server, SocketUserMessage msg, SocketCommandContext context)
        {
            if (server.CustomCommands == null)
            {
                return;
            }

            foreach (var c in server.CustomCommands)
            {
                var command = msg.Content.Replace(_botSettings.BotConfig.Prefix, "");

                if (c.Command.Equals(command, StringComparison.CurrentCultureIgnoreCase))
                {
                    await ProcessCommand(server, c, c.Cooldown, context);
                }
            }
        }

        public async Task ProcessRepeatedCustomCommand(DiscordServer server)
        {
            foreach (var c in server.CustomCommands)
            {
                if (c.Repeat)
                {
                    await ProcessCommand(server, c, c.Interval);
                }
            }
        }

        public async Task ProcessRoleCommands(DiscordServer server, SocketUserMessage msg, SocketCommandContext context)
        {

            if (server.RoleCommands == null)
            {
                return;
            }

            foreach (var role in server.RoleCommands)
            {
                var phrase = msg.Content.Replace(_botSettings.BotConfig.Prefix, "");

                if (phrase.Equals(role.Phrase, StringComparison.CurrentCultureIgnoreCase))
                {
                    var guildRole = context.Guild.Roles.FirstOrDefault(r => r.Id == role.RoleId);
                    var user = ((IGuildUser)context.User);

                    if (role.Function == RoleFunction.Add)
                    {
                        if (user.RoleIds.Contains(guildRole.Id))
                        {
                            await context.Channel.SendMessageAsync("You are already in that role.");
                        }
                        else
                        {
                            await _guildInteractionService.AddRole(user, guildRole, context.Channel);
                        }
                    }

                    if (role.Function == RoleFunction.Remove)
                    {
                        if (!user.RoleIds.Contains(guildRole.Id))
                        {
                            await context.Channel.SendMessageAsync("You aren't in that role..");
                        }
                        else
                        {
                            await _guildInteractionService.RemoveRole(user, guildRole, context.Channel);
                        }
                    }
                }
            }
        }
    }
}