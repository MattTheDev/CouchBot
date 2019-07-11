using Microsoft.Extensions.Options;
using MTD.CouchBot.Domain;
using MTD.CouchBot.Domain.Models.Bot;
using MTD.CouchBot.Domain.Models.Mixer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MTD.CouchBot.Managers;

namespace MTD.CouchBot.Services
{
    public class MixerConstellationService
    {
        public ClientWebSocket client = new ClientWebSocket();
        private readonly MixerManager _mixerManager;
        private readonly MessagingService _messagingService;
        private readonly DiscordService _discordService;
        private readonly BotSettings _botSettings;
        private readonly FileService _fileService;
        private readonly LoggingService _loggingService;

        private bool _reconnecting = false;

        public MixerConstellationService(MixerManager mixerManager, MessagingService messagingService,
            DiscordService discordService, IOptions<BotSettings> botSettings, FileService fileService,
            LoggingService loggingService)
        {
            _mixerManager = mixerManager;
            _messagingService = messagingService;
            _discordService = discordService;
            _botSettings = botSettings.Value;
            _fileService = fileService;
            _loggingService = loggingService;
        }

        public WebSocketState Status()
        {
            return client.State;
        }

        public async Task RunWebSockets()
        {
            _reconnecting = true;

            if (client.State == WebSocketState.Aborted)
            {
                client.Dispose();
                client = new ClientWebSocket();
            }

            if (client.State == WebSocketState.None ||
                client.State == WebSocketState.Closed ||
                client.State == WebSocketState.Aborted)
            {
                try
                {
                    client.Options.SetRequestHeader("x-is-bot", "true");
                    await client.ConnectAsync(new Uri("wss://constellation.mixer.com"), CancellationToken.None);

                    var receiving = Receiving(client);
                }
                catch (Exception ex)
                {
                    await _loggingService.LogError($"Error in RunWebSockets: {ex.Message}");
                    await _loggingService.LogError($"Error Details: Client State - {client.State}");
                }
            }

            _reconnecting = false;
        }

        private async Task Receiving(ClientWebSocket client)
        {
            var buffer = new byte[1024 * 4];

            while (true)
            {
                var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var data = Encoding.UTF8.GetString(buffer, 0, result.Count);

                    if (data.Replace(" ", "").ToLower().Contains("{\"online\":true}"))
                    {
                        var payload = JsonConvert.DeserializeObject<MixerPayload>(data);
                        var channelData = payload.data.channel.Split(':');
                        var channelId = channelData[1];
                        var channel = await _mixerManager.GetChannelById(channelId);

                        _loggingService.LogMixer(channel.token + " has gone online.");
                        await AnnounceLiveChannel(channelId, channel);
                    }
                    else if (data.Replace(" ", "").ToLower().Contains("{\"online\":false}"))
                    {
                        var payload = JsonConvert.DeserializeObject<MixerPayload>(data);
                        var channelData = payload.data.channel.Split(':');
                        var channelId = channelData[1];
                        var channel = await _mixerManager.GetChannelById(channelId);

                        _loggingService.LogMixer(channel.token + " has gone offline.");
                        await StreamOffline(channelId);
                    }

                    //if (!data.Contains("\"type\":\"reply\",\"result\":null,\"error\":null"))
                    //{
                    //    await _loggingService.LogMixerFile("Mixer Payload: " + data);
                    //}
                }

                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await client.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);

                    break;
                }
            }
        }

        public async Task SubscribeToLiveAnnouncements(string beamId)
        {
            //var channel = await beamManager.GetBeamChannelByName(channelName);           
            var subscribe = "{\"type\": \"method\", \"method\": \"livesubscribe\", \"params\": {\"events\": [\"channel:" + beamId + ":update\"]}, \"id\": " + beamId + "}";

            var bytes = Encoding.UTF8.GetBytes(subscribe);

            try
            {
                if (client.State == WebSocketState.Open)
                {
                    await client.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
            catch (Exception ex)
            {
                _loggingService.LogMixer("Exception in SubscribeToLiveAnnouncements: " + ex.Message);
                //await _loggingService.LogMixerFile("Exception in SubscribeToLiveAnnouncements: " + ex.Message);
            }
        }

        public async Task UnsubscribeFromLiveAnnouncements(string beamId)
        {
            var unsubscribe = "{\"type\": \"method\", \"method\": \"liveunsubscribe\", \"params\": {\"events\": [\"channel:" + beamId + ":update\"]}, \"id\": " + GetRandomInt() + "}";

            var bytes = Encoding.UTF8.GetBytes(unsubscribe);

            if (client.State == WebSocketState.Open)
            {
                await client.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        private int GetRandomInt()
        {
            var r = new Random();
            return r.Next(1, 100000);
        }

        public async Task Ping()
        {
            var ping = "{\"id\":1,\"type\":\"method\",\"method\":\"ping\",\"params\":null}";

            var bytes = Encoding.UTF8.GetBytes(ping);

            try
            {
                if (client.State == WebSocketState.Open)
                {
                    await client.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
                }
                else
                {
                    if (!_reconnecting)
                    {
                        await ResubscribeToBeamEvents();
                    }
                }
            }
            catch (Exception ex)
            {
                _loggingService.LogMixer("Exception in Ping: " + ex.Message);
                //await _loggingService.LogMixerFile("Exception in Ping: " + ex.Message);
            }
        }

        public async Task AnnounceLiveChannel(string beamId, MixerChannel channel)
        {
            var servers = _fileService.GetConfiguredServers();

            var beamServers = new List<DiscordServer>();
            var ownerBeamServers = new List<DiscordServer>();
            var teamServers = new List<MixerServerTeam>();

            var teams = await _mixerManager.GetTeamsByUserId(channel.userId.ToString());

            foreach (var server in servers)
            {
                if (server.ServerBeamChannelIds != null)
                {
                    if (server.ServerBeamChannelIds.Count > 0)
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

                if (!string.IsNullOrEmpty(server.OwnerBeamChannelId) && server.OwnerBeamChannelId.Equals(beamId))
                {
                    if (server.OwnerLiveChannel != 0)
                    {
                        ownerBeamServers.Add(server);
                    }
                }

                //if (server.MixerTeams != null && server.MixerTeams.Count > 0)
                //{
                //    foreach (var team in teams)
                //    {
                //        if (server.MixerTeams.Contains(team.id))
                //        {
                //            teamServers.Add(new MixerServerTeam
                //            {
                //                DiscordGuildId = server.Id,
                //                Team = team
                //            });
                //        }
                //    }
                //}
            }
            

            var messages = new List<BroadcastMessage>();

            foreach (var server in beamServers)
            {
                // Check to see if we have a message already queued up. If so, jump to the next server.

                if (server.AllowLive)
                {
                    if (server.GoLiveChannel != 0 && server.Id != 0)
                    {
                        if (messages.FirstOrDefault(x => x.GuildId == server.Id && x.UserId == beamId) == null)
                        {
                            var stream = channel;
                            var gameName = stream.type == null ? "A game" : stream.type.name;
                            var url = "http://mixer.com/" + stream.token;
                            var avatarUrl = stream.user.avatarUrl ??
                                            "https://mixer.com/_latest/assets/images/main/avatars/default.jpg";
                            var thumbnailUrl = "https://thumbs.mixer.com/channel/" + stream.id + ".small.jpg";
                            var channelId = stream.id.Value.ToString();
                            var test = stream.audience;
                            messages.Add(_messagingService.BuildMessage(stream.token, gameName, stream.name, url,
                                avatarUrl, thumbnailUrl,
                                Constants.Mixer, channelId, server, server.GoLiveChannel, null, false,
                                stream.viewersCurrent, stream.viewersTotal, stream.numFollowers));
                        }
                    }
                }
            }

            foreach (var server in ownerBeamServers)
            {
                if (server.AllowLive)
                {
                    if (server.OwnerLiveChannel != 0 && server.Id != 0)
                    {
                        if (messages.FirstOrDefault(x => x.GuildId == server.Id && x.UserId == beamId) == null)
                        {
                            var stream = channel;
                            var gameName = stream.type == null ? "A game" : stream.type.name;
                            var url = "http://mixer.com/" + stream.token;
                            var avatarUrl = stream.user.avatarUrl ??
                                            "https://mixer.com/_latest/assets/images/main/avatars/default.jpg";
                            var thumbnailUrl = "https://thumbs.mixer.com/channel/" + stream.id + ".small.jpg";
                            var channelId = stream.id.Value.ToString();

                            messages.Add(_messagingService.BuildMessage(stream.token, gameName, stream.name, url,
                                avatarUrl, thumbnailUrl,
                                Constants.Mixer, channelId, server, server.OwnerLiveChannel, null, true,
                                stream.viewersCurrent, stream.viewersTotal, stream.numFollowers));
                        }
                    }
                }
            }

            foreach (var teamServer in teamServers)
            {
                var server = servers.FirstOrDefault(x => x.Id == teamServer.DiscordGuildId);

                if (server != null && server.AllowLive)
                {
                    if (server?.GoLiveChannel != 0 && server?.Id != 0)
                    {
                        if (messages.FirstOrDefault(x => x.GuildId == server?.Id && x.UserId == beamId) == null)
                        {
                            var stream = channel;
                            var gameName = stream.type == null ? "A game" : stream.type.name;
                            var url = "http://mixer.com/" + stream.token;
                            var avatarUrl = stream.user.avatarUrl ??
                                            "https://mixer.com/_latest/assets/images/main/avatars/default.jpg";
                            var thumbnailUrl = "https://thumbs.mixer.com/channel/" + stream.id + ".small.jpg";
                            var channelId = stream.id.Value.ToString();

                            messages.Add(_messagingService.BuildMessage(stream.token, gameName, stream.name, url,
                                avatarUrl, thumbnailUrl,
                                Constants.Mixer, channelId, server, server.GoLiveChannel, teamServer.Team.name, false,
                                stream.viewersCurrent, stream.viewersTotal, stream.numFollowers));
                        }
                    }
                }
            }

            if (messages.Count > 0)
            {

                var liveChannel = new LiveChannel()
                {
                    Name = beamId,
                    Servers = new List<ulong>(),
                    ChannelMessages = await _messagingService.SendMessages(Constants.Mixer, messages)
                };

                File.WriteAllText(
                    _botSettings.DirectorySettings.ConfigRootDirectory +
                    _botSettings.DirectorySettings.LiveDirectory +
                    _botSettings.DirectorySettings.MixerDirectory +
                    beamId + ".json",
                    JsonConvert.SerializeObject(liveChannel));
            }
        }

        public async Task StreamOffline(string beamId)
        {
            var stream = await _mixerManager.GetChannelByName(beamId);
            var live = _fileService.GetCurrentlyLiveBeamChannels().FirstOrDefault(x => x.Name == beamId);

            if (live == null)
            {
                return;
            }

            foreach (var message in live.ChannelMessages)
            {
                var serverFile = _fileService.GetConfiguredServers().FirstOrDefault(x => x.Id == message.GuildId);

                if (serverFile == null)
                {
                    continue;
                }

                if (serverFile.DeleteWhenOffline)
                {
                    await _discordService.DeleteMessage(message.GuildId, message.ChannelId, message.MessageId);
                }
                else
                {
                    if (string.IsNullOrEmpty(serverFile.StreamOfflineMessage))
                    {
                        serverFile.StreamOfflineMessage = "This stream is now offline.";
                    }

                    await _discordService.SetOfflineStream(message.GuildId, serverFile.StreamOfflineMessage, message.ChannelId, message.MessageId);
                }

                _fileService.DeleteLiveBeamChannel(beamId);
            }
        }

        public async Task ResubscribeToBeamEvents()
        {
            var count = 0;
            var alreadyProcessed = new List<string>();
            var alreadyProcessedTeams = new List<int>();

            _loggingService.LogMixer("Getting Server Files.");

            var servers = _fileService.GetConfiguredServers().Where(x => x.ServerBeamChannelIds != null && 
                                                                         x.ServerBeamChannelIds.Count > 0).ToList();
            servers.AddRange(_fileService.GetConfiguredServers().Where(x => !string.IsNullOrEmpty(x.OwnerBeamChannelId)));
            //servers.AddRange(_fileService.GetConfiguredServers().Where(x => x.MixerTeams != null && x.MixerTeams.Count > 0));

            if (client.State != WebSocketState.Open)
            {
                _loggingService.LogMixer("Connecting to Mixer Constellation.");

                await RunWebSockets();

                _loggingService.LogMixer("Connected to Mixer Constellation.");
            }

            if (servers.Count > 0)
            {
                foreach (var s in servers)
                {
                    if (s.ServerBeamChannelIds != null && s.ServerBeamChannelIds.Count > 0)
                    {
                        foreach (var b in s.ServerBeamChannelIds)
                        {
                            if (!alreadyProcessed.Contains(b))
                            {
                                await SubscribeToLiveAnnouncements(b);
                                count++;
                                alreadyProcessed.Add(b);
                            }
                        }

                        if (!string.IsNullOrEmpty(s.OwnerBeamChannelId))
                        {
                            if (!alreadyProcessed.Contains(s.OwnerBeamChannelId))
                            {
                                await SubscribeToLiveAnnouncements(s.OwnerBeamChannelId);
                                count++;
                                alreadyProcessed.Add(s.OwnerBeamChannelId);
                            }
                        }
                    }

                    //if (s.MixerTeams != null && s.MixerTeams.Count > 0)
                    //{
                    //    foreach (var t in s.MixerTeams)
                    //    {
                    //        if (!alreadyProcessedTeams.Contains(t))
                    //        {
                    //            var teamUsers = await _mixerManager.GetTeamUsersByTeamId(t);

                    //            foreach (var user in teamUsers)
                    //            {
                    //                var mixerUser =
                    //                    await _mixerManager.GetUserById(user.teamMembership.userId.ToString());
                    //                if (!alreadyProcessed.Contains(mixerUser.channel.id.ToString()))
                    //                {
                    //                    await SubscribeToLiveAnnouncements(mixerUser.channel.id.ToString());
                    //                    count++;
                    //                    alreadyProcessed.Add(user.id.ToString());
                    //                }
                    //            }

                    //            alreadyProcessedTeams.Add(t);
                    //        }
                    //    }
                    //}
                }
            }
        }
    }
}