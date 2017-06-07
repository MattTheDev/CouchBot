using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace MTD.CouchBot.Modules
{
    [Group("purge")]
    public class Purge : ModuleBase
    {
        [Command("bot")]
        public async Task Bot(int count)
        {
            var guild = ((IGuildUser)Context.Message.Author).Guild;

            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }
        }
    }
}
