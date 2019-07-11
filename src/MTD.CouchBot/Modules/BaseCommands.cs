using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using MTD.CouchBot.Domain.Models.Bot;
using MTD.CouchBot.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MTD.CouchBot.Domain;
using MTD.CouchBot.Managers;
using EmbedBuilder = Discord.EmbedBuilder;

namespace MTD.CouchBot.Modules
{
    public class BaseCommands : BaseModule
    {
        private readonly YouTubeManager _youtubeManager;
        private readonly DiscordShardedClient _discord;
        private readonly BotSettings _botSettings;
        private readonly FileService _fileService;
        private readonly LoggingService _loggingService;

        public BaseCommands(YouTubeManager youtubeManager, DiscordShardedClient discord, 
            IOptions<BotSettings> botSettings, FileService fileService, LoggingService loggingService) : base(botSettings, fileService)
        {
            _youtubeManager = youtubeManager;
            _discord = discord;
            _botSettings = botSettings.Value;
            _fileService = fileService;
            _loggingService = loggingService;
        }

        [Command("info"), Summary("Get CouchBot Information.")]
        public async Task Info()
        {
            var serverFiles = Directory.GetFiles(_botSettings.DirectorySettings.ConfigRootDirectory + _botSettings.DirectorySettings.GuildDirectory);
            var counts = _fileService.GetUniqueServerChannelCounts();
            var serverCount = _discord.Guilds.Count;

            var builder = new EmbedBuilder();
            var authorBuilder = new EmbedAuthorBuilder();
            var footerBuilder = new EmbedFooterBuilder();

            var server = GetServer();

            authorBuilder.IconUrl = _discord.CurrentUser.GetAvatarUrl();
            authorBuilder.Name = _discord.CurrentUser.Username;
            authorBuilder.Url = "http://couchbot.mattthedev.codes";

            footerBuilder.IconUrl = _discord.CurrentUser.GetAvatarUrl();
            footerBuilder.Text = $"[CouchBot] - {DateTime.UtcNow.AddHours(server.TimeZoneOffset)}";

            builder.Description = "Welcome to CouchBot, the Content Creator and Streamer Bot! Looking to engage, " +
                "notify, and involve your communities? Look no further!";
            builder.Url = "http://google.com";

            builder.Author = authorBuilder;
            builder.Footer = footerBuilder;

            builder.AddField(new EmbedFieldBuilder()
            {
                Name = "Platforms",
                Value =
                    $"- Mixer: {counts.MixerCount}\r\n- Mixer Teams: {counts.MixerTeamCount}\r\n- Mobcrush: {counts.MobcrushCount}\r\n- Picarto: {counts.PicartoCount}\r\n",
                IsInline = true
            });

            builder.AddField(new EmbedFieldBuilder()
            {
                Name = " _ ",
                Value =
                    $"- Piczel: {counts.PiczelCount}\r\n- Smashcast: {counts.SmashcastCount}\r\n- Twitch: {counts.TwitchCount}\r\n- Twitch Games: {counts.TwitchGameCount}\r\n",
                IsInline = true
            });

            builder.AddField(new EmbedFieldBuilder()
            {
                Name = " _ ",
                Value = $"- Twitch Teams: {counts.TwitchTeamCount}\r\n- YouTube: {counts.YouTubeCount}\r\n",
                IsInline = true
            });

            builder.AddField(new EmbedFieldBuilder()
            {
                Name = "Connected Servers",
                Value = serverCount,
                IsInline = true
            });

            builder.AddField(new EmbedFieldBuilder()
            {
                Name = "Configured Servers",
                Value = serverFiles.Length,
                IsInline = true
            });


            builder.AddField(new EmbedFieldBuilder()
            {
                Name = "Even More Info!",
                Value = "- Current Memory Usage: " + ((System.Diagnostics.Process.GetCurrentProcess().WorkingSet64 / 1024) / 1024) + "MB \r\n" +
                        "- Developed and Maintained by Matt - http://twitter.com/MattTheDev \r\n" +
                        "- Join us on Discord!- http://discord.mattthedev.codes \r\n",
                IsInline = false
            });

            await Context.Channel.SendMessageAsync("", false, builder.Build());
        }

        [Command("invite"), Summary("Provide an invite link via DM.")]
        public async Task Invite()
        {
            await (await ((IGuildUser)(Context.Message.Author)).GetOrCreateDMChannelAsync()).SendMessageAsync("Want me to join your server? Click here: <https://discordapp.com/oauth2/authorize?client_id=308371905667137536&scope=bot&permissions=158720>");
        }

        [Command("ytidlookup"), Summary("Query YouTube API to find a Channel ID.")]
        public async Task YtIdLookup([Summary("Username to Query")]string name)
        {
            var channels = await _youtubeManager.GetYouTubeChannelByQuery(name);

            await Context.Channel.SendMessageAsync("YouTube Id Lookup Results: ");

            if (channels.items == null || channels.items.Count < 1)
            {
                await Context.Channel.SendMessageAsync("No Results Found.");
            }
            else
            {

                var channelInfo = "```\r\n";
                foreach (var channel in channels.items)
                {
                    channelInfo += "\r\n" +
                        "Id (Use This for " + _discord.CurrentUser.Username + " Settings): " + channel.snippet.channelId + "\r\n" +
                        "Channel Name: " + channel.snippet.channelTitle + "\r\n" +
                        "Description: " + channel.snippet.description + "\r\n";
                }

                channelInfo += "```";

                await Context.Channel.SendMessageAsync(channelInfo);
            }
        }

        [Command("permissions"), Summary("Check bot permissions.")]
        public async Task Permissions(IGuildChannel channel)
        {
            var botUser = await Context.Guild.GetCurrentUserAsync();
            var channelPermissions = botUser.GetPermissions(channel);

            var info = "```Markdown\r\n" +
              "# " + _discord.CurrentUser.Username + " Permissions in " + botUser.Username + "\r\n" +
              "- Add Reactions: " + channelPermissions.AddReactions + "\r\n" +
              "- Attach Files: " + channelPermissions.AttachFiles + "\r\n" +
              "- Connect: " + channelPermissions.Connect + "\r\n" +
              "- Create Invite: " + channelPermissions.CreateInstantInvite + "\r\n" +
              "- Deafen: " + channelPermissions.DeafenMembers + "\r\n" +
              "- **Embed**: " + channelPermissions.EmbedLinks + "\r\n" +
              "- Manage Channel: " + channelPermissions.ManageChannel + "\r\n" +
              "- **Manage Messages**: " + channelPermissions.ManageMessages + "\r\n" +
              "- **Manage Roles**: " + channelPermissions.ManageRoles + "\r\n" +
              "- Manage Webhooks: " + channelPermissions.ManageWebhooks + "\r\n" +
              "- **Mention Everyone**: " + channelPermissions.MentionEveryone + "\r\n" +
              "- Move: " + channelPermissions.MoveMembers + "\r\n" +
              "- Mute: " + channelPermissions.MuteMembers + "\r\n" +
              "- **Read History**: " + channelPermissions.ReadMessageHistory + "\r\n" +
              "- **Read**: " + channelPermissions.ViewChannel + "\r\n" +
              "- **Send**: " + channelPermissions.SendMessages + "\r\n" +
              "- Send TTS: " + channelPermissions.SendTTSMessages + "\r\n" +
              "- Speak: " + channelPermissions.Speak + "\r\n" +
              "- External Emojis: " + channelPermissions.UseExternalEmojis + "\r\n" +
              "- Use VAD: " + channelPermissions.UseVAD + "\r\n" +
              "**Required Permissions are Bold.**\r\n" +
              "```";

            await Context.Channel.SendMessageAsync(info);
        }

        [Command("ping")]
        public async Task Ping()
        {
            await Context.Channel.SendMessageAsync("Pong!");
        }

        [Command("purge")]
        public async Task Purge(IGuildUser user)
        {
            var deleteList = new List<IMessage>();

            if (!IsAdmin)
            {
                return;
            }

            var messages = Context.Channel.GetMessagesAsync();

            var message = (messages.Flatten()).GetEnumerator();

            while (await message.MoveNext())
            {
                if (message.Current.Author.Id == user.Id)
                {
                    deleteList.Add(message.Current);
                }
            }


            if (deleteList.Count > 0)
            {
                var channel = (ITextChannel)Context.Channel;
                await channel.DeleteMessagesAsync(deleteList);
            }

            await Context.Message.DeleteAsync();
        }

        [Command("purge")]
        public async Task Purge(IGuildUser user, int count)
        {
            var deleteList = new List<IMessage>();

            if (!IsAdmin)
            {
                return;
            }

            if (count > 100)
            {
                await Context.Channel.SendMessageAsync("You can only purge up to 100 messages.");

                return;
            }
            await Context.Message.DeleteAsync();

            var messages = Context.Channel.GetMessagesAsync(count);

            var message = (messages.Flatten()).GetEnumerator();

            while (await message.MoveNext())
            {
                if (message.Current.Author.Id == user.Id)
                {
                    deleteList.Add(message.Current);
                }
            }


            if (deleteList.Count > 0)
            {
                foreach (var m in deleteList)
                {
                    await m.DeleteAsync();
                }

                await Context.Message.DeleteAsync();
            }
        }

        [Command("purge")]
        public async Task Purge(int count)
        {
            var deleteList = new List<IMessage>();

            if (!IsAdmin)
            {
                return;
            }

            if (count > 100)
            {
                await Context.Channel.SendMessageAsync("You can only purge up to 100 messages.");

                return;
            }

            await Context.Message.DeleteAsync();

            var messages = Context.Channel.GetMessagesAsync(count);

            var message = (messages.Flatten()).GetEnumerator();

            while (await message.MoveNext())
            {
                deleteList.Add(message.Current);
            }


            if (deleteList.Count > 0)
            {
                var channel = (ITextChannel)Context.Channel;
                await channel.DeleteMessagesAsync(deleteList);
            }

            await Context.Message.DeleteAsync();
        }

        [Command("purgeall")]
        public async Task PurgeAll()
        {
            var deleteList = new List<IMessage>();

            if (!IsAdmin)
            {
                return;
            }

            var messages = Context.Channel.GetMessagesAsync();

            var message = (messages.Flatten()).GetEnumerator();

            while (await message.MoveNext())
            {
                deleteList.Add(message.Current);
            }

            if (deleteList.Count > 0)
            {
                var channel = (ITextChannel)Context.Channel;
                await channel.DeleteMessagesAsync(deleteList);
            }

            await Context.Message.DeleteAsync();
        }

        [Command("prefix")]
        public async Task Prefix()
        {
            if (!IsAdmin) return;

            var server = GetServer();

            var prefix = server.Prefix ?? _botSettings.BotConfig.Prefix;

            await Context.Channel.SendMessageAsync($"Your server's prefix is: {prefix}");
        }

        [Command("prefix")]
        public async Task Prefix(string prefix)
        {
            if (!IsAdmin) return;

            var server = GetServer();

            server.Prefix = prefix;
            _fileService.SaveDiscordServer(server);

            await Context.Channel.SendMessageAsync($"Your server's prefix has been set to: {prefix}");
        }

        [Command("disclaimer")]
        public async Task Disclaimer()
        {
            await Context.Channel.SendMessageAsync(
                "Per the Discord Developers Terms of Service (found here: <https://discordapp.com/developers/docs/legal>) I am required to let you know what by using @🛋 CouchBot 🤖 you are agreeing to allow the bot to store your User ID (17 digit unique identifier), the Guild (server) ID (17 digit unique identifier), and any Channel IDs (17 digit unique identifiers) that you specify in his configuration. These 3 values are the only values that he stores associated with your Discord Account and server. Please note .. these 3 values are easily found by ANYONE by turning on developer mode, right clicking your name, and clicking Copy Id. ANYONE can find this information in seconds - we use this data to find your configuration in the system, and announce your streams.\r\n\r\n" +
                "This data, as of August 20, 2017, will be encrypted in case this already public (sarcasm here. this is silly) information is ever obtained by an external source(impossible.but whatever.).\r\n\r\n" +
                "I am tagging @everyone to make sure you know, you are aware, and you're acknowledging that this data is stored and it is ok. I will be wiring in a !cb disclaimer command to show this information, and add it to the bot's about and website as well.\r\n\r\n" +
                "Thank you,\r\n" +
                "Dawgeth/Matt\r\n" +
                "Couch Commander Supreme");
        }

        [Command("NotSetupForAnnouncements")]
        public async Task NotSetupForAnnouncements(bool purge, int count)
        {
            if (!IsBotOwner)
            {
                return;
            }

            var allServersNotSetup = _fileService.GetServersNotSetupForAnnouncements();

            File.WriteAllText(@"C:\programdata\couchbot\audit_noannouncements.txt", string.Join("\r\n", allServersNotSetup.Select(x => x.Id)));

            await Context.Channel.SendMessageAsync($"Channels not configured for announcements: {allServersNotSetup.Count}");

            if (purge)
            {
                await Context.Channel.SendMessageAsync("Purging...");

                var processed = 1;
                foreach (var server in allServersNotSetup)
                {
                    try
                    {
                        File.Copy($"C:\\ProgramData\\Couchbot\\Guilds\\{server.Id}.json", $"C:\\programdata\\couchbot\\Guilds_Left\\{server.Id}.json");
                    }
                    catch (Exception)
                    {
                        // Couldnt backup. Whatever, lets go.
                    }

                    try
                    {
                        var guild = _discord.GetGuild(server.Id);
                        await guild.LeaveAsync();
                    }
                    catch (Exception)
                    {
                        // Most likely, already out of server. Just continue.
                    }

                    processed++;
                    if (processed == count)
                    {
                        break;
                    }
                }
            }
        }

        [Command("MessageServerOwner")]
        public async Task MessageServerOwner(ulong guildId, string message)
        {
            if (!IsBotOwner)
            {
                return;
            }

            var serverOwnerId = _discord.GetGuild(guildId)?.OwnerId;

            if (serverOwnerId.HasValue)
            {
                var serverOwner = _discord.GetUser(serverOwnerId.Value);

                try
                {
                    var dmChannel = await serverOwner.GetOrCreateDMChannelAsync();
                    await dmChannel.SendMessageAsync($"{message}\r\n\r\nMessage sent by Matt. " +
                        $"Please join the support server (<http://discord.mattthedev.codes>) if you need any further help.");

                }
                catch (Exception)
                {
                    await Context.Channel.SendMessageAsync($"Unable to open DM with {serverOwner.Username} ({serverOwnerId}).");
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync($"Unable to open DM with {serverOwnerId}.");
            }
        }

        [Command("ConfigurationCleanup")]
        public async Task ConfigurationCleanup(bool liveProcess)
        {
            if (!IsBotOwner)
            {
                return;
            }

            _botSettings.PlatformSettings.EnableMixer = false;
            _botSettings.PlatformSettings.EnableMobcrush = false;
            _botSettings.PlatformSettings.EnablePicarto = false;
            _botSettings.PlatformSettings.EnablePiczel = false;
            _botSettings.PlatformSettings.EnableSmashcast = false;
            _botSettings.PlatformSettings.EnableTwitch = false;
            _botSettings.PlatformSettings.EnableYouTube = false;

            var allServersNotSetup = _fileService.GetServersNotSetupForAnnouncements();
            var allServersConfiguredButNotConnected = _fileService.GetServersConfiguredButNotConnected();
            var allServersConnectedButNotConfigured = _fileService.GetServersConnectedByNotConfigured();

            await Context.Channel.SendMessageAsync("Removing the bot configurations for servers I'm not connected to.");

            if (liveProcess)
            {
                foreach (var server in allServersConfiguredButNotConnected)
                {
                    var fileFrom = $"C:\\ProgramData\\CouchBot\\Guilds\\{server.Id}.json";
                    var fileTo = $"C:\\ProgramData\\CouchBot\\Guilds_Left\\{server.Id}.json";
                    try
                    {
                        if (File.Exists(fileTo))
                        {
                            File.Delete(fileTo);
                        }

                        File.Move(fileFrom, fileTo);
                    }
                    catch (Exception)
                    {
                        // TODO MS
                        // Move on
                    }
                }
            }

            await Context.Channel.SendMessageAsync($"I've removed {allServersConfiguredButNotConnected.Count} server configurations.\r\n" +
                                                   $"Now I am going to leave servers that I do not have a configuration for.");

            if (liveProcess)
            {
                foreach (var server in allServersConnectedButNotConfigured)
                {
                    await server.LeaveAsync();
                }
            }

            await Context.Channel.SendMessageAsync($"I've left {allServersConnectedButNotConfigured.Count} servers.\r\n" +
                                                   $"Now we're going to leave servers that I'm not being used on.");

            if (liveProcess)
            {
                foreach (var server in allServersNotSetup)
                {
                    var fileFrom = $"C:\\ProgramData\\CouchBot\\Guilds\\{server.Id}.json";
                    var fileTo = $"C:\\ProgramData\\CouchBot\\Guilds_Left\\{server.Id}.json";
                    try
                    {
                        if (File.Exists(fileTo))
                        {
                            File.Delete(fileTo);
                        }

                        File.Move(fileFrom, fileTo);
                        var guild = _discord.GetGuild(server.Id);
                        await guild.LeaveAsync();
                    }
                    catch (Exception)
                    {
                        // TODO MS
                        // Move on
                    }
                }
            }

            await Context.Channel.SendMessageAsync($"I've left {allServersNotSetup.Count} servers.\r\n" +
                                                   $"Done.");

            _botSettings.PlatformSettings.EnableMixer = true;
            _botSettings.PlatformSettings.EnableMobcrush = true;
            _botSettings.PlatformSettings.EnablePicarto = true;
            _botSettings.PlatformSettings.EnablePiczel = true;
            _botSettings.PlatformSettings.EnableSmashcast = true;
            _botSettings.PlatformSettings.EnableTwitch = true;
            _botSettings.PlatformSettings.EnableYouTube = true;
        }
    }
}
