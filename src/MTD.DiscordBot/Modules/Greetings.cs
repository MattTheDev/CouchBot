using Discord;
using Discord.Commands;
using MTD.DiscordBot.Domain;
using MTD.DiscordBot.Json;
using MTD.DiscordBot.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MTD.DiscordBot.Modules
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
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            server.Greetings = true;
            if(string.IsNullOrEmpty(server.GreetingMessage))
            {
                server.GreetingMessage = "Welcome to the server, %USER%";
                server.GoodbyeMessage = "Good bye, %USER%, thanks for hanging out!";
            }

            File.WriteAllText(file, JsonConvert.SerializeObject(server));
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
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            server.Greetings = false;
            File.WriteAllText(file, JsonConvert.SerializeObject(server));
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
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            server.GreetingMessage = message;
            File.WriteAllText(file, JsonConvert.SerializeObject(server));
            await Context.Channel.SendMessageAsync("Greeting has been set.");
        }
    }
}
