using Discord;
using Discord.Commands;
using Microsoft.Extensions.Options;
using MTD.CouchBot.Domain.Models.Bot;
using MTD.CouchBot.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MTD.CouchBot.Modules
{
    [Group("admin")]
    [Alias("approvedadmin")]
    public class Admin : BaseModule
    {
        private readonly FileService _fileService;

        public Admin(IOptions<BotSettings> botSettings, FileService fileService) : base (botSettings, fileService)
        {
            _fileService = fileService;
        }

        [Command("add")]
        [Alias("+")]
        public async Task Add(IGuildUser user)
        {
            if (!IsAdmin)
            {
                return;
            }

            var server = GetServer();

            if (server.ApprovedAdmins == null)
            {
                server.ApprovedAdmins = new List<ulong>();
            }

            if (server.ApprovedAdmins.Contains(user.Id))
            {
                await Context.Channel.SendMessageAsync($"{user.Username} is already on the approved admins list for this server.");
                return;
            }

            server.ApprovedAdmins.Add(user.Id);
            await _fileService.SaveDiscordServer(server, Context.Guild);

            await Context.Channel.SendMessageAsync($"{user.Username} has been added to the approved admin list for this server.");
        }

        [Command("remove")]
        [Alias("-")]
        public async Task Remove(IGuildUser user)
        {
            if (!IsAdmin)
            {
                return;
            }

            var server = GetServer();

            if (server.ApprovedAdmins == null || !server.ApprovedAdmins.Contains(user.Id))
            {
                await Context.Channel.SendMessageAsync($"{user.Username} is not on the approved admins list for this server.");
                return;
            }

            server.ApprovedAdmins.Remove(user.Id);
            await _fileService.SaveDiscordServer(server, Context.Guild);

            await Context.Channel.SendMessageAsync($"{user.Username} has been removed from the approved admin list for this server.");
        }

        [Command("list")]
        public async Task List()
        {
            if (!IsAdmin)
            {
                return;
            }

            var server = GetServer();

            var admins = new List<string>();
            if (server.ApprovedAdmins == null || server.ApprovedAdmins.Count == 0)
            {
                admins.Add("There are currently no approved admins.");
            }
            else
            {
                foreach (var aa in server.ApprovedAdmins)
                {
                    var user = await Context.Guild.GetUserAsync(aa);
                    admins.Add(user.Nickname ?? user.Username);
                }
            }

            var info = $"```Markdown\r\n# Server Approved Admins\r\n{string.Join(", ", admins)}\r\n```\r\n";

            await Context.Channel.SendMessageAsync(info);
        }
    }
}
