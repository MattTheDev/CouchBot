using Discord;
using Discord.Commands;
using Microsoft.Extensions.Options;
using MTD.CouchBot.Domain.Models.Bot;
using Newtonsoft.Json;
using System.IO;
using System.Linq;

namespace MTD.CouchBot.Modules
{
    public class BaseModule : ModuleBase
    {
        private readonly BotSettings _botSettings;
        
        public BaseModule(IOptions<BotSettings> botSettings)
        {
            _botSettings = botSettings.Value;
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
    }
}
