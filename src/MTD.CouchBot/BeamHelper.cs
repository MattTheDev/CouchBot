using Discord;
using MTD.CouchBot.Bot.Utilities;
using MTD.CouchBot.Domain;
using MTD.CouchBot.Domain.Models;
using MTD.CouchBot.Domain.Utilities;
using MTD.CouchBot.Json;
using MTD.CouchBot.Managers;
using MTD.CouchBot.Managers.Implementations;
using MTD.CouchBot.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MTD.CouchBot.Bot
{
    public static class BeamHelper
    {
        public static async Task AnnounceLiveChannel(string beamId)
        {
            IBeamManager beamManager = new BeamManager();

            var beamUsers = BotFiles.GetConfiguredUsers()
                .Where(x => !string.IsNullOrEmpty(x.BeamId) && x.BeamId == beamId);
            var servers = BotFiles.GetConfiguredServers();

            var beamServers = new List<DiscordServer>();
            var userSharedServers = new List<DiscordServer>();

            foreach(var server in servers)
            {
                if(server.ServerBeamChannels != null && server.ServerBeamChannelIds != null)
                {
                    if(server.ServerBeamChannels.Count > 0 && server.ServerBeamChannelIds.Count > 0)
                    {
                        if (server.ServerBeamChannelIds.Contains(beamId))
                        {
                            if (server.GoLiveChannel != 0)
                            {
                                beamServers.Add(server);
                            }
                        }
                    }
                }
            }

            List<BroadcastMessage> messages = new List<BroadcastMessage>();

            foreach (var user in beamUsers)
            {
                var userServers = new List<DiscordServer>();

                foreach(var server in BotFiles.GetConfiguredServers())
                {
                    if(server.Users != null && server.Users.Contains(user.Id.ToString()))
                    {
                        if (server.GoLiveChannel != 0 && server.Id != 0)
                        {
                            if ((!server.BroadcastOthers && server.OwnerId == user.Id) || server.BroadcastOthers)
                            {
                                if (server.BroadcasterWhitelist == null)
                                    server.BroadcasterWhitelist = new List<string>();

                                if (!server.UseWhitelist || (server.UseWhitelist && server.BroadcasterWhitelist.Contains(user.Id.ToString())))
                                {
                                    userServers.Add(server);
                                }
                            }
                        }
                    }
                }

                var stream = await beamManager.GetBeamChannelByName(beamId);

                if (userServers != null && userServers.Count > 0)
                {
                    foreach (var server in userServers)
                    {
                        messages.Add(await BuildMessage(stream, server));
                    }
                }
            }

            foreach (var server in beamServers)
            {
                // Check to see if we have a message already queued up. If so, jump to the next server.

                if (server.GoLiveChannel != 0 && server.Id != 0)
                {
                    if (messages.FirstOrDefault(x => x.GuildId == server.Id && x.UserId == beamId) == null)
                    {
                        var stream = await beamManager.GetBeamChannelByName(beamId);

                        messages.Add(await BuildMessage(stream, server));
                    }
                }

            }

            if (messages.Count > 0)
            {

                var channel = new LiveChannel()
                {
                    Name = beamId,
                    Servers = new List<ulong>(),
                    ChannelMessages = await SendMessages(messages)
                };

                File.WriteAllText(
                    Constants.ConfigRootDirectory +
                    Constants.LiveDirectory +
                    Constants.BeamDirectory +
                    beamId + ".json",
                    JsonConvert.SerializeObject(channel));
            }
        }

        public static async Task<BroadcastMessage> BuildMessage(BeamChannel stream, DiscordServer server)
        {
            string gameName = stream.type == null ? "a game" : stream.type.name;
            string url = "http://beam.pro/" + stream.token;

            EmbedBuilder embed = new EmbedBuilder();
            EmbedAuthorBuilder author = new EmbedAuthorBuilder();
            EmbedFooterBuilder footer = new EmbedFooterBuilder();

            if (server.LiveMessage == null)
            {
                server.LiveMessage = "%CHANNEL% just went live with %GAME% - %TITLE% - %URL%";
            }

            Color blue = new Color(76, 144, 243);
            author.IconUrl = Program.client.CurrentUser.GetAvatarUrl() + "?_=" + Guid.NewGuid().ToString().Replace("-", "");
            author.Name = "CouchBot";
            author.Url = url;
            footer.Text = "[Beam] - " + DateTime.UtcNow.AddHours(server.TimeZoneOffset);
            footer.IconUrl = "http://couchbot.io/img/beam.jpg";
            embed.Author = author;
            embed.Color = blue;
            embed.Description = server.LiveMessage.Replace("%CHANNEL%", stream.token).Replace("%GAME%", gameName).Replace("%TITLE%", stream.name).Replace("%URL%", url);
            embed.Title = stream.token + " has gone live!";
            embed.ThumbnailUrl = stream.user.avatarUrl != null ? stream.user.avatarUrl + "?_=" + Guid.NewGuid().ToString().Replace("-", "") : "https://beam.pro/_latest/assets/images/main/avatars/default.jpg";
            embed.ImageUrl = server.AllowThumbnails ? "https://thumbs.beam.pro/channel/" + stream.id + ".small.jpg" + "?_=" + Guid.NewGuid().ToString().Replace("-", "") : "";
            embed.Footer = footer;

            var message = (server.AllowEveryone ? server.MentionRole != 0 ? (await DiscordHelper.GetRoleByGuildAndId(server.Id, server.MentionRole)).Mention : "@everyone " : "");

            if (server.UseTextAnnouncements)
            {
                if (!server.AllowThumbnails)
                {
                    url = "<" + url + ">";
                }

                message += "**[Beam]** " + server.LiveMessage.Replace("%CHANNEL%", stream.token).Replace("%GAME%", gameName).Replace("%TITLE%", stream.name).Replace("%URL%", url);
            }

            var broadcastMessage = new BroadcastMessage()
            {
                GuildId = server.Id,
                ChannelId = server.GoLiveChannel,
                UserId = stream.id.Value.ToString(),
                Message = message,
                Platform = "Beam",
                Embed = (!server.UseTextAnnouncements ? embed.Build() : null)
            };

            return broadcastMessage;
        }

        public static async Task<List<ChannelMessage>> SendMessages(List<BroadcastMessage> messages)
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
                        statisticsManager.AddToBeamAlertCount();
                    }
                    catch (Exception ex)
                    {
                        Logging.LogError("Send Message Error: " + ex.Message + " in server " + message.GuildId);
                    }
                }
            }

            return channelMessages;
        }

        public static async Task StreamOffline(string beamId)
        {
            IBeamManager beamManager = new BeamManager();
            var stream = await beamManager.GetBeamChannelByName(beamId);
            var live = BotFiles.GetCurrentlyLiveBeamChannels().FirstOrDefault(x => x.Name == beamId);
            
            if (live == null)
                return;

            foreach(var message in live.ChannelMessages)
            {
                var serverFile = BotFiles.GetConfiguredServers().FirstOrDefault(x => x.Id == message.GuildId);

                if (serverFile == null)
                    continue;

                if(serverFile.DeleteWhenOffline)
                {
                    await DiscordHelper.DeleteMessage(message.GuildId, message.ChannelId, message.MessageId);
                }
                else
                {
                    await DiscordHelper.SetOfflineStream(message.GuildId, message.ChannelId, message.MessageId);
                }

                BotFiles.DeleteLiveBeamChannel(beamId);
            }
        }
    }
}
