using Discord;
using Discord.Commands;
using Microsoft.Extensions.Options;
using MTD.CouchBot.Domain.Enums;
using MTD.CouchBot.Domain.Models.Bot;
using MTD.CouchBot.Services;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTD.CouchBot.Modules
{
    [Group("Role")]
    public class Role : BaseModule
    {
        private readonly FileService _fileService;

        public Role(IOptions<BotSettings> botSettings, FileService fileService) : base(botSettings)
        {
            _fileService = fileService;
        }

        [Command("Add")]
        public async Task Add(string phrase, RoleFunction function, IRole role)
        {
            if (!IsAdmin)
            {
                return;
            }

            if (!BotHasManageRoles)
            {
                await Context.Channel.SendMessageAsync(
                    "Sorry, I don't have 'Manage Roles' permission. I won't be able to assign roles. Fix that, and try again.");
                return;
            }

            var server = _fileService.GetConfiguredServerById(Context.Guild.Id);

            if (server.RoleCommands == null)
            {
                server.RoleCommands = new List<RoleCommand>();
            }

            server.RoleCommands.Add(new RoleCommand()
            {
                Phrase = phrase,
                RoleId = role.Id,
                Function = function
            });

            _fileService.SaveDiscordServer(server);
            await Context.Channel.SendMessageAsync(
                $"Your auto role phrase has been added. When people use the phrase '{phrase}' they will be assigned the {role.Mention} role.");
        }

        [Command("Remove")]
        public async Task Remove(string phrase)
        {
            if (!IsAdmin)
            {
                return;
            }

            var server = _fileService.GetConfiguredServerById(Context.Guild.Id);

            if (server.RoleCommands == null)
            {
                await Context.Channel.SendMessageAsync("Sorry, you don't have any auto role phrases setup.");
                return;
            }

            var roleCommand = server.RoleCommands.FirstOrDefault(x => x.Phrase.Equals(phrase));
            server.RoleCommands.Remove(roleCommand);

            _fileService.SaveDiscordServer(server);
            await Context.Channel.SendMessageAsync(
                "Your auto role phrase has been removed.");
        }

        [Command("List")]
        public async Task List()
        {
            if (!IsAdmin)
            {
                return;
            }

            var server = _fileService.GetConfiguredServerById(Context.Guild.Id);
            var phrases = new StringBuilder();
            var functions = new StringBuilder();
            var roles = new StringBuilder();

            var toRemove = new List<RoleCommand>();

            foreach (var rp in server.RoleCommands)
            {
                var role = Context.Guild.Roles.FirstOrDefault(r => r.Id == rp.RoleId);

                if (role == null)
                {
                    roles.Append("Removed. Role invalid.");
                    toRemove.Add(rp);
                }
                else
                {
                    roles.Append($"{role.Name}\r\n");
                }

                phrases.Append($"{rp.Phrase}\r\n");
                functions.Append($"{rp.Function}\r\n");
            }

            foreach (var role in toRemove)
            {
                server.RoleCommands.Remove(role);
            }

            _fileService.SaveDiscordServer(server);

            var embedBuilder = new EmbedBuilder {Description = "Your current auto role phrases... "};

            if (!string.IsNullOrEmpty(phrases.ToString()))
            {
                embedBuilder.Fields.Add(
                    new EmbedFieldBuilder()
                    {
                        IsInline = true,
                        Name = "Phrases",
                        Value = phrases.ToString()
                    });

                embedBuilder.Fields.Add(
                    new EmbedFieldBuilder()
                    {
                        IsInline = true,
                        Name = "Function",
                        Value = functions.ToString()
                    });

                embedBuilder.Fields.Add(
                    new EmbedFieldBuilder()
                    {
                        IsInline = true,
                        Name = "Role",
                        Value = roles.ToString()
                    });
            }
            else
            {
                embedBuilder.Fields.Add(
                    new EmbedFieldBuilder()
                    {
                        IsInline = false,
                        Name = "Phrases",
                        Value = "You currently have no phrases created."
                    });
            }

            await Context.Channel.SendMessageAsync("", false, embedBuilder.Build());
        }
    }
}