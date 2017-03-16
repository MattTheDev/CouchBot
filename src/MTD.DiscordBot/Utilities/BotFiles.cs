using MTD.DiscordBot.Domain;
using MTD.DiscordBot.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MTD.DiscordBot.Utilities
{
    public static class BotFiles
    {
        public static DiscordServer GetDiscordServer(string guildId)
        {
            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + guildId + ".json";
            var server = new DiscordServer();

            return server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));
        }

        public static void SaveDiscordServer(DiscordServer server)
        {
            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + server.Id + ".json";
            File.WriteAllText(file, JsonConvert.SerializeObject(server));
        }
    }
}
