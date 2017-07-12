using Discord;
using Discord.Commands;
using MTD.CouchBot.Domain;
using MTD.CouchBot.Domain.Models.Bot;
using MTD.CouchBot.Domain.Utilities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MTD.CouchBot.Modules
{
    [Group("admin")]
    public class Admin : ModuleBase
    {
        [Command("add")]
        public async Task Add(IGuildUser user)
        {
            var guild = ((IGuildUser)Context.Message.Author).Guild;

            var authorUser = ((IGuildUser)Context.Message.Author);

            if (!authorUser.GuildPermissions.ManageGuild)
            {
                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
            {
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));
            }

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
            await BotFiles.SaveDiscordServer(server, Context.Guild);

            await Context.Channel.SendMessageAsync(user.Username + " has been added to the approved admin list for this server.");
        }

        [Command("remove")]
        public async Task Remove(IGuildUser user)
        {
            var guild = ((IGuildUser)Context.Message.Author).Guild;

            var authorUser = ((IGuildUser)Context.Message.Author);

            if (!authorUser.GuildPermissions.ManageGuild)
            {
                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
            {
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));
            }

            if (server.ApprovedAdmins == null || !server.ApprovedAdmins.Contains(user.Id))
            {
                await Context.Channel.SendMessageAsync(user.Username + " is not on the approved admins list for this server.");

                return;
            }

            server.ApprovedAdmins.Remove(user.Id);
            await BotFiles.SaveDiscordServer(server, Context.Guild);

            await Context.Channel.SendMessageAsync(user.Username + " has been removed from the approved admin list for this server.");
        }

        [Command("list")]
        public async Task List()
        {
            var guild = ((IGuildUser)Context.Message.Author).Guild;

            var authorUser = ((IGuildUser)Context.Message.Author);

            if (!authorUser.GuildPermissions.ManageGuild)
            {
                return;
            }

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
            {
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));
            }

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

            string info = "```Markdown\r\n" +
              "# Server Approved Admins\r\n" +
              admins + "\r\n" +
              "```\r\n";

            await Context.Channel.SendMessageAsync(info);
        }
    }
}
