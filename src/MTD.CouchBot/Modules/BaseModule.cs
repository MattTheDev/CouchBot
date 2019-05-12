using Discord;
using Discord.Commands;
using Microsoft.Extensions.Options;
using MTD.CouchBot.Domain.Models.Bot;
using MTD.CouchBot.Services;
using Newtonsoft.Json;
using System.IO;

namespace MTD.CouchBot.Modules
{
    public class BaseModule : ModuleBase
    {
        private readonly BotSettings _botSettings;
        private readonly FileService _fileService;
        
        public BaseModule(IOptions<BotSettings> botSettings, FileService fileService)
        {
            _botSettings = botSettings.Value;
            _fileService = fileService;
        }

        public bool IsBotOwner
        {
            get
            {
                var authorId = ((IGuildUser)Context.Message.Author).Id;

                if (authorId != _botSettings.BotConfig.OwnerId)
                {
                    return false;
                }

                return true;
            }
        }

        public bool IsAdmin
        {
            get
            {
                var user = ((IGuildUser)Context.Message.Author);

                if (!user.GuildPermissions.ManageGuild)
                {
                    return false;
                }

                return true;
            }
        }

        public bool IsApprovedAdmin
        {
            get
            {
                var file = _botSettings.DirectorySettings.ConfigRootDirectory + _botSettings.DirectorySettings.GuildDirectory +
                    Context.Guild.Id + ".json";
                DiscordServer server = null;

                if (File.Exists(file))
                    server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

                if (server == null)
                {
                    return false;
                }

                if ((server.ApprovedAdmins != null && server.ApprovedAdmins.Contains(Context.User.Id)) || IsAdmin)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool BotHasManageRoles
        {
            get
            {
                var user = (Context.Guild.GetUserAsync(Context.Client.CurrentUser.Id).Result);

                if (user == null)
                {
                    return false;
                }

                return user.GuildPermissions.ManageRoles;
            }
        }

        public DiscordServer GetServer()
        {
            return _fileService.GetConfiguredServerById(Context.Guild.Id);
        }
    }
}
