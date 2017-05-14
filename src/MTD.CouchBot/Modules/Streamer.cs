using Discord;
using Discord.Commands;
using MTD.CouchBot.Domain;
using MTD.CouchBot.Domain.Utilities;
using MTD.CouchBot.Json;
using MTD.CouchBot.Managers;
using MTD.CouchBot.Managers.Implementations;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace MTD.CouchBot.Modules
{
    [Group("streamer")]
    public class Streamer : ModuleBase
    {
        IYouTubeManager youtubeManager;
        IBeamManager beamManager;
        ISmashcastManager smashcastManager;
        ITwitchManager twitchManager;

        public Streamer()
        {
            youtubeManager = new YouTubeManager();
        }

        [Command("list"), Summary("List server streamers")]
        public async Task List()
        {
            var guild = ((IGuildUser)Context.Message.Author).Guild;
            var guildObject = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(Constants.ConfigRootDirectory + Constants.GuildDirectory + guild.Id + ".json"));
            string twitch = "";
            string youtube = "";
            string beam = "";
            string hitbox = "";

            int count = 0;

            if (guildObject.ServerTwitchChannels != null && guildObject.ServerTwitchChannels.Count > 0)
            {
                foreach (var streamer in guildObject.ServerTwitchChannels)
                {
                    if (count == 0)
                    {
                        twitch += streamer;
                    }
                    else
                    {
                        twitch += ", " + streamer;
                    }

                    count++;
                }
            }

            count = 0;

            if (guildObject.ServerYouTubeChannelIds != null && guildObject.ServerYouTubeChannelIds.Count > 0)
            {
                foreach (var streamer in guildObject.ServerYouTubeChannelIds)
                {
                    var channel = await youtubeManager.GetYouTubeChannelSnippetById(streamer);

                    if (count == 0)
                    {
                        youtube += (channel.items.Count > 0 ? channel.items[0].snippet.title + " (" +  streamer + ")" : streamer);
                    }
                    else
                    {
                        youtube += ", " + (channel.items.Count > 0 ? channel.items[0].snippet.title + " (" + streamer + ")" : streamer);
                    }

                    count++;
                }
            }

            count = 0;

            if (guildObject.ServerBeamChannels != null && guildObject.ServerBeamChannels.Count > 0)
            {
                foreach (var streamer in guildObject.ServerBeamChannels)
                {
                    if (count == 0)
                    {
                        beam += streamer;
                    }
                    else
                    {
                        beam += ", " + streamer;
                    }

                    count++;
                }
            }

            if (guildObject.ServerHitboxChannels != null && guildObject.ServerHitboxChannels.Count > 0)
            {
                foreach (var streamer in guildObject.ServerHitboxChannels)
                {
                    if (count == 0)
                    {
                        hitbox += streamer;
                    }
                    else
                    {
                        hitbox += ", " + streamer;
                    }

                    count++;
                }
            }

            string info = "```Markdown\r\n" +
              "# Server Configured Channels\r\n" +
              "- Beam: " + beam + "\r\n" +
              "- Smashcast: " + hitbox + "\r\n" +
              "- Twitch: " + twitch + "\r\n" +
              "- YouTube Gaming: " + youtube + "\r\n" +
              "```\r\n";

            await Context.Channel.SendMessageAsync(info);
        }

        [Command("live"), Summary("Display who is currently live in a server.")]
        public async Task Live()
        {
            var beam = BotFiles.GetCurrentlyLiveBeamChannels();
            var hitbox = BotFiles.GetCurrentlyLiveHitboxChannels();
            var twitch = BotFiles.GetCurrentlyLiveTwitchChannels();
            var youtube = BotFiles.GetCurrentlyLiveYouTubeChannels();

            var guildId = Context.Guild.Id;

            var beamLive = "";
            var hitboxLive = "";
            var twitchLive = "";
            var youtubeLive = "";

            foreach(var b in beam)
            {
                foreach(var cm in b.ChannelMessages)
                {
                    if(cm.GuildId == guildId)
                    {
                        var channel = await beamManager.GetBeamChannelById(b.Name);

                        if(channel != null && channel.online)
                        beamLive += channel.token + ", ";

                        break;
                    }
                }
            }

            foreach (var h in hitbox)
            {
                foreach (var cm in h.ChannelMessages)
                {
                    if (cm.GuildId == guildId)
                    {
                        hitboxLive += h.Name + ", ";

                        break;
                    }
                }
            }

            foreach (var t in twitch)
            {
                foreach (var cm in t.ChannelMessages)
                {
                    if (cm.GuildId == guildId)
                    {
                        var channel = await twitchManager.GetStreamById(t.Name);

                        if (channel != null && channel.stream != null)
                        {
                            twitchLive += channel.stream.channel.name + ", ";
                        }

                        break;
                    }
                }
            }

            foreach (var yt in youtube)
            {
                foreach (var cm in yt.ChannelMessages)
                {
                    if (cm.GuildId == guildId)
                    {
                        var channel = await youtubeManager.GetLiveVideoByChannelId(yt.Name);

                        if (channel != null && channel.items != null && channel.items.Count > 0)
                        {
                            youtubeLive += channel.items[0].snippet.channelTitle + ", ";
                        }

                        break;
                    }
                }
            }

            beamLive = beamLive.Trim().TrimEnd(',');
            hitboxLive = hitboxLive.Trim().TrimEnd(',');
            twitchLive = twitchLive.Trim().TrimEnd(',');
            youtubeLive = youtubeLive.Trim().TrimEnd(',');

            string info = "```Markdown\r\n" +
              "# Currently Live\r\n" +
              "- Beam: " + beamLive + "\r\n" +
              "- Smashcast: " + hitboxLive + "\r\n" +
              "- Twitch: " + twitchLive + "\r\n" +
              "- YouTube Gaming: " + youtubeLive + "\r\n" +
              "```\r\n";

            await Context.Channel.SendMessageAsync(info);
        }
    }
}
