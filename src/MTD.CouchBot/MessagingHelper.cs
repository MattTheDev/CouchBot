using Discord;
using Discord.WebSocket;
using MTD.CouchBot.Domain.Utilities;
using MTD.CouchBot.Json;
using MTD.CouchBot.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MTD.CouchBot.Bot
{
    public class MessagingHelper
    {
        public static async Task<BroadcastMessage> BuildTestMessage(SocketUser user, ulong guildId, ulong channelId)
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

            Color blue = new Color(76, 144, 243);
            author.IconUrl = user.GetAvatarUrl() + "?_=" + Guid.NewGuid().ToString().Replace("-", "");
            author.Name = "CouchBot";
            author.Url = url;
            footer.Text = "[Beam] - " + DateTime.UtcNow.AddHours(server.TimeZoneOffset);
            footer.IconUrl = "http://couchbot.io/img/beam.jpg";
            embed.Author = author;
            embed.Color = blue;
            embed.Description = server.LiveMessage.Replace("%CHANNEL%", "Test Channel").Replace("%GAME%", gameName).Replace("%TITLE%", "Test Title").Replace("%URL%", url);
            embed.Title = "Test Channel has gone live!";
            embed.ThumbnailUrl = "https://beam.pro/_latest/assets/images/main/avatars/default.jpg";
            embed.ImageUrl = server.AllowThumbnails ? "https://thumbs.beam.pro/channel/170165.small.jpg" + "?_=" + Guid.NewGuid().ToString().Replace("-", "") : "";
            embed.Footer = footer;

            var message = (server.AllowEveryone ? server.MentionRole != 0 ? (await DiscordHelper.GetRoleByGuildAndId(server.Id, server.MentionRole)).Mention : "@everyone " : "");

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
