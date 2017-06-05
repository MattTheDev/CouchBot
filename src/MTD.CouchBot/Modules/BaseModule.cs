using Discord;
using Discord.Commands;
using MTD.CouchBot.Domain;
using MTD.CouchBot.Domain.Models.Bot;
using Newtonsoft.Json;
using System.IO;

namespace MTD.CouchBot.Modules
{
    public class BaseModule : ModuleBase
    {
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
                var file = Constants.ConfigRootDirectory + Constants.GuildDirectory +
                    Context.Guild.Id + ".json";

                DiscordServer server = null;

                if (File.Exists(file))
                {
                    server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));
                }

                if(server == null)
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
    }
}
