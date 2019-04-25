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
    [Alias("a")]
    public class Admin : BaseModule
    {
        private readonly FileService _fileService;

        public Admin(IOptions<BotSettings> botSettings, FileService fileService) : base (botSettings)
        {
            _fileService = fileService;
        }

        [Command("add")]
        public async Task Add(IGuildUser user)
        {
            if(!IsAdmin)
            {
                return;
            }

            var server = _fileService.GetConfiguredServerById(Context.Guild.Id);

            if (server.ApprovedAdmins == null)
            {
                server.ApprovedAdmins = new List<ulong>();
            }

            if(server.ApprovedAdmins.Contains(user.Id))
            {
                await Context.Channel.SendMessageAsync(user.Username + " is already on the approved admins list for this server.");

                return;
            }

            server.ApprovedAdmins.Add(user.Id);
            await _fileService.SaveDiscordServer(server, Context.Guild);

            await Context.Channel.SendMessageAsync(user.Username + " has been added to the approved admin list for this server.");
        }

        [Command("remove")]
        public async Task Remove(IGuildUser user)
        {
            if (!IsAdmin)
            {
                return;
            }

            var server = _fileService.GetConfiguredServerById(Context.Guild.Id);

            if (server.ApprovedAdmins == null || !server.ApprovedAdmins.Contains(user.Id))
            {
                await Context.Channel.SendMessageAsync(user.Username + " is not on the approved admins list for this server.");

                return;
            }

            server.ApprovedAdmins.Remove(user.Id);
            await _fileService.SaveDiscordServer(server, Context.Guild);

            await Context.Channel.SendMessageAsync(user.Username + " has been removed from the approved admin list for this server.");
        }

        [Command("list")]
        public async Task List()
        {
            if (!IsAdmin)
            {
                return;
            }

            var server = _fileService.GetConfiguredServerById(Context.Guild.Id);

            if (server.ApprovedAdmins == null)
            {
                server.ApprovedAdmins = new List<ulong>();
            }

            var admins = "";

            foreach (var aa in server.ApprovedAdmins)
            {
                var user = await Context.Guild.GetUserAsync(aa);
                admins += user.Username + ", ";
            }

            admins = admins.Trim().TrimEnd(',');

            var info = "```Markdown\r\n" +
              "# Server Approved Admins\r\n" +
              admins + "\r\n" +
              "```\r\n";

            await Context.Channel.SendMessageAsync(info);
        }
    }
}
