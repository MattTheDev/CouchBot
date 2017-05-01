using Discord;
using Discord.WebSocket;
using MTD.CouchBot.Domain.Utilities;
using MTD.CouchBot.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MTD.CouchBot.Bot
{
    public class MessagingHelper
    {
        public static async Task<BroadcastMessage> BuildTestPublishedMessage(SocketUser user, ulong guildId, ulong channelId)
        {
            var servers = BotFiles.GetConfiguredServers();
            var server = servers.FirstOrDefault(x => x.Id == guildId);

            if (server == null)
                return null;

            string url = "http://" + (server.UseYouTubeGamingPublished ? "gaming" : "www") + ".youtube.com/watch?v=B7wkzmZ4GBw";

            EmbedBuilder embed = new EmbedBuilder();
            EmbedAuthorBuilder author = new EmbedAuthorBuilder();
            EmbedFooterBuilder footer = new EmbedFooterBuilder();


            if (server.PublishedMessage == null)
            {
                server.PublishedMessage = "%CHANNEL% just published a new video.";
            }

            Color red = new Color(179, 18, 23);
            author.IconUrl = user.GetAvatarUrl() + "?_=" + Guid.NewGuid().ToString().Replace("-", "");
            author.Name = "CouchBot";
            author.Url = url;
            footer.Text = "[YouTube] - " + DateTime.UtcNow.AddHours(server.TimeZoneOffset);
            footer.IconUrl = "http://couchbot.io/img/ytg.jpg";
            embed.Author = author;
            embed.Color = red;
            embed.Description = server.PublishedMessage.Replace("%CHANNEL%", "Test Channel").Replace("%TITLE%", "Test Title").Replace("%URL%", url);

            embed.Title = "Test Channel published a new video!";
            embed.ThumbnailUrl = "http://couchbot.io/img/bot/vader.jpg" + "?_=" + Guid.NewGuid().ToString().Replace("-", "");
            embed.ImageUrl = server.AllowThumbnails ? "http://couchbot.io/img/bot/test_thumbnail.jpg" + "?_=" + Guid.NewGuid().ToString().Replace("-", "") : "";
            embed.Footer = footer;

            var role = await DiscordHelper.GetRoleByGuildAndId(server.Id, server.MentionRole);

            if (role == null)
            {
                server.MentionRole = 0;
            }

            var message = (server.AllowEveryone ? server.MentionRole != 0 ? role.Mention : "@everyone " : "");

            if (server.UseTextAnnouncements)
            {
                if (!server.AllowThumbnails)
                {
                    url = "<" + url + ">";
                }

                message += "**[Test]** " + server.PublishedMessage.Replace("%CHANNEL%", "Test Channel").Replace("%TITLE%", "Test Title").Replace("%URL%", url);
            }

            var broadcastMessage = new BroadcastMessage()
            {
                GuildId = server.Id,
                ChannelId = channelId,
                UserId = "0",
                Message = message,
                Platform = "Test",
                Embed = (!server.UseTextAnnouncements ? embed.Build() : null)
            };

            return broadcastMessage;
        }

        public static async Task<BroadcastMessage> BuildTestMessage(SocketUser user, ulong guildId, ulong channelId, string platform)
        {
            var servers = BotFiles.GetConfiguredServers();
            var server = servers.FirstOrDefault(x => x.Id == guildId);

            if (server == null)
                return null;

            string gameName = "a game"; ;
            string url = "http://couchbot.io";

            EmbedBuilder embed = new EmbedBuilder();
            EmbedAuthorBuilder author = new EmbedAuthorBuilder();
            EmbedFooterBuilder footer = new EmbedFooterBuilder();

            if (server.LiveMessage == null)
            {
                server.LiveMessage = "%CHANNEL% just went live with %GAME% - %TITLE% - %URL%";
            }

            Color color = new Color(76, 144, 243);
            switch(platform)
            {
                case "twitch":
                    color = new Color(100, 65, 164);
                    footer.Text = "[Twitch] - " + DateTime.UtcNow.AddHours(server.TimeZoneOffset);
                    footer.IconUrl = "http://couchbot.io/img/twitch.jpg";
                    break;
                case "youtube":
                    color = new Color(179, 18, 23);
                    footer.Text = "[YouTube Gaming] - " + DateTime.UtcNow.AddHours(server.TimeZoneOffset);
                    footer.IconUrl = "http://couchbot.io/img/ytg.jpg";
                    break;
                case "hitbox":
                    color = new Color(153, 204, 0);
                    footer.Text = "[Hitbox] - " + DateTime.UtcNow.AddHours(server.TimeZoneOffset);
                    footer.IconUrl = "http://couchbot.io/img/hitbox.jpg";
                    break;
                case "beam":
                    color = new Color(76, 144, 243);
                    footer.Text = "[Beam] - " + DateTime.UtcNow.AddHours(server.TimeZoneOffset);
                    footer.IconUrl = "http://couchbot.io/img/beam.jpg";
                    break;
            }

            author.IconUrl = user.GetAvatarUrl() + "?_=" + Guid.NewGuid().ToString().Replace("-", "");
            author.Name = "CouchBot";
            author.Url = url;
            embed.Author = author;
            embed.Color = color;
            embed.Description = server.LiveMessage.Replace("%CHANNEL%", "Test Channel").Replace("%GAME%", gameName).Replace("%TITLE%", "Test Title").Replace("%URL%", url);
            embed.Title = "Test Channel has gone live!";
            embed.ThumbnailUrl = "http://couchbot.io/img/bot/vader.jpg" + "?_=" + Guid.NewGuid().ToString().Replace("-", "");
            embed.ImageUrl = server.AllowThumbnails ? "http://couchbot.io/img/bot/test_thumbnail.jpg" + "?_=" + Guid.NewGuid().ToString().Replace("-", "") : "";
            embed.Footer = footer;

            var role = await DiscordHelper.GetRoleByGuildAndId(server.Id, server.MentionRole);

            if (role == null)
            {
                server.MentionRole = 0;
            }

            var message = (server.AllowEveryone ? server.MentionRole != 0 ? role.Mention : "@everyone " : "");

            if (server.UseTextAnnouncements)
            {
                if (!server.AllowThumbnails)
                {
                    url = "<" + url + ">";
                }

                message += "**[Test]** " + server.LiveMessage.Replace("%CHANNEL%", "Test Channel").Replace("%GAME%", gameName).Replace("%TITLE%", "Test Title").Replace("%URL%", url);
            }

            var broadcastMessage = new BroadcastMessage()
            {
                GuildId = server.Id,
                ChannelId = channelId,
                UserId = "0",
                Message = message,
                Platform = "Test",
                Embed = (!server.UseTextAnnouncements ? embed.Build() : null)
            };

            return broadcastMessage;
        }
    }
}
