using Discord;
using Discord.Commands;
using MTD.CouchBot.Domain;
using MTD.CouchBot.Domain.Models.Bot;
using MTD.CouchBot.Domain.Utilities;
using MTD.CouchBot.Managers;
using MTD.CouchBot.Managers.Implementations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MTD.CouchBot.Modules
{
    public class BaseCommands : ModuleBase
    {
        IStatisticsManager statisticsManager;
        IYouTubeManager youtubeManager;
        IApiAiManager apiAiManager;

        public BaseCommands()
        {
            statisticsManager = new StatisticsManager();
            youtubeManager = new YouTubeManager();
            apiAiManager = new ApiAiManager();
        }

        [Command("info"), Summary("Get CouchBot Information.")]
        public async Task Info()
        {
            var serverFiles = Directory.GetFiles(Constants.ConfigRootDirectory + Constants.GuildDirectory);
            var userFiles = Directory.GetFiles(Constants.ConfigRootDirectory + Constants.UserDirectory);
            var serverTwitchCount = 0;
            var serverYouTubeCount = 0;
            var serverBeamCount = 0;
            var serverHitboxCount = 0;
            var serverPicartoCount = 0;
            var twitchGameCount = 0;
            var twitchTeamCount = 0;

            foreach (var s in serverFiles)
            {
                var server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(s));

                if (server.PicartoChannels != null)
                {
                    foreach (var u in server.PicartoChannels)
                    {
                        serverPicartoCount++;
                    }
                }

                if (server.ServerBeamChannels != null)
                {
                    foreach (var u in server.ServerBeamChannels)
                    {
                        serverBeamCount++;
                    }
                }

                if (server.ServerTwitchChannelIds != null)
                {
                    foreach (var u in server.ServerTwitchChannelIds)
                    {
                        serverTwitchCount++;
                    }
                }

                if (server.ServerYouTubeChannelIds != null)
                {
                    foreach (var u in server.ServerYouTubeChannelIds)
                    {
                        serverYouTubeCount++;
                    }
                }

                if (server.ServerHitboxChannels != null)
                {
                    foreach (var u in server.ServerHitboxChannels)
                    {
                        serverHitboxCount++;
                    }
                }

                if(server.TwitchTeams != null)
                {
                    foreach(var t in server.TwitchTeams)
                    {
                        twitchTeamCount++;
                    }
                }

                if (server.ServerGameList != null)
                {
                    foreach (var g in server.ServerGameList)
                    {
                        twitchGameCount++;
                    }
                }
            }

            int serverCount = 0;

            foreach(var shard in Program.client.Shards)
            {
                serverCount += shard.Guilds.Count;
            }

            string info = "```Markdown\r\n" +
                          "# " + Program.client.CurrentUser.Username + "\r\n" +
                          "- Configured Servers: " + serverFiles.Length + "\r\n" +
                          "- Connected Shards: " + Program.client.Shards.Count + "\r\n" +
                          "- Connected Servers: " + serverCount + "\r\n" +
                          "Platforms: \r\n" +
                          "-- Mixer: " + serverBeamCount + "\r\n" +
                          "-- Picarto: " + serverPicartoCount + "\r\n" +
                          "-- Smashcast: " + serverHitboxCount + "\r\n" +
                          "-- Twitch: " + serverTwitchCount + "\r\n" +
                          "-- Twitch Games: " + twitchGameCount + "\r\n" +
                          "-- Twitch Teams: " + twitchTeamCount + "\r\n" +
                          "-- YouTube: " + serverYouTubeCount + "\r\n" +
                          "-- Total Channels Checked: " + (serverYouTubeCount + serverTwitchCount + serverBeamCount + serverHitboxCount + serverPicartoCount) + "\r\n" +
                          "- Current Memory Usage: " + ((System.Diagnostics.Process.GetCurrentProcess().WorkingSet64 / 1024) / 1024) + "MB \r\n" +
                          "- Built on Discord.Net - (https://github.com/RogueException/Discord.Net)\r\n" +
                          "- Developed and Maintained by Dawgeth - (http://twitter.com/dawgeth)\r\n" +
                          "- Want to submit feedback, a bug, or suggestion? - (http://github.com/dawgeth/couchbot/issues)\r\n" +
                          "- Join us on Discord!-(http://discord.couchbot.io)\r\n" +
                          "```\r\n";

            await Context.Channel.SendMessageAsync(info);
        }
        
        [Command("servers"), Summary("Get Server Statistic Information.")]
        public async Task Servers()
        {
            var serverFiles = Directory.GetFiles(Constants.ConfigRootDirectory + Constants.GuildDirectory);

            List<ulong> serversUsingNone = new List<ulong>();
            List<ulong> serversUsingGoLive = new List<ulong>();
            List<ulong> serversUsingAnnouncements = new List<ulong>();
            List<ulong> serversUsingPublished = new List<ulong>();
            List<ulong> serversUsingGreetings = new List<ulong>();

            foreach (var s in serverFiles)
            {
                var server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(s));

                if(server.GoLiveChannel != 0)
                {
                    serversUsingGoLive.Add(server.Id);
                }

                if (server.AnnouncementsChannel != 0)
                {
                    serversUsingAnnouncements.Add(server.Id);
                }

                if (server.PublishedChannel != 0)
                {
                    serversUsingPublished.Add(server.Id);
                }

                if (server.GreetingsChannel != 0)
                {
                    serversUsingGreetings.Add(server.Id);
                }

                if (server.GoLiveChannel == 0 && server.AnnouncementsChannel == 0 && server.PublishedChannel == 0 && server.GreetingsChannel == 0)
                {
                    serversUsingNone.Add(server.Id);
                }
            }

            string info = "```Markdown\r\n" +
                          "# " + Program.client.CurrentUser.Username + "\r\n" +
                          "- Servers: " + serverFiles.Length + "\r\n" +
                          "- Servers Announcing Livestreams: " + serversUsingGoLive.Count + "\r\n" +
                          "- Servers Announcing Announcements: " + serversUsingAnnouncements.Count + "\r\n" +
                          "- Servers Announcing Published Content: " + serversUsingPublished.Count + "\r\n" +
                          "- Servers Announcing Greetings: " + serversUsingGreetings.Count + "\r\n" +
                          "- Servers Announcing Nothing: " + serversUsingNone.Count + "\r\n" +
                          "```\r\n";

            await Context.Channel.SendMessageAsync(info);
        }

        [Command("help"), Summary("Provides a link to the CouchBot Website.")]
        public async Task Help()
        {
            string info = "Need help? Ask me a question! ie: !cb help \"How can I add my Twitch account?\". Note: Please include quotes around your question.";

            await Context.Channel.SendMessageAsync(info);
        }

        [Command("help"), Summary("Provides a link to the CouchBot Website.")]
        public async Task Help(string question)
        {
            var response = await apiAiManager.AskAQuestion(question);

            await Context.Channel.SendMessageAsync(response.result.speech);
        }

        [Command("invite"), Summary("Provide an invite link via DM.")]
        public async Task Invite()
        {
            await (await ((IGuildUser)(Context.Message.Author)).CreateDMChannelAsync()).SendMessageAsync("Want me to join your server? Click here: <https://discordapp.com/oauth2/authorize?client_id=308371905667137536&scope=bot&permissions=158720>");
        }

        [Command("uptime"), Summary("Get Uptime Statistic Information.")]
        public async Task Uptime()
        {
            var botStats = statisticsManager.GetBotStats();
            var minutesSinceStart = Math.Abs((int)DateTime.UtcNow.Subtract(botStats.LoggingStartDate).TotalMinutes);
            TimeSpan ts = new TimeSpan(0, botStats.UptimeMinutes, 0);

            decimal percentage = ((decimal)botStats.UptimeMinutes / minutesSinceStart) * (decimal)100;

            var info = "```Markdown\r\n" +
                    "# Uptime since initial tracking - " + botStats.LoggingStartDate.ToString("MM/dd/yyyy HH:mm:ss UTC") + "\r\n" +
                    "" + ts.Days + " day(s) " + ts.Hours + " hour(s)" + (ts.Minutes > 0 ? " and " + ts.Minutes + " minutes" : "") + ".\r\n" +
                    "Uptime Percentage: " + percentage.ToString("#.##") + "%\r\n" +
                    "Last Restart: " + botStats.LastRestart.ToString("MM/dd/yyyy HH:mm:ss UTC") + "\r\n" +
                    "```\r\n";

            await Context.Channel.SendMessageAsync(info);
        }

        [Command("alerts"), Summary("Get Alert Statistic Information.")]
        public async Task Alerts()
        {
            var botStats = statisticsManager.GetBotStats();

            var info = "```Markdown\r\n" +
                    "# Alerts since initial tracking - " + botStats.LoggingStartDate.ToString("MM/dd/yyyy hh:mm:ss UTC") + "\r\n" +
                    "Mixer - " + botStats.BeamAlertCount + "\r\n" +
                    "Picarto - " + botStats.PicartoAlertCount + "\r\n" +
                    "Smashcast - " + botStats.HitboxAlertCount + "\r\n" +
                    "Twitch - " + botStats.TwitchAlertCount + "\r\n" +
                    "YouTube - " + botStats.YouTubeAlertCount + "\r\n" +
                    "Total Alerts Sent - " + (botStats.YouTubeAlertCount + botStats.BeamAlertCount + botStats.TwitchAlertCount + botStats.HitboxAlertCount) + "\r\n" +
                    "```\r\n";

            await Context.Channel.SendMessageAsync(info);
        }
        
        [Command("supporters"), Summary("Get Supporter Information")]
        public async Task Supporters()
        {
            var supporters = (File.ReadAllText(Constants.ConfigRootDirectory + "Supporters.txt")).Split(',');

            string info = "```Markdown\r\n" +
                          "# " + Program.client.CurrentUser.Username + " Supporters\r\n" +
                          "I wanted to create a place to show my thanks to everyone that supports the development and growth of " + Program.client.CurrentUser.Username + ", whether it be via <http://patreon.com/dawgeth>, or " +
                          "just in it's use. Thanks to all those out there that have used it, provided feedback, found bugs, and supported me through Patreon.\r\n\r\n" +
                          "Patron List:\r\n";

            foreach(var s in supporters)
            {
                info += "- " + s + "\r\n";
            }

            info +=  "- Your Name Could Be Here. Visit <http://patreon.com/dawgeth> today <3" +
                     "- Want to be a one time supporter? <http://paypal.me/dawgeth>" +
                     "```\r\n";

            await Context.Channel.SendMessageAsync(info);
        }

        [Command("ytidlookup"), Summary("Query YouTube API to find a Channel ID.")]
        public async Task YtIdLookup([Summary("Username to Query")]string name)
        {
            var channels = await youtubeManager.GetYouTubeChannelByQuery(name);

            await Context.Channel.SendMessageAsync("YouTube Id Lookup Results: ");

            if (channels.items == null || channels.items.Count < 1)
            {
                await Context.Channel.SendMessageAsync("No Results Found.");
            }
            else {

                string channelInfo = "```\r\n";
                foreach (var channel in channels.items)
                {
                    channelInfo += "\r\n" +
                        "Id (Use This for " + Program.client.CurrentUser.Username + " Settings): " + channel.snippet.channelId + "\r\n" +
                        "Channel Name: " + channel.snippet.channelTitle + "\r\n" +
                        "Description: " + channel.snippet.description + "\r\n";
                }

                channelInfo += "```";

                await Context.Channel.SendMessageAsync(channelInfo);
            }
        }

        [Command("haibai")]
        public async Task HaiBai()
        {
            statisticsManager.AddToHaiBaiCount();

            EmbedBuilder builder = new EmbedBuilder();
            builder.ImageUrl = "http://couchbot.io/img/bot/haibai.gif";
            builder.AddField(f =>
            {
                f.Name = "Hai Bai Count!";
                f.Value = statisticsManager.GetHaiBaiCount();
                f.IsInline = false;
            });

            await Context.Channel.SendMessageAsync("", false, builder.Build());
        }

        [Command("setbotgame")]
        public async Task SetBotGame(string game)
        {
            if(Context.User.Id != 93015586698727424)
            {
                await Context.Channel.SendMessageAsync("*Bbbbbzzztttt* You are not *zzzzt* Dawgeth. Acc *bbrrrttt* Access Denied.");

                return;
            }

            await Program.client.SetGameAsync(game, "", StreamType.NotStreaming);
        }

        [Command("permissions"), Summary("Check bot permissions.")]
        public async Task Permissions(IGuildChannel channel)
        {
            var botUser = await Context.Guild.GetCurrentUserAsync();
            var channelPermissions = botUser.GetPermissions(channel);

            string info = "```Markdown\r\n" +
              "# " + Program.client.CurrentUser.Username + " Permissions in " + botUser.Username + "\r\n" +
              "- Add Reactions: " + channelPermissions.AddReactions + "\r\n" +
              "- Attach Files: " + channelPermissions.AttachFiles + "\r\n" +
              "- Connect: " + channelPermissions.Connect + "\r\n" +
              "- Create Invite: " + channelPermissions.CreateInstantInvite + "\r\n" +
              "- Deafen: " + channelPermissions.DeafenMembers + "\r\n" +
              "- **Embed**: " + channelPermissions.EmbedLinks + "\r\n" +
              "- Manage Channel: " + channelPermissions.ManageChannel + "\r\n" +
              "- Manage Messages: " + channelPermissions.ManageMessages + "\r\n" +
              "- Manage Permissions: " + channelPermissions.ManagePermissions + "\r\n" +
              "- Manage Webhooks: " + channelPermissions.ManageWebhooks + "\r\n" +
              "- **Mention Everyone**: " + channelPermissions.MentionEveryone + "\r\n" +
              "- Move: " + channelPermissions.MoveMembers + "\r\n" +
              "- Mute: " + channelPermissions.MuteMembers + "\r\n" +
              "- **Read History**: " + channelPermissions.ReadMessageHistory + "\r\n" +
              "- **Read**: " + channelPermissions.ReadMessages + "\r\n" +
              "- **Send**: " + channelPermissions.SendMessages + "\r\n" +
              "- Send TTS: " + channelPermissions.SendTTSMessages + "\r\n" +
              "- Speak: " + channelPermissions.Speak + "\r\n" +
              "- External Emojis: " + channelPermissions.UseExternalEmojis + "\r\n" +
              "- Use VAD: " + channelPermissions.UseVAD + "\r\n" +
              "**Required Permissions are Bold.**\r\n" +
              "```";

            await Context.Channel.SendMessageAsync(info);
        }

        [Command("echo")]
        public async Task Echo(string message)
        {
            await Context.Channel.SendMessageAsync(StringUtilities.ScrubChatMessage(message));
        }

        [Command("echoembed")]
        public async Task EchoEmbed(string message)
        {
            EmbedBuilder eb = new EmbedBuilder();
            eb.AddField("ECHO!", StringUtilities.ScrubChatMessage(message));

            await Context.Channel.SendMessageAsync("", false, eb.Build(), null);
        }

        [Command("ping")]
        public async Task Ping()
        {
            await Context.Channel.SendMessageAsync("Pong!");
        }
    }
}
