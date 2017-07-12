using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MTD.CouchBot.Bot;
using MTD.CouchBot.Domain;
using MTD.CouchBot.Domain.Models.Bot;
using MTD.CouchBot.Domain.Utilities;
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

            if (message.ToLower().Equals("clear"))
            {
                server.LiveMessage = "%CHANNEL% just went live with %GAME% - %TITLE% - %URL%";
                await Context.Channel.SendMessageAsync("Live Message has been reset to the default message.");
            }
            else
            {
                server.LiveMessage = message;
                await Context.Channel.SendMessageAsync("Live Message has been set.");
            }

            await BotFiles.SaveDiscordServer(server, Context.Guild);
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

            if (message.ToLower().Equals("clear"))
            {
                server.PublishedMessage = "%CHANNEL% just published a new video - %TITLE% - %URL%";
                await Context.Channel.SendMessageAsync("Published Message has been reset to the default message.");
            }
            else
            {
                server.PublishedMessage = message;
                await Context.Channel.SendMessageAsync("Published Message has been set.");
            }

            await BotFiles.SaveDiscordServer(server, Context.Guild);
        }

        [Command("offline")]
        public async Task Offline(string message)
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

            if (message.ToLower().Equals("clear"))
            {
                server.StreamOfflineMessage = "This stream is now offline.";
                await Context.Channel.SendMessageAsync("Stream Offline Message has been reset to the default message.");
            }
            else
            {
                server.StreamOfflineMessage = message;
                await Context.Channel.SendMessageAsync("Stream Offline Message has been set.");
            }

            await BotFiles.SaveDiscordServer(server, Context.Guild);
        }

        [Command("testlive")]
        public async Task TestLive(string platform)
        {
            if(platform.ToLower() != Constants.Mixer.ToLower() && platform.ToLower() != Constants.YouTube.ToLower() && platform.ToLower() != Constants.Twitch.ToLower() && platform.ToLower() != Constants.Smashcast.ToLower())
            {
                await Context.Channel.SendMessageAsync("Please pass in mixer, smashcast, twitch, youtube or youtube gaming when requesting a test message. (ie: !cb message test youtube)");
                return;
            }

            var guild = ((IGuildUser)Context.Message.Author).Guild;

            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }

            var message = await MessagingHelper.BuildTestMessage((SocketUser) Context.User, Context.Guild.Id, Context.Channel.Id, platform.ToLower());

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

        [Command("testpublished")]
        public async Task TestPublished()
        {
            var guild = ((IGuildUser)Context.Message.Author).Guild;

            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }

            var message = await MessagingHelper.BuildTestPublishedMessage((SocketUser)Context.User, Context.Guild.Id, Context.Channel.Id);

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
