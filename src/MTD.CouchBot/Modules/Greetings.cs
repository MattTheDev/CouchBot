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
    [Group("greetings")]
    public class Greetings : ModuleBase
    {
        [Command("on"), Summary("Turns the greetings on")]
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
            {
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));
            }

            server.Greetings = true;
            if(string.IsNullOrEmpty(server.GreetingMessage))
            {
                server.GreetingMessage = "Welcome to the server, %USER%";
                server.GoodbyeMessage = "Good bye, %USER%, thanks for hanging out!";
            }

            await BotFiles.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("Greetings have been turned on.");
        }

        [Command("off"), Summary("Turns the greetings off")]
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
            {
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));
            }

            server.Greetings = false;
            await BotFiles.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("Greetings have been turned off.");
        }

        [Command("set"), Summary("Sets the greeting message")]
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
            {
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));
            }

            server.GreetingMessage = message;
            await BotFiles.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("Greeting has been set.");
        }
    }
}
