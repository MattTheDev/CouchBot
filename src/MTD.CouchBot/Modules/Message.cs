using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MTD.CouchBot.Bot;
using MTD.CouchBot.Domain;
using MTD.CouchBot.Domain.Utilities;
using MTD.CouchBot.Json;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MTD.CouchBot.Modules
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

        [Command("test")]
        public async Task Test()
        {
            var guild = ((IGuildUser)Context.Message.Author).Guild;

            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }

            var message = await MessagingHelper.BuildTestMessage((SocketUser) Context.User, Context.Guild.Id, Context.Channel.Id);

            if (message != null)
            {
                try
                {
                    if (message.Embed != null)
                    {
                        RequestOptions options = new RequestOptions();
                        options.RetryMode = RetryMode.AlwaysRetry;
                        var msg = await Context.Channel.SendMessageAsync(message.Message, false, message.Embed, options);
                    }
                    else
                    {
                        var msg = await Context.Channel.SendMessageAsync(message.Message);
                    }
                }
                catch (Exception ex)
                {
                    Logging.LogError("Error in Message.Test Command: " + ex.Message);
                }
            }
        }
    }
}
