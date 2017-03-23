using MTD.CouchBot.Domain.Models;
using MTD.CouchBot.Managers;
using MTD.CouchBot.Managers.Implementations;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
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
        IBeamManager _beamManager;
        IStatisticsManager _statisticsManager;
    
        public BeamClient()
        {
            _beamManager = new BeamManager();
            _statisticsManager = new StatisticsManager();
        }

        public async Task RunWebSockets()
        {
            client.Options.SetRequestHeader("x-is-bot", "true");
            await client.ConnectAsync(new Uri("wss://constellation.beam.pro"), CancellationToken.None);

            var receiving = Receiving(client);
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



                    if (data.ToLower().Contains("{\"online\": true}"))
                    {
                        Console.WriteLine("BEAM DATA: " + data);
                        var payload = JsonConvert.DeserializeObject<BeamPayload>(data);
                        var id = (payload.data.channel.Split(':'))[1];
                        await BeamHelper.AnnounceLiveChannel(id);
                    }
                    else if(data.ToLower().Contains("{\"online\": false}"))
                    {
                        Console.WriteLine("BEAM DATA: " + data);
                        var payload = JsonConvert.DeserializeObject<BeamPayload>(data);
                        var id = (payload.data.channel.Split(':'))[1];
                        await BeamHelper.StreamOffline(id);
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
