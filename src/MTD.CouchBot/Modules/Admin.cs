using Discord;
using Discord.Commands;
using Microsoft.Extensions.Options;
using MTD.CouchBot.Domain.Models.Bot;
using MTD.CouchBot.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using MTD.CouchBot.Domain.Utilities;

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

            if (server.Admins.Users == null)
            {
                server.Admins.Users = new List<ulong>();
            }

            if (server.Admins.Users.Contains(user.Id))
            {
                await Context.Channel.SendMessageAsync($"{user.Username} is already on the approved admins list for this server.");
                return;
            }

            server.Admins.Users.Add(user.Id);
            await _fileService.SaveDiscordServer(server, Context.Guild);

            await Context.Channel.SendMessageAsync($"{user.Username} has been added to the approved admin list for this server.");
        }

        [Command("add")]
        [Alias("+")]
        public async Task Add(IRole role)
        {
            if (!IsAdmin)
            {
                return;
            }

            var server = GetServer();

            if (server.Admins == null)
            {
                server.Admins = new Domain.Models.Bot.Admin()
                {
                    Users = new List<ulong>(),
                    Roles = new List<ulong>()
                };
            }

            if (server.Admins.Roles == null)
            {
                server.Admins.Roles = new List<ulong>();
            }

            if (server.Admins.Roles.Contains(role.Id))
            {
                await Context.Channel.SendMessageAsync($"{role.Name} is already on the admins list for this server.");
                return;
            }

            server.Admins.Roles.Add(role.Id);
            await _fileService.SaveDiscordServer(server, Context.Guild);

            await Context.Channel.SendMessageAsync($"{role.Name} has been added to the approved admin list for this server.");
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

            if (server.Admins == null || !server.Admins.Users.Contains(user.Id))
            {
                await Context.Channel.SendMessageAsync($"{user.Username} is not on the approved admins list for this server.");
                return;
            }

            server.Admins.Users.Remove(user.Id);
            await _fileService.SaveDiscordServer(server, Context.Guild);

            await Context.Channel.SendMessageAsync($"{user.Username} has been removed from the approved admin list for this server.");
        }

        [Command("remove")]
        [Alias("-")]
        public async Task Remove(IRole role)
        {
            if (!IsAdmin)
            {
                return;
            }

            var server = GetServer();

            if (server.Admins?.Roles == null || !server.Admins.Roles.Contains(role.Id))
            {
                await Context.Channel.SendMessageAsync($"{role.Name} is not on the admins list for this server.");
                return;
            }

            server.Admins.Roles.Remove(role.Id);
            await _fileService.SaveDiscordServer(server, Context.Guild);

            await Context.Channel.SendMessageAsync($"{role.Name} has been removed from the approved admin list for this server.");
        }

        [Command("list")]
        public async Task List()
        {
            if (!IsAdmin)
            {
                return;
            }

            var server = GetServer();

            var builder = new EmbedBuilder();
            var authorBuilder = new EmbedAuthorBuilder {Name = "Server Administrators of CouchBot"};

            builder.Author = authorBuilder;
            builder.Color = DiscordUtilities.GetRandomColor();

            var userList = new List<string>();
            var roleList = new List<string>();

            if (server.Admins != null)
            {
                if (server.Admins.Users == null || server.Admins.Users.Count == 0)
                {
                    userList.Add("None");
                }
                else
                {
                    foreach (var userId in server.Admins.Users)
                    {
                        var user = await Context.Guild.GetUserAsync(userId);
                        userList.Add(user.Nickname ?? user.Username);
                    }
                }

                if (server.Admins.Roles == null || server.Admins.Roles.Count == 0)
                {
                    roleList.Add("None");
                }
                else
                {
                    foreach (var roleId in server.Admins.Roles)
                    {
                        var role = Context.Guild.GetRole(roleId);
                        roleList.Add(role.Name);
                    }
                }
            }
            else
            {
                userList.Add("None");
                roleList.Add("None");
            }

            builder.AddField(
                new EmbedFieldBuilder
                {
                    IsInline = true,
                    Name = "Users",
                    Value = string.Join("\r\n", userList)
                }
                );

            builder.AddField(
                new EmbedFieldBuilder
                {
                    IsInline = true,
                    Name = "Roles",
                    Value = string.Join("\r\n", roleList)
                }
            );

            await Context.Channel.SendMessageAsync("", false, builder.Build());
        }
    }
}
