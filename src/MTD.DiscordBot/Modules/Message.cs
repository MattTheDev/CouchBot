using Discord;
using Discord.Commands;
using MTD.DiscordBot.Domain;
using MTD.DiscordBot.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MTD.DiscordBot.Modules
{
    [Group("message"), Summary("Subset of Commands to configure server messages.")]
    public class Message : ModuleBase
    {
        [Command("live")]
        public async Task Live(string message)
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

            server.LiveMessage = message;
            File.WriteAllText(file, JsonConvert.SerializeObject(server));
            await Context.Channel.SendMessageAsync("Live Message has been set.");
        }

        [Command("published")]
        public async Task Published(string message)
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

            server.PublishedMessage = message;
            File.WriteAllText(file, JsonConvert.SerializeObject(server));
            await Context.Channel.SendMessageAsync("Live Message has been set.");
        }
    }
}
