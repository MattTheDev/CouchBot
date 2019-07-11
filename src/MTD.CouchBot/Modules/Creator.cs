using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using MTD.CouchBot.Domain;
using MTD.CouchBot.Domain.Models.Bot;
using MTD.CouchBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MTD.CouchBot.Domain.Enums;
using MTD.CouchBot.Managers;

namespace MTD.CouchBot.Modules
{
    [Group("creator")]
    public class Creator : BaseModule
    {
        private readonly YouTubeManager _youtubeManager;
        private readonly MixerManager _mixerManager;
        private readonly TwitchManager _twitchManager;
        private readonly MobcrushManager _mobCrushManager;
        private readonly PiczelManager _piczelManager;
        private readonly FileService _fileService;
        private readonly DiscordShardedClient _discord;

        public Creator(YouTubeManager youTubeManager, MixerManager mixerManager, TwitchManager twitchManager, FileService fileService, MobcrushManager mobCrushManager,
            DiscordShardedClient discord, PiczelManager piczelManager, IOptions<BotSettings> botSettings) : base(botSettings, fileService)
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
            var builder = new EmbedBuilder();
            var authorBuilder = new EmbedAuthorBuilder();
            var footerBuilder = new EmbedFooterBuilder();

            var server = GetServer();

            authorBuilder.IconUrl = _discord.CurrentUser.GetAvatarUrl();
            authorBuilder.Name = _discord.CurrentUser.Username;
            authorBuilder.Url = "http://mattthedev.codes";

            footerBuilder.IconUrl = _discord.CurrentUser.GetAvatarUrl();
            footerBuilder.Text = $"[CouchBot] - {DateTime.UtcNow.AddHours(server.TimeZoneOffset)}";

            builder.Description = "To list the creators you've configured for your server, please type one of the following commands:";
            builder.Url = "http://mattthedev.codes";

            builder.Author = authorBuilder;
            builder.Footer = footerBuilder;

            builder.AddField(new EmbedFieldBuilder()
            {
                Name = "Commands",
                Value = "!cb creator list dlive\r\n" +
                        "!cb creator list mixer\r\n" +
                        "!cb creator list mobcrush\r\n" +
                        "!cb creator list picarto\r\n" +
                        "!cb creator list piczel\r\n" +
                        "!cb creator list smashcast\r\n" +
                        "!cb creator list twitch\r\n" +
                        "!cb creator list youtube\r\n",
                IsInline = false
            });

            await Context.Channel.SendMessageAsync("", false, builder.Build());
        }

        [Command("newList")]
        public async Task NewList(Platform platform)
        {
            var builder = new EmbedBuilder();
            var authorBuilder = new EmbedAuthorBuilder();
            var footerBuilder = new EmbedFooterBuilder();
            var server = GetServer();
            IUserMessage pendingMessage = null;

            authorBuilder.IconUrl = _discord.CurrentUser.GetAvatarUrl();
            authorBuilder.Name = _discord.CurrentUser.Username;
            authorBuilder.Url = "http://mattthedev.codes";

            footerBuilder.IconUrl = _discord.CurrentUser.GetAvatarUrl();
            footerBuilder.Text = $"[{_discord.CurrentUser.Username}] - {DateTime.UtcNow.AddHours(server.TimeZoneOffset)}";

            builder.Url = "http://mattthedev.codes";

            var creatorChannelList = new List<CreatorChannel>();

            var distinctDLive = server.Streamers.Where(s => s.Platform == Platform.DLive).Distinct();
            var distinctMixer = server.Streamers.Where(s => s.Platform == Platform.Mixer).Distinct();
            var distinctMobcrush = server.Streamers.Where(s => s.Platform == Platform.Mobcrush).Distinct();
            var distinctPicarto = server.Streamers.Where(s => s.Platform == Platform.Picarto).Distinct();
            var distinctPiczel = server.Streamers.Where(s => s.Platform == Platform.Piczel).Distinct();
            var distinctSmashcast = server.Streamers.Where(s => s.Platform == Platform.Smashcast).Distinct();
            var distinctYouTube = server.Streamers.Where(s => s.Platform == Platform.YouTube).Distinct();

            var distinctTwitchIds = server.Streamers.Where(s => s.Platform == Platform.Twitch).Select(s => s.StreamerChannelId).Distinct().ToList();
            var fullTwitchList = server.Streamers.Where(s => s.Platform == Platform.Twitch).ToList();

            switch (platform)
            {
                case Platform.Twitch:
                    pendingMessage =
                        await Context.Channel.SendMessageAsync($"🕐 Please wait - pulling channel data for { distinctTwitchIds.Count() } channels. 🕐");

                    foreach (var twitchId in distinctTwitchIds)
                    {
                        var twitchChannel = await _twitchManager.GetTwitchChannelById(twitchId);

                        foreach (var fullTwitch in fullTwitchList)
                        {
                            if (fullTwitch.StreamerChannelId.Equals(twitchId))
                            {
                                creatorChannelList.Add(new CreatorChannel
                                {
                                    CreatorChannelId = twitchId,
                                    CreatorChannelName = twitchChannel.DisplayName,
                                    Platform = Platform.Twitch,
                                    DiscordChannelId = fullTwitch.DisordChannelId
                                });
                            }
                        }
                    }

                    builder.ThumbnailUrl = Constants.TwitchLogoUrl;
                    break;
            }
            
            builder.AddField(new EmbedFieldBuilder()
            {
                Name = "Creators",
                Value = creatorChannelList.Count == 0 ? $"There are currently no creators set for {platform}." : 
                    string.Join(", ", creatorChannelList
                        .OrderBy(ccl => ccl.DiscordChannelId)
                        .ThenBy(ccl => ccl.CreatorChannelName)
                        .Select(ccl => $"{ccl.CreatorChannelName} (<#{ccl.DiscordChannelId}>)")),
                IsInline = false
            });

            if (pendingMessage != null)
            {
                await pendingMessage.DeleteAsync();
            }

            await Context.Channel.SendMessageAsync("", false, builder.Build());
        }

        class CreatorChannel
        {
            public string CreatorChannelName { get; set; }
            public string CreatorChannelId { get; set; }
            public ulong DiscordChannelId { get; set; }
            public Platform Platform { get; set; }
        }

        [Command("list")]
        public async Task List(string platform)
        {
            var builder = new EmbedBuilder();
            var authorBuilder = new EmbedAuthorBuilder();
            var footerBuilder = new EmbedFooterBuilder();

            var creators = new List<string>();
            var owner = "Not Set";

            var error = "";
            IUserMessage pendingMessage = null;

            var badList = new List<string>();
            var goodList = new List<string>();

            var server = GetServer();

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
                    
                    builder.ThumbnailUrl = Constants.MixerLogoUrl;

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

                    builder.ThumbnailUrl = Constants.MobcrushLogoUrl;

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

                    builder.ThumbnailUrl = Constants.PiczelLogoUrl;

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
                    builder.ThumbnailUrl = Constants.SmashcastLogoUrl;

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

                    builder.ThumbnailUrl = Constants.TwitchLogoUrl;

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

                    builder.ThumbnailUrl = Constants.YouTubeLogoUrl;

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
            footerBuilder.Text = $"[{_discord.CurrentUser.Username}] - {DateTime.UtcNow.AddHours(server.TimeZoneOffset)}";

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
    }
}
