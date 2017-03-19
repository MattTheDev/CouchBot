using Discord;
using Discord.Commands;
using MTD.CouchBot.Json;
using MTD.CouchBot.Managers;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using MTD.CouchBot.Managers.Implementations;
using System;
using MTD.CouchBot.Domain;
using System.Collections.Generic;
using MTD.CouchBot.Domain.Utilities;

namespace MTD.CouchBot.Modules
{
    public class BaseCommands : ModuleBase
    {
        IStatisticsManager statisticsManager;
        IYouTubeManager youtubeManager;

        public BaseCommands()
        {
            statisticsManager = new StatisticsManager();
            youtubeManager = new YouTubeManager();
        }

        [Command("info"), Summary("Get CouchBot Information.")]
        public async Task Info()
        {
            var serverFiles = Directory.GetFiles(Constants.ConfigRootDirectory + Constants.GuildDirectory);
            var userFiles = Directory.GetFiles(Constants.ConfigRootDirectory + Constants.UserDirectory);
            var twitchCount = 0;
            var serverTwitchCount = 0;
            var youTubeCount = 0;
            var serverYouTubeCount = 0;
            var beamCount = 0;
            var serverBeamCount = 0;
            var hitboxCount = 0;
            var serverHitboxCount = 0;

            foreach (var u in userFiles)
            {
                var user = JsonConvert.DeserializeObject<User>(File.ReadAllText(u));

                if (!string.IsNullOrEmpty(user.BeamName))
                {
                    beamCount++;
                }

                if (!string.IsNullOrEmpty(user.TwitchName))
                {
                    twitchCount++;
                }

                if (!string.IsNullOrEmpty(user.YouTubeChannelId))
                {
                    youTubeCount++;
                }

                if (!string.IsNullOrEmpty(user.HitboxName))
                {
                    hitboxCount++;
                }
            }

            foreach (var s in serverFiles)
            {
                var server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(s));

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
            }

            string info = "```Markdown\r\n" +
                          "# CouchBot v1.0\r\n" +
                          "- Guilds: " + serverFiles.Length + "\r\n" +
                          "- Users: " + userFiles.Length + "\r\n" +
                          "Platforms: \r\n" +
                          "-- User Configured YouTube: " + youTubeCount + "\r\n" +
                          "-- Server Configured YouTube: " + serverYouTubeCount + "\r\n" +
                          "-- User Configured Twitch: " + twitchCount + "\r\n" +
                          "-- Server Configured Twitch: " + serverTwitchCount + "\r\n" +
                          "-- User Configured Beam: " + beamCount + "\r\n" +
                          "-- Server Configured Beam: " + serverBeamCount + "\r\n" +
                          "-- User Configured Hitbox: " + hitboxCount + "\r\n" +
                          "-- Server Configured Hitbox: " + serverHitboxCount + "\r\n" +
                          "-- Total Channels Checked: " + (youTubeCount + twitchCount + beamCount + hitboxCount + serverYouTubeCount + serverTwitchCount + serverBeamCount + serverHitboxCount) + "\r\n" +
                          "- Current Memory Usage: " + ((System.Diagnostics.Process.GetCurrentProcess().WorkingSet64 / 1024) / 1024) + "MB \r\n" +
                          "- Built on Discord.Net - (https://github.com/RogueException/Discord.Net)\r\n" +
                          "- Developed and Maintained by Dawgeth - (http://twitter.com/dawgeth)\r\n" +
                          "- Want to submit feedback, a bug, or suggestion ? - (https://bitbucket.org/MattTheDev/couchbot/issues?status=new&status=open)\r\n" +
                          "- Join us on Discord!-(http://discord.couchbot.io)\r\n" +
                          "- I've become self aware. http://twitter.com/CouchBotIsAlive\r\n" +
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
                          "# CouchBot v1.0\r\n" +
                          "- Joined Guilds: " + serverFiles.Length + "\r\n" +
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
            string info = "```Markdown\r\n" + 
                          "#CouchBot v2.0 Help\r\n\r\n" + 
                          "We've got so many commands, we needed to move all of them to our website, http://couchbot.io!\r\n\r\n" +
                          "If you need any further help, join us on our Discord Server, http://discord.couchbot.io. Thanks!!";

            await Context.Channel.SendMessageAsync(info);
        }

        [Command("invite"), Summary("Provide an invite link via DM.")]
        public async Task Invite()
        {
            await (await ((IGuildUser)(Context.Message.Author)).CreateDMChannelAsync()).SendMessageAsync("Want me to join your server? Click here: <http://discordapp.com/oauth2/authorize?client_id=227846530613248000&scope=bot>");
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
                    "YouTube - " + botStats.YouTubeAlertCount + "\r\n" +
                    "Twitch - " + botStats.TwitchAlertCount + "\r\n" +
                    "Beam - " + botStats.BeamAlertCount + "\r\n" +
                    "Hitbox - " + botStats.HitboxAlertCount + "\r\n" +
                    "Total Alerts Sent - " + (botStats.YouTubeAlertCount + botStats.BeamAlertCount + botStats.TwitchAlertCount + botStats.HitboxAlertCount) + "\r\n" +
                    "```\r\n";

            await Context.Channel.SendMessageAsync(info);
        }
        
        [Command("supporters"), Summary("Get Supporter Information")]
        public async Task Supporters()
        {
            string info = "```Markdown\r\n" +
                          "# CouchBot Supporters\r\n" +
                          "I wanted to create a place to show my thanks to everyone that supports the development and growth of CouchBot, whether it be via <http://patreon.com/dawgeth>, or " +
                          "just in it's use. Thanks to all those out there that have used it, provided feedback, found bugs, and supported me through Patreon.\r\n\r\n" +
                          "Patron List:\r\n" + 
                          "- GreatOrator\r\n" +
                          "- Your Name Could Be Here. Visit <http://patreon.com/dawgeth> today <3" +
                          "- Want to be a 1 time supporter? <http://paypal.me/dawgeth" +
                          "```\r\n";

            await Context.Channel.SendMessageAsync(info);
        }

        [Command("broadcast"), Summary("Broadcasts to EVERY server with an AnnounceChannel set.")]
        public async Task Broadcast([Summary("Message to Broadcast")]string message)
        {
            // Lock this command to Bot Owner only.
            if (Context.Message.Author.Id == 93015586698727424)
            {
                var serverFiles = Directory.GetFiles(Constants.ConfigRootDirectory + Constants.GuildDirectory);
                var guilds = new List<DiscordServer>();

                foreach (var server in serverFiles)
                {
                    try
                    {
                        var guild = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(server));
                        if (guild.AnnouncementsChannel != 0)
                        {
                            guilds.Add(guild);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logging.LogError("Broadcast Error: Deserialize - " + ex.Message);
                    }
                }

                foreach (var guild in guilds)
                {
                    try
                    {
                        var guildObj = Program.client.GetGuild(guild.Id);
                        var channel = (IMessageChannel)(guildObj.GetChannel(guild.Id));

                        await channel.SendMessageAsync(message.Replace("%NEWLINE%", "\r\n"));
                    }
                    catch (Exception ex)
                    {
                        Logging.LogError("Broadcast Error: SendMessage - " + ex.Message);
                    }
                }
            }
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
                        "Id (User This for CouchBot Settings): " + channel.snippet.channelId + "\r\n" +
                        "Channel Name: " + channel.snippet.channelTitle + "\r\n" +
                        "Description: " + channel.snippet.description + "\r\n";
                }

                channelInfo += "```";

                await Context.Channel.SendMessageAsync(channelInfo);
            }
        }
    }
}
