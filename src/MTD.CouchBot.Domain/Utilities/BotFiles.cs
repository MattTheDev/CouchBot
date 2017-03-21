using MTD.CouchBot.Domain.Models;
using MTD.CouchBot.Json;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace MTD.CouchBot.Domain.Utilities
{
    public static class BotFiles
    {
        public static void CheckFolderStructure()
        {
            if (!Directory.Exists(Constants.ConfigRootDirectory))
                Directory.CreateDirectory(Constants.ConfigRootDirectory);

            if (!Directory.Exists(Constants.ConfigRootDirectory + Constants.GuildDirectory))
                Directory.CreateDirectory(Constants.ConfigRootDirectory + Constants.GuildDirectory);

            if (!Directory.Exists(Constants.ConfigRootDirectory + Constants.UserDirectory))
                Directory.CreateDirectory(Constants.ConfigRootDirectory + Constants.UserDirectory);

            if (!Directory.Exists(Constants.ConfigRootDirectory + Constants.LiveDirectory))
                Directory.CreateDirectory(Constants.ConfigRootDirectory + Constants.LiveDirectory);

            if (!Directory.Exists(Constants.ConfigRootDirectory + Constants.LiveDirectory + Constants.YouTubeDirectory))
                Directory.CreateDirectory(Constants.ConfigRootDirectory + Constants.LiveDirectory + Constants.YouTubeDirectory);

            if (!Directory.Exists(Constants.ConfigRootDirectory + Constants.LiveDirectory + Constants.TwitchDirectory))
                Directory.CreateDirectory(Constants.ConfigRootDirectory + Constants.LiveDirectory + Constants.TwitchDirectory);

            if (!Directory.Exists(Constants.ConfigRootDirectory + Constants.LiveDirectory + Constants.BeamDirectory))
                Directory.CreateDirectory(Constants.ConfigRootDirectory + Constants.LiveDirectory + Constants.BeamDirectory);

            if (!Directory.Exists(Constants.ConfigRootDirectory + Constants.LiveDirectory + Constants.HitboxDirectory))
                Directory.CreateDirectory(Constants.ConfigRootDirectory + Constants.LiveDirectory + Constants.HitboxDirectory);
    }

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

        public static List<DiscordServer> GetConfiguredServers()
        {
            var servers = new List<DiscordServer>();

            // Get Servers
            foreach (var server in Directory.GetFiles(Constants.ConfigRootDirectory + Constants.GuildDirectory))
            {
                servers.Add(JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(server)));
            }

            return servers;
        }

        public static List<User> GetConfiguredUsers()
        {
            var users = new List<User>();

            // Get Users
            foreach (var user in Directory.GetFiles(Constants.ConfigRootDirectory + Constants.UserDirectory))
            {
                users.Add(JsonConvert.DeserializeObject<User>(File.ReadAllText(user)));
            }

            return users;
        }

        public static List<LiveChannel> GetCurrentlyLiveBeamChannels()
        {
            var liveChannels = new List<LiveChannel>();

            // Get Live Channels
            foreach (var live in Directory.GetFiles(Constants.ConfigRootDirectory + Constants.LiveDirectory + Constants.BeamDirectory))
            {
                liveChannels.Add(JsonConvert.DeserializeObject<LiveChannel>(File.ReadAllText(live)));
            }

            return liveChannels;
        }

        public static List<LiveChannel> GetCurrentlyLiveYouTubeChannels()
        {
            var liveChannels = new List<LiveChannel>();

            // Get Live Channels
            foreach (var live in Directory.GetFiles(Constants.ConfigRootDirectory + Constants.LiveDirectory + Constants.YouTubeDirectory))
            {
                liveChannels.Add(JsonConvert.DeserializeObject<LiveChannel>(File.ReadAllText(live)));
            }

            return liveChannels;
        }

        public static List<LiveChannel> GetCurrentlyLiveTwitchChannels()
        {
            var liveChannels = new List<LiveChannel>();

            // Get Live Channels
            foreach (var live in Directory.GetFiles(Constants.ConfigRootDirectory + Constants.LiveDirectory + Constants.TwitchDirectory))
            {
                liveChannels.Add(JsonConvert.DeserializeObject<LiveChannel>(File.ReadAllText(live)));
            }

            return liveChannels;
        }

        public static List<LiveChannel> GetCurrentlyLiveHitboxChannels()
        {
            var liveChannels = new List<LiveChannel>();

            // Get Live Channels
            foreach (var live in Directory.GetFiles(Constants.ConfigRootDirectory + Constants.LiveDirectory + Constants.HitboxDirectory))
            {
                liveChannels.Add(JsonConvert.DeserializeObject<LiveChannel>(File.ReadAllText(live)));
            }

            return liveChannels;
        }
    }
}
