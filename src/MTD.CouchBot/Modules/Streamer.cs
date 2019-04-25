using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MTD.CouchBot.Managers;
using MTD.CouchBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTD.CouchBot.Modules
{
    [Group("streamer"), Alias("creator")]
    public class Streamer : ModuleBase
    {
        private readonly IYouTubeManager _youtubeManager;
        private readonly IMixerManager _mixerManager;
        private readonly ITwitchManager _twitchManager;
        private readonly IMobcrushManager _mobCrushManager;
        private readonly IPiczelManager _piczelManager;
        private readonly FileService _fileService;
        private readonly DiscordShardedClient _discord;

        public Streamer(IYouTubeManager youTubeManager, IMixerManager mixerManager, ITwitchManager twitchManager, FileService fileService, IMobcrushManager mobCrushManager,
            DiscordShardedClient discord, IPiczelManager piczelManager)
        {
            _youtubeManager = youTubeManager;
            _twitchManager = twitchManager;
            _mixerManager = mixerManager;
            _fileService = fileService;
            _mobCrushManager = mobCrushManager;
            _discord = discord;
            _piczelManager = piczelManager;
        }

        [Command("list")]
        public async Task List()
        {
            var server = _fileService.GetConfiguredServerById(Context.Guild.Id);

            var builder = new EmbedBuilder();
            var authorBuilder = new EmbedAuthorBuilder();
            var footerBuilder = new EmbedFooterBuilder();

            authorBuilder.IconUrl = _discord.CurrentUser.GetAvatarUrl();
            authorBuilder.Name = _discord.CurrentUser.Username;
            authorBuilder.Url = "http://mattthedev.codes";

            footerBuilder.IconUrl = _discord.CurrentUser.GetAvatarUrl();
            footerBuilder.Text = $"[CouchBot] - {DateTime.UtcNow.AddHours(server.TimeZoneOffset)}";

            builder.Description = "To list the streamers you've configured for your server, please type one of the following commands:";
            builder.Url = "http://mattthedev.codes";

            builder.Author = authorBuilder;
            builder.Footer = footerBuilder;

            builder.AddField(new EmbedFieldBuilder()
            {
                Name = "Commands",
                Value = "!cb streamer list mixer\r\n" +
                        "!cb streamer list mobcrush\r\n" +
                        "!cb streamer list picarto\r\n" +
                        "!cb streamer list piczel\r\n" +
                        "!cb streamer list smashcast\r\n" +
                        "!cb streamer list twitch\r\n" +
                        "!cb streamer list youtube\r\n",
                IsInline = false
            });

            await Context.Channel.SendMessageAsync("", false, builder.Build());
        }

        [Command("list")]
        public async Task List(string platform)
        {
            var server = _fileService.GetConfiguredServerById(Context.Guild.Id);

            var builder = new EmbedBuilder();
            var authorBuilder = new EmbedAuthorBuilder();
            var footerBuilder = new EmbedFooterBuilder();

            var creators = new List<string>();
            var owner = "Not Set";

            var error = "";
            IUserMessage pendingMessage = null;

            var badList = new List<string>();
            var goodList = new List<string>();

            switch (platform.ToLower())
            {               
                case "mixer":
                    if (server.ServerBeamChannelIds != null && server.ServerBeamChannelIds.Count > 0)
                    {
                        pendingMessage =
                            await Context.Channel.SendMessageAsync($"🕐 Please wait - pulling channel data for { server.ServerBeamChannelIds.Distinct().Count() } channels. 🕐");

                        foreach (var streamer in server.ServerBeamChannelIds.Distinct())
                        {
                            var channel = await _mixerManager.GetChannelById(streamer);

                            if (channel != null)
                            {
                                creators.Add(channel.user.username);
                                goodList.Add(streamer);
                            }
                            else
                            {
                                badList.Add(streamer);
                            }
                        }

                        if(badList.Count > 0)
                        {
                            server.ServerBeamChannelIds = goodList;

                            _fileService.SaveDiscordServer(server);
                        }
                    }

                    owner = "Not Set";

                    if (!string.IsNullOrEmpty(server.OwnerBeamChannelId))
                    {
                        var channel = await _mixerManager.GetChannelById(server.OwnerBeamChannelId);

                        if(channel != null)
                        {
                            owner = channel.user.username;
                        }
                        else
                        {
                            server.OwnerBeamChannelId = null;

                            _fileService.SaveDiscordServer(server);
                        }
                    }
                    
                    builder.ThumbnailUrl = "http://mattthedev.codes/img/mixer2.png";

                    break;
                case "mobcrush":
                    if (server.ServerMobcrushIds != null && server.ServerMobcrushIds.Count > 0)
                    {
                        pendingMessage =
                            await Context.Channel.SendMessageAsync($"🕐 Please wait - pulling channel data for { server.ServerMobcrushIds.Distinct().Count() } channels. 🕐");

                        foreach (var streamer in server.ServerMobcrushIds.Distinct())
                        {
                            var channel = await _mobCrushManager.GetMobcrushChannelById(streamer);

                            if (channel != null)
                            {
                                creators.Add(channel.name);
                                goodList.Add(streamer);
                            }
                            else
                            {
                                badList.Add(streamer);
                            }
                        }

                        if (badList.Count > 0)
                        {
                            server.ServerBeamChannelIds = goodList;

                            _fileService.SaveDiscordServer(server);
                        }
                    }

                    owner = "Not Set";

                    if (!string.IsNullOrEmpty(server.OwnerMobcrushId))
                    {
                        var channel = await _mobCrushManager.GetMobcrushChannelById(server.OwnerMobcrushId);

                        if (channel != null)
                        {
                            owner = channel.name;
                        }
                        else
                        {
                            server.OwnerMobcrushId = null;

                            _fileService.SaveDiscordServer(server);
                        }
                    }

                    builder.ThumbnailUrl = "http://mattthedev.codes/img/mobcrush.jpg";

                    break;
                case "picarto":
                    if (server.PicartoChannels != null && server.PicartoChannels.Count > 0)
                    {
                        pendingMessage =
                            await Context.Channel.SendMessageAsync($"🕐 Please wait - pulling channel data for { server.PicartoChannels.Distinct().Count() } channels. 🕐");

                        foreach (var streamer in server.PicartoChannels.Distinct())
                        {
                            creators.Add(streamer);
                        }
                    }

                    owner = (string.IsNullOrEmpty(server.OwnerPicartoChannel) ? "Not Set" : server.OwnerPicartoChannel);
                    builder.ThumbnailUrl = "https://picarto.tv/images/Picarto_logo.png";

                    break;
                case "piczel":
                    if (server.ServerPiczelChannelIds != null && server.ServerPiczelChannelIds.Count > 0)
                    {
                        pendingMessage =
                            await Context.Channel.SendMessageAsync($"🕐 Please wait - pulling channel data for { server.ServerPiczelChannelIds.Distinct().Count() } channels. 🕐");

                        foreach (var streamer in server.ServerPiczelChannelIds.Distinct())
                        {

                            var piczelChannel = await _piczelManager.GetStreamById(streamer);

                            if (piczelChannel != null && piczelChannel.Streams.Count > 0)
                            {
                                creators.Add(piczelChannel.Streams[0].Username);
                                goodList.Add(streamer.ToString());
                            }
                            else
                            {
                                badList.Add(streamer.ToString());
                            }
                        }

                        if (badList.Count > 0)
                        {
                            server.ServerPiczelChannelIds = goodList.Select(x => int.Parse(x)).ToList();
                            _fileService.SaveDiscordServer(server);
                        }
                    }

                    owner = "Not Set";

                    if (server.OwnerPiczelChannelId.HasValue)
                    {
                        var channel = await _piczelManager.GetStreamById(server.OwnerPiczelChannelId.Value);

                        if (channel != null && channel.Streams.Count > 0)
                        {
                            owner = channel.Streams[0].Username;
                        }
                        else
                        {
                            server.OwnerPiczelChannelId = null;
                            _fileService.SaveDiscordServer(server);
                        }
                    }

                    builder.ThumbnailUrl = "http://mattthedev.codes/img/piczeltv.png";

                    break;
                case "smashcast":
                    if (server.ServerHitboxChannels != null && server.ServerHitboxChannels.Count > 0)
                    {
                        pendingMessage =
                            await Context.Channel.SendMessageAsync($"🕐 Please wait - pulling channel data for { server.ServerHitboxChannels.Distinct().Count() } channels. 🕐");

                        foreach (var streamer in server.ServerHitboxChannels.Distinct())
                        {
                            creators.Add(streamer);
                        }
                    }

                    owner = (string.IsNullOrEmpty(server.OwnerHitboxChannel) ? "Not Set" : server.OwnerHitboxChannel);
                    builder.ThumbnailUrl = "http://mattthedev.codes/img/smashcast2.png";

                    break;
                case "twitch":
                    if (server.ServerTwitchChannelIds != null && server.ServerTwitchChannelIds.Count > 0)
                    {
                        pendingMessage =
                            await Context.Channel.SendMessageAsync($"🕐 Please wait - pulling channel data for { server.ServerTwitchChannelIds.Distinct().Count() } channels. 🕐");

                        foreach (var streamer in server.ServerTwitchChannelIds.Distinct())
                        {

                            var twitchChannel = await _twitchManager.GetTwitchChannelById(streamer);

                            if (twitchChannel != null)
                            {
                                creators.Add(twitchChannel.Name);
                                goodList.Add(streamer);
                            }
                            else
                            {
                                badList.Add(streamer);
                            }
                        }

                        if (badList.Count > 0)
                        {
                            server.ServerTwitchChannelIds = goodList;

                            _fileService.SaveDiscordServer(server);
                        }
                    }

                    owner = "Not Set";

                    if (!string.IsNullOrEmpty(server.OwnerTwitchChannelId))
                    {
                        var channel = await _twitchManager.GetTwitchChannelById(server.OwnerTwitchChannelId);

                        if (channel != null)
                        {
                            owner = channel.Name;
                        }
                        else
                        {
                            server.OwnerTwitchChannelId = null;

                            _fileService.SaveDiscordServer(server);
                        }
                    }

                    builder.ThumbnailUrl = "http://mattthedev.codes/img/twitch2.png";

                    break;
                case "youtube":
                    if (server.ServerYouTubeChannelIds != null && server.ServerYouTubeChannelIds.Count > 0)
                    {
                        pendingMessage =
                            await Context.Channel.SendMessageAsync($"🕐 Please wait - pulling channel data for { server.ServerYouTubeChannelIds.Distinct().Count() } channels. 🕐");

                        foreach (var streamer in server.ServerYouTubeChannelIds.Distinct())
                        {
                            var channel = await _youtubeManager.GetYouTubeChannelSnippetById(streamer);

                            creators.Add((channel.items.Count > 0 ? channel.items[0].snippet.title + " (" + streamer + ")" : streamer));
                        }
                    }

                    if (!string.IsNullOrEmpty(server.OwnerYouTubeChannelId))
                    {
                        var channel = await _youtubeManager.GetYouTubeChannelSnippetById(server.OwnerYouTubeChannelId);

                        if (channel != null && channel.items.Count > 0)
                        {
                            owner = channel.items[0].snippet.title + " (" + server.OwnerYouTubeChannelId + ")";
                        }
                    }

                    builder.ThumbnailUrl = "http://mattthedev.codes/img/yt.png";

                    break;
                default:
                    error = "Please pass in a valid platform. Valid platforms are: mixer, mobcrush, picarto, twitch, smashcast, or youtube.";
                    break;
            }

            creators.Sort();
            var listCombinedStrings = new List<string>();
            var combined = "";
            var maxLength = 1000;
            foreach (var creator in creators)
            {
                if ((combined + ", " + creator).Length >= maxLength)
                {
                    combined = combined.Trim().TrimEnd(',');
                    listCombinedStrings.Add(combined);
                    combined = creator + ", ";
                }
                else
                {
                    combined += creator + ", ";
                }
            }

            combined = combined.Trim().TrimEnd(',');
            listCombinedStrings.Add(combined);

            authorBuilder.IconUrl = _discord.CurrentUser.GetAvatarUrl();
            authorBuilder.Name = _discord.CurrentUser.Username;
            authorBuilder.Url = "http://mattthedev.codes";

            footerBuilder.IconUrl = _discord.CurrentUser.GetAvatarUrl();
            footerBuilder.Text = $"[CouchBot] - {DateTime.UtcNow.AddHours(server.TimeZoneOffset)}";

            builder.Url = "http://mattthedev.codes";

            if (!string.IsNullOrEmpty(error))
            {
                builder.AddField(new EmbedFieldBuilder()
                {
                    Name = "Error!",
                    Value = error,
                    IsInline = false
                });
            }
            else
            {
                foreach (var creatorString in listCombinedStrings)
                {

                    builder.AddField(new EmbedFieldBuilder()
                    {
                        Name = "Creators",
                        Value = string.IsNullOrEmpty(creatorString) ? "Not Set" : creatorString,
                        IsInline = false
                    });
                }

                builder.AddField(new EmbedFieldBuilder()
                {
                    Name = "Owner",
                    Value = owner,
                    IsInline = false
                });
            }

            builder.Author = authorBuilder;
            builder.Footer = footerBuilder;

            await Context.Channel.SendMessageAsync("", false, builder.Build());

            if (pendingMessage != null)
            {
                await pendingMessage.DeleteAsync();
            }

            // Cleanup duplicates
            server.ServerBeamChannelIds = server.ServerBeamChannelIds?.Distinct().ToList();
            server.ServerHitboxChannels = server.ServerHitboxChannels?.Distinct().ToList();
            server.ServerMobcrushIds = server.ServerMobcrushIds?.Distinct().ToList();
            server.ServerTwitchChannelIds = server.ServerTwitchChannelIds?.Distinct().ToList();
            server.ServerYouTubeChannelIds = server.ServerYouTubeChannelIds?.Distinct().ToList();

            _fileService.SaveDiscordServer(server);
        }

        [Command("live"), Summary("Display who is currently live in a server.")]
        public async Task Live()
        {
            var beam = _fileService.GetCurrentlyLiveBeamChannels();
            var hitbox = _fileService.GetCurrentlyLiveHitboxChannels();
            var twitch = _fileService.GetCurrentlyLiveTwitchChannels();
            var youtube = _fileService.GetCurrentlyLiveYouTubeChannels();
            var picarto = _fileService.GetCurrentlyLivePicartoChannels();


            var guildId = Context.Guild.Id;

            var beamLive = "";
            var hitboxLive = "";
            var twitchLive = "";
            var youtubeLive = "";
            var picartoLive = "";

            foreach (var b in beam)
            {
                foreach (var cm in b.ChannelMessages)
                {
                    if (cm.GuildId == guildId)
                    {
                        var channel = await _mixerManager.GetChannelById(b.Name);

                        if (channel != null && channel.online)
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

                        if (channel?.stream != null)
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

            var info = "```Markdown\r\n" +
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
