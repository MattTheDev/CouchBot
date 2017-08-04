using MTD.CouchBot.Domain.Models.Mixer;
using MTD.CouchBot.Domain.Utilities;
using MTD.CouchBot.Managers;
using MTD.CouchBot.Managers.Implementations;
using Newtonsoft.Json;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTD.CouchBot.Bot
{
    // https://dev.beam.pro/reference/constellation/index.html
    public class BeamClient
    {
        public ClientWebSocket client = new ClientWebSocket();
        IMixerManager _mixerManager;
        IStatisticsManager _statisticsManager;
    
        public BeamClient()
        {
            _mixerManager = new MixerManager();
            _statisticsManager = new StatisticsManager();
        }

        public WebSocketState Status()
        {
            return client.State;
        }

        public async Task RunWebSockets()
        {
            client.Options.SetRequestHeader("x-is-bot", "true");
            await client.ConnectAsync(new Uri("wss://constellation.mixer.com"), CancellationToken.None);

            var receiving = Receiving(client);
        }

        private async Task Receiving(ClientWebSocket client)
        {
            var buffer = new byte[1024 * 4];

            while (true)
            {
                var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                var connectionStatus = client.State;
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var data = Encoding.UTF8.GetString(buffer, 0, result.Count);

                    if (data.Replace(" ", "").ToLower().Contains("{\"online\":true}"))
                    {
                        var payload = JsonConvert.DeserializeObject<MixerPayload>(data);
                        var channelData = payload.data.channel.Split(':');
                        var channelId = channelData[1];
                        var channel = await _mixerManager.GetChannelById(channelId);

                        Logging.LogMixer(channel.token + " has gone online.");
                        await BeamHelper.AnnounceLiveChannel(channelId);
                    }
                    else if(data.Replace(" ", "").ToLower().Contains("{\"online\":false}"))
                    {
                        var payload = JsonConvert.DeserializeObject<MixerPayload>(data);
                        var channelData = payload.data.channel.Split(':');
                        var channelId = channelData[1];
                        var channel = await _mixerManager.GetChannelById(channelId);

                        Logging.LogMixer(channel.token + " has gone offline.");
                        await BeamHelper.StreamOffline(channelId);
                    }
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
            int random = GetRandomInt();

            while (_statisticsManager.ContainsRandomInt(random))
            {
                random = GetRandomInt();
            }

            _statisticsManager.AddRandomInt(random);            
            
            var subscribe = "{\"type\": \"method\", \"method\": \"livesubscribe\", \"params\": {\"events\": [\"channel:" + beamId + ":update\"]}, \"id\": " + random + "}";

            var bytes = Encoding.UTF8.GetBytes(subscribe);

            try
            {
                await client.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch(Exception ex)
            {
                var test = ex;
            }
        }

        public async Task UnsubscribeFromLiveAnnouncements(string beamId)
        {
            var unsubscribe = "{\"type\": \"method\", \"method\": \"liveunsubscribe\", \"params\": {\"events\": [\"channel:" + beamId + ":update\"]}, \"id\": " + GetRandomInt() + "}";

            var bytes = Encoding.UTF8.GetBytes(unsubscribe);

            await client.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private int GetRandomInt()
        {
            Random r = new Random();
            return r.Next(1, 100000);
        }
    }
}
