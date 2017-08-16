using Discord;
using Discord.WebSocket;
using MTD.CouchBot.Domain;
using MTD.CouchBot.Domain.Models.Bot;
using MTD.CouchBot.Domain.Utilities;
using MTD.CouchBot.Managers;
using MTD.CouchBot.Managers.Implementations;
using MTD.CouchBot.Models.Bot;
using System;
using System.Collections.Generic;
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
            author.Name = Program.client.CurrentUser.Username;
            author.Url = url;
            footer.Text = "[" + Constants.YouTube + "] - " + DateTime.UtcNow.AddHours(server.TimeZoneOffset);
            footer.IconUrl = "http://couchbot.io/img/ytg.jpg";
            embed.Author = author;
            embed.Color = red;
            embed.Description = server.PublishedMessage.Replace("%CHANNEL%", "Test Channel").Replace("%TITLE%", "Test Title").Replace("%URL%", url);

            embed.Title = "Test Channel published a new video!";
            embed.ThumbnailUrl = "http://couchbot.io/img/bot/vader.jpg" + "?_=" + Guid.NewGuid().ToString().Replace("-", "");
            embed.ImageUrl = server.AllowThumbnails ? "http://couchbot.io/img/bot/test_thumbnail.jpg" + "?_=" + Guid.NewGuid().ToString().Replace("-", "") : "";
            embed.Footer = footer;

            var role = await DiscordHelper.GetRoleByGuildAndId(server.Id, server.MentionRole);
            var roleName = "";

            if (role == null)
            {
                server.MentionRole = 0;
            }

            if (server.MentionRole == 0)
            {
                roleName = "@everyone";
            }
            else if (server.MentionRole == 1)
            {
                roleName = "@here";
            }
            else
            {
                roleName = role.Mention;
            }

            var message = (server.AllowEveryone ? roleName + " " : "");

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
            if (platform.Equals(Constants.Twitch))
            {
                color = new Color(100, 65, 164);
                footer.Text = "[" + Constants.Twitch + "] - " + DateTime.UtcNow.AddHours(server.TimeZoneOffset);
                footer.IconUrl = "http://couchbot.io/img/twitch.jpg";
            }
            else if (platform.Equals(Constants.YouTube) || platform.Equals(Constants.YouTubeGaming))
            {
                color = new Color(179, 18, 23);
                footer.Text = "[" + Constants.YouTubeGaming + "] - " + DateTime.UtcNow.AddHours(server.TimeZoneOffset);
                footer.IconUrl = "http://couchbot.io/img/ytg.jpg";
            }
            else if (platform.Equals(Constants.Smashcast))
            {
                color = new Color(153, 204, 0);
                footer.Text = "[" + Constants.Smashcast + "] - " + DateTime.UtcNow.AddHours(server.TimeZoneOffset);
                footer.IconUrl = "http://couchbot.io/img/smashcast.png";
            }
            else if(platform.Equals(Constants.Mixer))
            { 
                    color = new Color(76, 144, 243);
                    footer.Text = "[" + Constants.Mixer + "] - " + DateTime.UtcNow.AddHours(server.TimeZoneOffset);
                    footer.IconUrl = "http://couchbot.io/img/beam.jpg";
            }

            author.IconUrl = (user.GetAvatarUrl() != null ? user.GetAvatarUrl() : "http://couchbot.io/img/bot/discord.png") + "?_=" + Guid.NewGuid().ToString().Replace("-", "");
            author.Name = Program.client.CurrentUser.Username;
            author.Url = url;
            embed.Author = author;
            embed.Color = color;
            embed.Description = server.LiveMessage.Replace("%CHANNEL%", "Test Channel").Replace("%GAME%", gameName).Replace("%TITLE%", "Test Title").Replace("%URL%", url);
            embed.Title = "Test Channel has gone live!";
            embed.ThumbnailUrl = "http://couchbot.io/img/bot/vader.jpg" + "?_=" + Guid.NewGuid().ToString().Replace("-", "");
            embed.ImageUrl = server.AllowThumbnails ? "http://couchbot.io/img/bot/test_thumbnail.jpg" + "?_=" + Guid.NewGuid().ToString().Replace("-", "") : "";
            embed.Footer = footer;

            var role = await DiscordHelper.GetRoleByGuildAndId(server.Id, server.MentionRole);
            var roleName = "";

            if (role == null && server.MentionRole != 1)
            {
                server.MentionRole = 0;
            }

            if (server.MentionRole == 0)
            {
                roleName = "@everyone";
            }
            else if (server.MentionRole == 1)
            {
                roleName = "@here";
            }
            else
            {
                roleName = role.Mention;
            }

            var message = (server.AllowEveryone ? roleName + " " : "");

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

        public static async Task<BroadcastMessage> BuildMessage(string channel, 
            string gameName, string title, string url, string avatarUrl, string thumbnailUrl, string platform, 
            string channelId, DiscordServer server, ulong discordChannelId, string teamName)
        {
            EmbedBuilder embed = new EmbedBuilder();
            EmbedAuthorBuilder author = new EmbedAuthorBuilder();
            EmbedFooterBuilder footer = new EmbedFooterBuilder();

            if (server.LiveMessage == null)
            {
                server.LiveMessage = "%CHANNEL% just went live with %GAME% - %TITLE% - %URL%";
            }

            author.IconUrl = Program.client.CurrentUser.GetAvatarUrl() + "?_=" + Guid.NewGuid().ToString().Replace("-", "");
            author.Name = Program.client.CurrentUser.Username;
            author.Url = url;
            footer.Text = "[" + platform + "] - " + DateTime.UtcNow.AddHours(server.TimeZoneOffset);
            embed.Author = author;

            if (platform.Equals(Constants.Mixer))
            {
                embed.Color = Constants.Blue;
                embed.ThumbnailUrl = avatarUrl != null ?
                        avatarUrl + "?_=" + Guid.NewGuid().ToString().Replace("-", "") :
                        "https://mixer.com/_latest/assets/images/main/avatars/default.jpg";
                footer.IconUrl = "http://couchbot.io/img/beam.jpg";
            }
            else if (platform.Equals(Constants.YouTubeGaming))
            {
                embed.Color = Constants.Red;
                embed.ThumbnailUrl = avatarUrl + "?_=" + Guid.NewGuid().ToString().Replace("-", "");
                footer.IconUrl = "http://couchbot.io/img/ytg.jpg";
            }
            else if (platform.Equals(Constants.Twitch))
            {
                embed.Color = Constants.Purple;
                embed.ThumbnailUrl = avatarUrl != null ?
                        avatarUrl + "?_=" + Guid.NewGuid().ToString().Replace("-", "") :
                        "https://static-cdn.jtvnw.net/jtv_user_pictures/xarth/404_user_70x70.png";
                footer.IconUrl = "http://couchbot.io/img/twitch.jpg";
            }
            else if (platform.Equals(Constants.Smashcast))
            {
                embed.Color = Constants.Green;
                embed.ThumbnailUrl = avatarUrl + "?_=" + Guid.NewGuid().ToString().Replace("-", "");
                footer.IconUrl = "http://couchbot.io/img/smashcast.png";
            }
            
            embed.Description = server.LiveMessage
                .Replace("%CHANNEL%", channel)
                .Replace("%GAME%", gameName)
                .Replace("%TITLE%", title)
                .Replace("%URL%", url);
            embed.Title = channel + (string.IsNullOrEmpty(teamName) ? "" : " from the team '" + teamName + "'") + " has gone live!";
            embed.ImageUrl = server.AllowThumbnails ? thumbnailUrl + "?_=" + Guid.NewGuid().ToString().Replace("-", "") : "";
            embed.Footer = footer;

            var role = await DiscordHelper.GetRoleByGuildAndId(server.Id, server.MentionRole);
            var roleName = "";

            if (role == null)
            {
                server.MentionRole = 0;
            }

            if (server.MentionRole == 0)
            {
                roleName = "@everyone";
            }
            else if (server.MentionRole == 1)
            {
                roleName = "@here";
            }
            else
            {
                roleName = role.Mention;
            }

            var message = (server.AllowEveryone ? roleName + " " : "");

            if (server.UseTextAnnouncements)
            {
                if (!server.AllowThumbnails)
                {
                    url = "<" + url + ">";
                }

                message += "**[" + platform + "]** " + server.LiveMessage.Replace("%CHANNEL%", channel).Replace("%GAME%", gameName).Replace("%TITLE%", title).Replace("%URL%", url);
            }

            var broadcastMessage = new BroadcastMessage()
            {
                GuildId = server.Id,
                ChannelId = discordChannelId,
                UserId = channelId,
                Message = message,
                Platform = platform,
                Embed = (!server.UseTextAnnouncements ? embed.Build() : null),
                DeleteOffline = server.DeleteWhenOffline
            };

            return broadcastMessage;
        }


        public static async Task<List<ChannelMessage>> SendMessages(string platform, List<BroadcastMessage> messages)
        {
            IStatisticsManager statisticsManager = new StatisticsManager();
            var channelMessages = new List<ChannelMessage>();

            foreach (var message in messages)
            {
                var chat = await DiscordHelper.GetMessageChannel(message.GuildId, message.ChannelId);

                if (chat != null)
                {
                    try
                    {
                        ChannelMessage channelMessage = new ChannelMessage();
                        channelMessage.ChannelId = message.ChannelId;
                        channelMessage.GuildId = message.GuildId;
                        channelMessage.DeleteOffline = message.DeleteOffline;

                        if (message.Embed != null)
                        {
                            RequestOptions options = new RequestOptions();
                            options.RetryMode = RetryMode.AlwaysRetry;
                            var msg = await chat.SendMessageAsync(message.Message, false, message.Embed, options);

                            if (msg != null || msg.Id != 0)
                            {
                                channelMessage.MessageId = msg.Id;
                            }
                        }
                        else
                        {
                            var msg = await chat.SendMessageAsync(message.Message);

                            if (msg != null || msg.Id != 0)
                            {
                                channelMessage.MessageId = msg.Id;
                            }
                        }

                        channelMessages.Add(channelMessage);

                        if (platform.Equals(Constants.Mixer))
                        {
                            statisticsManager.AddToBeamAlertCount();
                        }
                        else if (platform.Equals(Constants.Smashcast))
                        {
                            statisticsManager.AddToHitboxAlertCount();
                        }
                        else if (platform.Equals(Constants.Twitch))
                        {
                            statisticsManager.AddToTwitchAlertCount();
                        }
                        else if (platform.Equals(Constants.YouTubeGaming))
                        {
                            statisticsManager.AddToYouTubeAlertCount();
                        }
                        else if(platform.Equals(Constants.Picarto))
                        {
                            statisticsManager.AddToPicartoAlertCount();
                        }
                        else if(platform.Equals(Constants.VidMe))
                        {
                            statisticsManager.AddToVidMeAlertCount();
                        }

                    }
                    catch (Exception ex)
                    {
                        Logging.LogError("Send Message Error: " + ex.Message + " in server " + message.GuildId);
                    }
                }
            }

            return channelMessages;
        }
    }
}
