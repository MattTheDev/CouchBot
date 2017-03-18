using Discord;
using Discord.Commands;
using MTD.CouchBot.Domain;
using MTD.CouchBot.Json;
using MTD.CouchBot.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MTD.CouchBot.Modules
{
    [Group("whitelist")]
    public class Whitelist : ModuleBase
    {

        [Command("view"), Summary("View the server whitelist")]
        public async Task View()
        {
            var guild = ((IGuildUser)Context.Message.Author).Guild;

            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }

            var server = BotFiles.GetDiscordServer(guild.Id.ToString());
            if (server.BroadcasterWhitelist != null)
            {
                List<IUser> users = new List<IUser>();
                foreach(var u in server.BroadcasterWhitelist)
                {
                    users.Add(await guild.GetUserAsync(ulong.Parse(u)));
                }

                string userList = "";
                int counter = 1;
                foreach(var u in users)
                {
                    userList += u.Username;

                    if (counter < users.Count)
                        userList += ", ";

                    counter++;
                }

                string info = "```Markdown\r\n" +
                    "# Current Server Whitelist\r\n" +
                    userList + "\r\n" +
                    "```\r\n";

                await Context.Channel.SendMessageAsync(info);
            }
        }

        [Command("on"), Summary("Turns the whitelist on")]
        public async Task On()
        {
            var guild = ((IGuildUser)Context.Message.Author).Guild;

            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }


            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            server.UseWhitelist = true;
            File.WriteAllText(file, JsonConvert.SerializeObject(server));
            await Context.Channel.SendMessageAsync("Use whitelist has been turned on.");
        }

        [Command("off"), Summary("Turns the whitelist off")]
        public async Task Off()
        {
            var guild = ((IGuildUser)Context.Message.Author).Guild;

            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }

            var server = BotFiles.GetDiscordServer(guild.Id.ToString());

            server.UseWhitelist = false;
            BotFiles.SaveDiscordServer(server);
            await Context.Channel.SendMessageAsync("Use whitelist has been turned off.");
        }

        [Command("add"), Summary("Adds user to go live announcement whitelist.")]
        public async Task Add(IUser user)
        {
            var guild = ((IGuildUser)Context.Message.Author).Guild;
            var authorUser = ((IGuildUser)Context.Message.Author);

            if (!authorUser.GuildPermissions.ManageGuild)
            {
                return;
            }

            var server = BotFiles.GetDiscordServer(guild.Id.ToString());
            
            if (server.BroadcasterWhitelist == null)
                server.BroadcasterWhitelist = new List<string>();

            if (!server.BroadcasterWhitelist.Contains(user.Id.ToString()))
            {
                server.BroadcasterWhitelist.Add(user.Id.ToString());
                BotFiles.SaveDiscordServer(server);
                await Context.Channel.SendMessageAsync(user.Username + " has been added to the whitelist.");
            }
            else
            {
                await Context.Channel.SendMessageAsync(user.Username + " was already on the whitelist.");
            }
        }

        [Command("remove"), Summary("Adds user to go live announcement whitelist.")]
        public async Task Remove(IUser user)
        {
            var guild = ((IGuildUser)Context.Message.Author).Guild;
            var authorUser = ((IGuildUser)Context.Message.Author);

            if (!authorUser.GuildPermissions.ManageGuild)
            {
                return;
            }

            var server = BotFiles.GetDiscordServer(guild.Id.ToString());

            if (server.BroadcasterWhitelist == null)
                server.BroadcasterWhitelist = new List<string>();

            if (server.BroadcasterWhitelist.Contains(user.Id.ToString()))
            {
                server.BroadcasterWhitelist.Remove(user.Id.ToString());
                BotFiles.SaveDiscordServer(server);
                await Context.Channel.SendMessageAsync(user.Username + " has been removed from the whitelist.");
            }
            else
            {
                await Context.Channel.SendMessageAsync(user.Username + " was not on the whitelist.");
            }
        }
    }
}
