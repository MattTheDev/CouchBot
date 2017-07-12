using Discord;
using Discord.Commands;
using MTD.CouchBot.Domain;
using MTD.CouchBot.Domain.Models.Bot;
using MTD.CouchBot.Domain.Utilities;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace MTD.CouchBot.Modules
{
    // Create a module with the 'sample' prefix
    [Group("goodbyes")]
    public class Goodbyes : ModuleBase
    {
        [Command("on"), Summary("Turns the goodbyes on")]
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

            server.Goodbyes = true;
            if (string.IsNullOrEmpty(server.GoodbyeMessage))
            {
                server.GoodbyeMessage = "Good bye, %USER%, thanks for hanging out!";
            }

            await BotFiles.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("Goodbyes have been turned on.");
        }

        [Command("off"), Summary("Turns the goodbyes off")]
        public async Task Off()
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

            server.Goodbyes = false;
            await BotFiles.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("Goodbyes have been turned off.");
        }

        [Command("set"), Summary("Sets the goodbye message")]
        public async Task Set(string message)
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

            server.GoodbyeMessage = message;
            await BotFiles.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("Goodbye Message has been set.");
        }
    }
}
