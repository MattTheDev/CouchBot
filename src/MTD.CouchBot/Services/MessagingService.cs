using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using MTD.CouchBot.Domain;
using MTD.CouchBot.Domain.Models.Bot;
using MTD.CouchBot.Domain.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTD.CouchBot.Services
{
    public class MessagingService
    {
        private readonly DiscordShardedClient _discord;
        private readonly DiscordService _discordService;
        private readonly BotSettings _botSettings;
        private readonly FileService _fileService;
        private readonly StringService _stringService;
        private readonly LoggingService _loggingService;

        public MessagingService(DiscordShardedClient discord, DiscordService discordService, IOptions<BotSettings> botSettings, FileService fileService,
            StringService stringService, LoggingService loggingService)
        {
            _discord = discord;
            _discordService = discordService;
            _botSettings = botSettings.Value;
            _fileService = fileService;
            _stringService = stringService;
            _loggingService = loggingService;
        }

        public BroadcastMessage BuildTestPublishedMessage(SocketUser user, ulong guildId, ulong channelId)
        {
            var servers = _fileService.GetConfiguredServers();
            var server = servers.FirstOrDefault(x => x.Id == guildId);

            if (server == null)
                return null;

            var url = "http://" + (server.UseYouTubeGamingPublished ? "gaming" : "www") + ".youtube.com/watch?v=B7wkzmZ4GBw";

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder();
            var footer = new EmbedFooterBuilder();


            if (server.PublishedMessage == null)
            {
                server.PublishedMessage = "%CHANNEL% just published a new video.";
            }

            var red = new Color(179, 18, 23);
            author.IconUrl = user.GetAvatarUrl() + "?_=" + Guid.NewGuid().ToString().Replace("-", "");

            author.Name = ((IGuildUser)user).Nickname ?? _discord.CurrentUser.Username;
            author.Url = url;
            footer.Text = "[" + Constants.YouTube + "] - " + DateTime.UtcNow.AddHours(server.TimeZoneOffset);
            footer.IconUrl = Constants.YouTubeLogoUrl;
            embed.Author = author;
            embed.Color = red;
            embed.Description = _stringService.AnnouncementText(server.PublishedMessage, "Test Channel", "Test Title", url, "");

            embed.Title = "Test Channel published a new video!";
            embed.ThumbnailUrl = "http://mattthedev.codes/img/bot/vader.jpg" + "?_=" + Guid.NewGuid().ToString().Replace("-", "");
            embed.ImageUrl = server.AllowThumbnails ? "http://mattthedev.codes/img/bot/test_thumbnail.jpg" + "?_=" + Guid.NewGuid().ToString().Replace("-", "") : "";
            embed.Footer = footer;
            
            var role = _discordService.GetRoleByGuildAndId(server.Id, server.MentionRole);
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

                message += $"**[Test]** {_stringService.AnnouncementText(server.LiveMessage, "Test Channel", "Test Title", url, "")}";
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

        public BroadcastMessage BuildTestMessage(SocketUser user, ulong guildId, ulong channelId, string platform)
        {
            var servers = _fileService.GetConfiguredServers();
            var server = servers.FirstOrDefault(x => x.Id == guildId);

            if (server == null)
                return null;

            var url = "http://mattthedev.codes";

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder();
            var footer = new EmbedFooterBuilder();

            if (server.LiveMessage == null)
            {
                server.LiveMessage = "%CHANNEL% just went live with %GAME% - %TITLE% - %URL%";
            }

            var color = new Color(76, 144, 243);
            if (platform.Equals(Constants.Twitch))
            {
                color = new Color(100, 65, 164);
                footer.Text = "[" + Constants.Twitch + "] - " + DateTime.UtcNow.AddHours(server.TimeZoneOffset);
                footer.IconUrl = Constants.TwitchLogoUrl;
            }
            else if (platform.Equals(Constants.YouTube) || platform.Equals(Constants.YouTubeGaming))
            {
                color = new Color(179, 18, 23);
                footer.Text = "[" + Constants.YouTubeGaming + "] - " + DateTime.UtcNow.AddHours(server.TimeZoneOffset);
                footer.IconUrl = Constants.YouTubeLogoUrl;
            }
            else if (platform.Equals(Constants.Smashcast))
            {
                color = new Color(153, 204, 0);
                footer.Text = "[" + Constants.Smashcast + "] - " + DateTime.UtcNow.AddHours(server.TimeZoneOffset);
                footer.IconUrl = Constants.SmashcastLogoUrl;
            }
            else if (platform.Equals(Constants.Mixer))
            {
                color = new Color(76, 144, 243);
                footer.Text = "[" + Constants.Mixer + "] - " + DateTime.UtcNow.AddHours(server.TimeZoneOffset);
                footer.IconUrl = Constants.MixerLogoUrl;
            }

            author.IconUrl = (!string.IsNullOrEmpty(user.GetAvatarUrl()) ? user.GetAvatarUrl() : "http://mattthedev.codes/img/bot/discord.png") + "?_=" + Guid.NewGuid().ToString().Replace("-", "");
            author.Name = ((IGuildUser)user).Nickname ?? _discord.CurrentUser.Username;
            author.Url = url;
            embed.Author = author;
            embed.Color = color;
            embed.Description = _stringService.AnnouncementText(server.LiveMessage, "Test Channel", "Test Title", url, "");
            embed.Title = "Test Channel has gone live!";
            embed.ThumbnailUrl = "http://mattthedev.codes/img/bot/vader.jpg" + "?_=" + Guid.NewGuid().ToString().Replace("-", "");
            embed.ImageUrl = server.AllowThumbnails ? "http://mattthedev.codes/img/bot/test_thumbnail.jpg" + "?_=" + Guid.NewGuid().ToString().Replace("-", "") : "";
            embed.Footer = footer;

            embed.Fields.Add(new EmbedFieldBuilder()
            {
                IsInline = true,
                Name = "Game",
                Value = "Test Game"
            });

            embed.Fields.Add(new EmbedFieldBuilder()
            {
                IsInline = true,
                Name = "Viewers",
                Value = "123"
            });

            embed.Fields.Add(new EmbedFieldBuilder()
            {
                IsInline = true,
                Name = "Followers",
                Value = "123"
            });

            embed.Fields.Add(new EmbedFieldBuilder()
            {
                IsInline = true,
                Name = "Total Views",
                Value = "1234"
            });

            var communities = new List<string>() { "Community 1", "Community 2", "Community 3" };
            embed.Fields.Add(new EmbedFieldBuilder()
            {
                IsInline = true,
                Name = "Communities",
                Value = communities.Count > 0 ? String.Join(", ", communities) : "None"
            });

            var role = _discordService.GetRoleByGuildAndId(server.Id, server.MentionRole);
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

                message += $"**[Test]** {_stringService.AnnouncementText(server.LiveMessage, "Test Channel", "Test Title", url, "")}";
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

        public BroadcastMessage BuildMessage(string channel,
            string gameName, string title, string url, string avatarUrl, string thumbnailUrl, string platform,
            string channelId, DiscordServer server, ulong discordChannelId, string teamName, bool owner,
            int? viewers = null, int? totalViews = null, int? followers = null)
        {
            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder();
            var footer = new EmbedFooterBuilder();

            if (server.LiveMessage == null)
            {
                server.LiveMessage = "%CHANNEL% just went live with %GAME% - %TITLE% - %URL%";
            }

            var botName = _discord.CurrentUser.Username;

            try
            {
                botName = ((IGuildUser) _discord.GetGuild(server.Id).GetUser(_botSettings.BotConfig.CouchBotId))
                    .Nickname;
            }
            catch (Exception)
            {
                // Nothing. It's fine. Everything is fine.
            }

            author.IconUrl = _discord.CurrentUser.GetAvatarUrl() + "?_=" + Guid.NewGuid().ToString().Replace("-", "");
            author.Name = botName;
            author.Url = url;
            footer.Text = "[" + platform + "] - " + DateTime.UtcNow.AddHours(server.TimeZoneOffset);
            embed.Author = author;

            var allowEveryone = false;

            if (platform.Equals(Constants.Mixer))
            {
                embed.Color = Constants.Blue;
                embed.ThumbnailUrl = avatarUrl != null ?
                        avatarUrl + "?_=" + Guid.NewGuid().ToString().Replace("-", "") :
                        "https://mixer.com/_latest/assets/images/main/avatars/default.jpg";
                footer.IconUrl = Constants.MixerLogoUrl;
                allowEveryone = owner ? server.AllowMentionOwnerMixerLive : server.AllowMentionMixerLive;
            }
            else if (platform.Equals(Constants.YouTubeGaming))
            {
                embed.Color = Constants.Red;
                embed.ThumbnailUrl = avatarUrl + "?_=" + Guid.NewGuid().ToString().Replace("-", "");
                footer.IconUrl = Constants.YouTubeLogoUrl;
                allowEveryone = owner ? server.AllowMentionOwnerYouTubeLive : server.AllowMentionYouTubeLive;
            }
            else if (platform.Equals(Constants.Twitch))
            {
                embed.Color = Constants.Purple;
                embed.ThumbnailUrl = avatarUrl != null ?
                        avatarUrl + "?_=" + Guid.NewGuid().ToString().Replace("-", "") :
                        "https://static-cdn.jtvnw.net/jtv_user_pictures/xarth/404_user_70x70.png";
                footer.IconUrl = Constants.TwitchLogoUrl;
                allowEveryone = owner ? server.AllowMentionOwnerTwitchLive : server.AllowMentionTwitchLive;
            }
            else if (platform.Equals(Constants.Smashcast))
            {
                embed.Color = Constants.Green;
                embed.ThumbnailUrl = avatarUrl + "?_=" + Guid.NewGuid().ToString().Replace("-", "");
                footer.IconUrl = Constants.SmashcastLogoUrl;
                allowEveryone = owner ? server.AllowMentionOwnerSmashcastLive : server.AllowMentionSmashcastLive;
            }
            else if (platform.Equals(Constants.Mobcrush))
            {
                embed.Color = Constants.Yellow;
                embed.ThumbnailUrl = avatarUrl + "?_=" + Guid.NewGuid().ToString().Replace("-", "");
                footer.IconUrl = Constants.MobcrushLogoUrl;
                allowEveryone = owner ? server.AllowMentionOwnerMobcrushLive : server.AllowMentionMobcrushLive;
            }

            embed.Description = server.LiveMessage
                .Replace("%CHANNEL%", Format.Sanitize(channel))
                .Replace("%GAME%", gameName)
                .Replace("%TITLE%", title)
                .Replace("%URL%", url);
            embed.Title = channel + (string.IsNullOrEmpty(teamName) ? "" : " from the team '" + teamName + "'") + " has gone live!";
            embed.ImageUrl = server.AllowThumbnails ? thumbnailUrl + "?_=" + Guid.NewGuid().ToString().Replace("-", "") : "";
            embed.Footer = footer;

            if (server.DisplayStreamStatistics)
            {
                if (!platform.Equals(Constants.YouTubeGaming) && !platform.Equals(Constants.YouTubeGaming))
                {
                    if (!string.IsNullOrEmpty(gameName))
                    {
                        embed.Fields.Add(new EmbedFieldBuilder()
                        {
                            IsInline = true,
                            Name = "Game",
                            Value = gameName
                        });
                    }
                }

                if (followers != null)
                {
                    embed.Fields.Add(new EmbedFieldBuilder()
                    {
                        IsInline = true,
                        Name = "Followers",
                        Value = followers
                    });
                }

                if (totalViews != null)
                {
                    embed.Fields.Add(new EmbedFieldBuilder()
                    {
                        IsInline = true,
                        Name = "Total Views",
                        Value = totalViews
                    });
                }
            }

            var role = _discordService.GetRoleByGuildAndId(server.Id, server.MentionRole);
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

            var message = (allowEveryone ? roleName + " " : "");

            if (server.UseTextAnnouncements)
            {
                if (!server.AllowThumbnails)
                {
                    url = "<" + url + ">";
                }

                message += "**[" + platform + "]** " + server.LiveMessage.Replace("%CHANNEL%", Format.Sanitize(channel)).Replace("%GAME%", gameName).Replace("%TITLE%", title).Replace("%URL%", url);
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

        public async Task<List<ChannelMessage>> SendMessages(string platform, List<BroadcastMessage> messages)
        {
            var channelMessages = new List<ChannelMessage>();

            foreach (var message in messages)
            {
                var chat = await _discordService.GetMessageChannel(message.GuildId, message.ChannelId);

                if (chat != null)
                {
                    try
                    {
                        var channelMessage = new ChannelMessage
                        {
                            ChannelId = message.ChannelId,
                            GuildId = message.GuildId,
                            DeleteOffline = message.DeleteOffline
                        };

                        if (message.Embed != null)
                        {
                            var options = new RequestOptions {RetryMode = RetryMode.AlwaysRetry};
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
                    }
                    catch (Exception ex)
                    {
                        await _loggingService.LogError("Send Message Error: " + ex.Message + " in server " + message.GuildId);
                    }
                }
            }

            return channelMessages;
        }
    }
}
