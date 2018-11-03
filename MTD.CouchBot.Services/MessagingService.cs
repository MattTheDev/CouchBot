using System;
using Discord;
using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using MTD.CouchBot.Domain.Models;
using MTD.CouchBot.Domain;
using MTD.CouchBot.Domain.Enums;
using MTD.CouchBot.Domain.Utilities;

namespace MTD.CouchBot.Services
{
    public class MessagingService : BaseService
    {
        private readonly DiscordSocketClient _discord;
        private readonly GuildInteractionService _guildInteractionService;
        private readonly IConfiguration _configuration;

        public MessagingService(DiscordSocketClient discord, GuildInteractionService guildInteractionService,
            IConfiguration configuration) : base(discord)
        {
            _discord = discord;
            _guildInteractionService = guildInteractionService;
            _configuration = configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseMessage"></param>
        /// <param name="platform"></param>
        /// <param name="guildId"></param>
        /// <param name="channelId"></param>
        /// <param name="avatarUrl"></param>
        /// <param name="gameName"></param>
        /// <param name="creatorContentTitle"></param>
        /// <param name="creatorContentUrl"></param>
        /// <param name="creatorChannelId"></param>
        /// <param name="creatorChannelName"></param>
        /// <returns></returns>
        public async Task<BroadcastMessage> BuildMessage(string baseMessage, Platform platform, string guildId,
            string channelId, string avatarUrl, string creatorChannelId, string creatorChannelName, string gameName,
            string creatorContentTitle, string creatorContentUrl, int followers, int totalViews, string thumbnailUrl)
        {
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder();
            var footer = new EmbedFooterBuilder();

            switch (platform)
            {
                case Platform.Mixer:
                    embed.Color = Constants.Blue;
                    embed.ThumbnailUrl = avatarUrl != null ?
                            avatarUrl + "?_=" + Guid.NewGuid().ToString().Replace("-", "") :
                            "https://mixer.com/_latest/assets/images/main/avatars/default.jpg";
                    footer.IconUrl = "http://couchbot.io/img/mixer2.png";
                    break;
                case Platform.Twitch:
                    embed.Color = Constants.Purple;
                    embed.ThumbnailUrl = avatarUrl != null ?
                            avatarUrl + "?_=" + Guid.NewGuid().ToString().Replace("-", "") :
                            "https://static-cdn.jtvnw.net/jtv_user_pictures/xarth/404_user_70x70.png";
                    footer.IconUrl = "http://couchbot.io/img/twitch.jpg";
                    break;
                case Platform.YouTube:
                    embed.Color = Constants.Red;
                    embed.ThumbnailUrl = avatarUrl + "?_=" + Guid.NewGuid().ToString().Replace("-", "");
                    footer.IconUrl = "http://couchbot.io/img/ytg.jpg";
                    break;
            }

            embed.Description = baseMessage
                    .Replace("%CHANNEL%", Format.Sanitize(creatorChannelName))
                    .Replace("%GAME%", gameName)
                    .Replace("%TITLE%", creatorContentTitle)
                    .Replace("%URL%", creatorContentUrl);

            if (!string.IsNullOrEmpty(gameName))
            {
                embed.Fields.Add(new EmbedFieldBuilder()
                {
                    IsInline = true,
                    Name = "Game",
                    Value = gameName
                });
            }

            embed.Fields.Add(new EmbedFieldBuilder()
            {
                IsInline = true,
                Name = "Followers",
                Value = followers
            });

            embed.Fields.Add(new EmbedFieldBuilder()
            {
                IsInline = true,
                Name = "Total Views",
                Value = totalViews
            });

            embed.ImageUrl = thumbnailUrl.Replace("{height}", "648").Replace("{width}", "1152");

            return new BroadcastMessage
            {
                Platform = platform,
                ChannelId = channelId,
                DeleteOffline = true,
                Embed = embed.Build(),
                GuildId = guildId,
                Message = baseMessage,
                CreatorChannelID = creatorChannelId
            };
        }

        //public async Task<BroadcastMessage> BuildMessage(string baseMessage, string channel, string gameName, string title, string url,
        //    string avatarUrl, string thumbnailUrl, string platform, string channelId, ulong discordChannelId, string teamName, )
        //{
        //    var embed = new EmbedBuilder();
        //    var author = new EmbedAuthorBuilder();
        //    var footer = new EmbedFooterBuilder();




        //    return new BroadcastMessage();
        //}

        //public async Task<BroadcastMessage> BuildMessage(string channel,
        //    string gameName, string title, string url, string avatarUrl, string thumbnailUrl, string platform,
        //    string channelId, DiscordServer server, ulong discordChannelId, string teamName, bool owner,
        //    int? viewers = null, int? totalViews = null, int? followers = null)
        //{
        //    var embed = new EmbedBuilder();
        //    var author = new EmbedAuthorBuilder();
        //    var footer = new EmbedFooterBuilder();

        //    if (server.LiveMessage == null)
        //    {
        //        server.LiveMessage = "%CHANNEL% just went live with %GAME% - %TITLE% - %URL%";
        //    }

        //    var botName = _discord.CurrentUser.Username;

        //    try
        //    {
        //        botName = ((IGuildUser)_discord.GetGuild(server.Id).GetUser(ulong.Parse(_configuration["Ids:BotId"])))
        //            .Nickname;
        //    }
        //    catch (Exception)
        //    {
        //        // Nothing. It's fine. Everything is fine.
        //    }

        //    author.IconUrl = _discord.CurrentUser.GetAvatarUrl() + "?_=" + Guid.NewGuid().ToString().Replace("-", "");
        //    author.Name = botName;
        //    author.Url = url;
        //    footer.Text = "[" + platform + "] - " + DateTime.UtcNow.AddHours(server.TimeZoneOffset);
        //    embed.Author = author;

        //    var allowEveryone = false;

        //    if (platform.Equals(Constants.Mixer))
        //    {

        //        allowEveryone = owner ? server.AllowMentionOwnerMixerLive : server.AllowMentionMixerLive;
        //    }
        //    else if (platform.Equals(Constants.YouTubeGaming))
        //    {

        //    }
        //    else if (platform.Equals(Constants.Twitch))
        //    {

        //        allowEveryone = owner ? server.AllowMentionOwnerTwitchLive : server.AllowMentionTwitchLive;
        //    }
        //    else if (platform.Equals(Constants.Smashcast))
        //    {
        //        embed.Color = Constants.Green;
        //        embed.ThumbnailUrl = avatarUrl + "?_=" + Guid.NewGuid().ToString().Replace("-", "");
        //        footer.IconUrl = "http://couchbot.io/img/smashcast2.png";
        //        allowEveryone = owner ? server.AllowMentionOwnerSmashcastLive : server.AllowMentionSmashcastLive;
        //    }
        //    else if (platform.Equals(Constants.Mobcrush))
        //    {
        //        embed.Color = Constants.Yellow;
        //        embed.ThumbnailUrl = avatarUrl + "?_=" + Guid.NewGuid().ToString().Replace("-", "");
        //        footer.IconUrl = "http://couchbot.io/img/mobcrush.jpg";
        //        allowEveryone = owner ? server.AllowMentionOwnerMobcrushLive : server.AllowMentionMobcrushLive;
        //    }


        //    embed.Title = channel + (string.IsNullOrEmpty(teamName) ? "" : " from the team '" + teamName + "'") + " has gone live!";
        //    embed.ImageUrl = server.AllowThumbnails ? thumbnailUrl + "?_=" + Guid.NewGuid().ToString().Replace("-", "") : "";
        //    embed.Footer = footer;

        //    if (server.DisplayStreamStatistics)
        //    {
        //        if (!platform.Equals(Constants.YouTubeGaming) && !platform.Equals(Constants.YouTubeGaming))
        //        {
        //            if (!string.IsNullOrEmpty(gameName))
        //            {
        //                embed.Fields.Add(new EmbedFieldBuilder()
        //                {
        //                    IsInline = true,
        //                    Name = "Game",
        //                    Value = gameName
        //                });
        //            }
        //        }

        //        if (followers != null)
        //        {
        //            embed.Fields.Add(new EmbedFieldBuilder()
        //            {
        //                IsInline = true,
        //                Name = "Followers",
        //                Value = followers
        //            });
        //        }

        //        if (totalViews != null)
        //        {
        //            embed.Fields.Add(new EmbedFieldBuilder()
        //            {
        //                IsInline = true,
        //                Name = "Total Views",
        //                Value = totalViews
        //            });
        //        }
        //    }

        //    var message = "";

        //    var broadcastMessage = new BroadcastMessage()
        //    {
        //        GuildId = server.Id,
        //        ChannelId = discordChannelId,
        //        UserId = channelId,
        //        Message = message,
        //        Platform = platform,
        //        Embed = (!server.UseTextAnnouncements ? embed.Build() : null),
        //        DeleteOffline = server.DeleteWhenOffline
        //    };

        //    return broadcastMessage;
        //}
    }
}