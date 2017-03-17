using MTD.DiscordBot.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTD.DiscordBot.Constellation
{
    // https://dev.beam.pro/reference/constellation/index.html
    public class BeamClient
    {
        ClientWebSocket client = new ClientWebSocket();
        IBeamManager beamManager;

        public BeamClient()
        {
            client.Options.SetRequestHeader("x-is-bot", "true");

            beamManager = new BeamManager();
        }

        private async Task RunWebSockets()
        {
            await client.ConnectAsync(new Uri("wss://constellation.beam.pro"), CancellationToken.None);

            var sending = Task.Run(async () => { });

            var receiving = Receiving(client);

            await Task.WhenAll(sending, receiving);
        }

        private static async Task Receiving(ClientWebSocket client)
        {
            var buffer = new byte[1024 * 4];

            while (true)
            {
                var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Text)
                    Console.WriteLine(Encoding.UTF8.GetString(buffer, 0, result.Count));

                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await client.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                    break;
                }
            }
        }

        public async Task SubscribeToLiveAnnouncements(string channelName)
        {
            var channel = await beamManager.GetBeamChannelByName(channelName);
            var subscribe = "{\"type\": \"method\", \"method\": \"livesubscribe\", \"params\": {\"events\": [\"channel:" + channel.id + ":status\"]}, \"id\": " + GetRandomInt() + "}";

            var bytes = Encoding.UTF8.GetBytes(subscribe);

            await client.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public async Task UnsubscribeFromLiveAnnouncements(string channelName)
        {
            var channel = await beamManager.GetBeamChannelByName(channelName);
            var unsubscribe = "{\"type\": \"method\", \"method\": \"liveunsubscribe\", \"params\": {\"events\": [\"channel:" + channel.id + ":status\"]}, \"id\": " + GetRandomInt() + "}";

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
