using Discord;
using Discord.Commands;
using MTD.CouchBot.Domain;
using MTD.CouchBot.Domain.Models.Bot;
using MTD.CouchBot.Domain.Utilities;
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
        IYouTubeManager _youtubeManager;
        IMixerManager _mixerManager;
        ITwitchManager _twitchManager;
        IVidMeManager _vidMeManager;

        public Streamer()
        {
            _youtubeManager = new YouTubeManager();
            _vidMeManager = new VidMeManager();
            _twitchManager = new TwitchManager();
            _mixerManager = new MixerManager();
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
            string picarto = "";
            string vidMe = "";

            int count = 0;

            if (guildObject.PicartoChannels != null && guildObject.PicartoChannels.Count > 0)
            {
                foreach (var streamer in guildObject.PicartoChannels)
                {
                    if (count == 0)
                    {
                        picarto += streamer;
                    }
                    else
                    {
                        picarto += ", " + streamer;
                    }

                    count++;
                }
            }

            count = 0;

            if (guildObject.ServerTwitchChannelIds != null && guildObject.ServerTwitchChannelIds.Count > 0)
            {
                foreach (var streamer in guildObject.ServerTwitchChannelIds)
                {
                    var twitchChannel = await _twitchManager.GetTwitchChannelById(streamer);

                    var name = twitchChannel == null ? streamer + " (Streamer Not Found)" : twitchChannel.DisplayName;

                    if (count == 0)
                    {
                        twitch += name;
                    }
                    else
                    {
                        twitch += ", " + name;
                    }

                    count++;
                }
            }

            count = 0;

            if (guildObject.ServerYouTubeChannelIds != null && guildObject.ServerYouTubeChannelIds.Count > 0)
            {
                foreach (var streamer in guildObject.ServerYouTubeChannelIds)
                {
                    var channel = await _youtubeManager.GetYouTubeChannelSnippetById(streamer);

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

            count = 0;

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

            if (guildObject.ServerVidMeChannels != null && guildObject.ServerVidMeChannels.Count > 0)
            {
                foreach (var streamer in guildObject.ServerVidMeChannels)
                {
                    if (count == 0)
                    {
                        vidMe += streamer;
                    }
                    else
                    {
                        vidMe += ", " + streamer;
                    }

                    count++;
                }
            }

            count = 0;

            var ownerYouTube = "Not Set";

            if(!string.IsNullOrEmpty(guildObject.OwnerYouTubeChannelId))
            {
                var channel = await _youtubeManager.GetYouTubeChannelSnippetById(guildObject.OwnerYouTubeChannelId);

                if(channel != null && channel.items.Count > 0)
                {
                    ownerYouTube = channel.items[0].snippet.title + " (" + guildObject.OwnerYouTubeChannelId + ")";
                }
            }

            var ownerVidMe = "Not Set";

            if (!string.IsNullOrEmpty(guildObject.OwnerVidMeChannel))
            {
                ownerVidMe = guildObject.OwnerVidMeChannel;
            }

            string info = "```Markdown\r\n" +
              "# Server Configured Channels\r\n" +
              "- Mixer: " + beam + "\r\n" +
              "- Picarto: " + picarto + "\r\n" +
              "- Smashcast: " + hitbox + "\r\n" +
              "- Twitch: " + twitch + "\r\n" +
              "- VidMe: " + vidMe + "\r\n" +
              "- YouTube: " + youtube + "\r\n" +
              "- Owner Mixer: " + (string.IsNullOrEmpty(guildObject.OwnerBeamChannel) ? "Not Set" : guildObject.OwnerBeamChannel) + "\r\n" +
              "- Owner Picarto: " + (string.IsNullOrEmpty(guildObject.OwnerPicartoChannel) ? "Not Set" : guildObject.OwnerPicartoChannel) + "\r\n" +
              "- Owner Smashcast: " + (string.IsNullOrEmpty(guildObject.OwnerHitboxChannel) ? "Not Set" : guildObject.OwnerHitboxChannel) + "\r\n" +
              "- Owner Twitch: " + (string.IsNullOrEmpty(guildObject.OwnerTwitchChannel) ? "Not Set" : guildObject.OwnerTwitchChannel) + "\r\n" +
              "- Owner VidMe: " + ownerVidMe + "\r\n" +
              "- Owner YouTube: " + ownerYouTube + "\r\n" +
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
            var picarto = BotFiles.GetCurrentlyLivePicartoChannels();


            var guildId = Context.Guild.Id;

            var beamLive = "";
            var hitboxLive = "";
            var twitchLive = "";
            var youtubeLive = "";
            var picartoLive = "";

            foreach(var b in beam)
            {
                foreach(var cm in b.ChannelMessages)
                {
                    if(cm.GuildId == guildId)
                    {
                        var channel = await _mixerManager.GetChannelById(b.Name);

                        if(channel != null && channel.online)
                        beamLive += channel.token + ", ";

                        break;
                    }
                }
            }

            foreach (var p in picarto)
            {
                foreach (var cm in p.ChannelMessages)
                {
                    if (cm.GuildId == guildId)
                    {
                        var channel = await _mixerManager.GetChannelById(p.Name);

                        if (channel != null && channel.online)
                            picartoLive += channel.token + ", ";

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
                        var channel = await _twitchManager.GetStreamById(t.Name);

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
                        var channel = await _youtubeManager.GetLiveVideoByChannelId(yt.Name);

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
            picartoLive = picartoLive.Trim().TrimEnd(',');

            string info = "```Markdown\r\n" +
              "# Currently Live\r\n" +
              "- Mixer: " + beamLive + "\r\n" +
              "- Picarto: " + picartoLive + "\r\n" +
              "- Smashcast: " + hitboxLive + "\r\n" +
              "- Twitch: " + twitchLive + "\r\n" +
              "- YouTube Gaming: " + youtubeLive + "\r\n" +
              "```\r\n";

            await Context.Channel.SendMessageAsync(info);
        }
    }
}
