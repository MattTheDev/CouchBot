using Discord;
using MTD.CouchBot.Domain.Models.Bot;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MTD.CouchBot.Domain.Utilities
{
    public static class BotFiles
    {
        public static void CheckFolderStructure()
        {
            if (!Directory.Exists(Constants.ConfigRootDirectory))
            {
                Directory.CreateDirectory(Constants.ConfigRootDirectory);
            }

            if (!Directory.Exists(Constants.ConfigRootDirectory + Constants.GuildDirectory))
            { 
                Directory.CreateDirectory(Constants.ConfigRootDirectory + Constants.GuildDirectory);
            }

            if (!Directory.Exists(Constants.ConfigRootDirectory + Constants.UserDirectory))
            {
                Directory.CreateDirectory(Constants.ConfigRootDirectory + Constants.UserDirectory);
            }

            if (!Directory.Exists(Constants.ConfigRootDirectory + Constants.LiveDirectory))
            {
                Directory.CreateDirectory(Constants.ConfigRootDirectory + Constants.LiveDirectory);
            }

            if (!Directory.Exists(Constants.ConfigRootDirectory + Constants.LiveDirectory + Constants.YouTubeDirectory))
            {
                Directory.CreateDirectory(Constants.ConfigRootDirectory + Constants.LiveDirectory + Constants.YouTubeDirectory);
            }

            if (!Directory.Exists(Constants.ConfigRootDirectory + Constants.LiveDirectory + Constants.TwitchDirectory))
            {
                Directory.CreateDirectory(Constants.ConfigRootDirectory + Constants.LiveDirectory + Constants.TwitchDirectory);
            }

            if (!Directory.Exists(Constants.ConfigRootDirectory + Constants.LiveDirectory + Constants.SmashcastDirectory))
            {
                Directory.CreateDirectory(Constants.ConfigRootDirectory + Constants.LiveDirectory + Constants.SmashcastDirectory);
            }

            if (!Directory.Exists(Constants.ConfigRootDirectory + Constants.LiveDirectory + Constants.MixerDirectory))
            {
                Directory.CreateDirectory(Constants.ConfigRootDirectory + Constants.LiveDirectory + Constants.MixerDirectory);
            }

            if (!Directory.Exists(Constants.ConfigRootDirectory + Constants.LiveDirectory + Constants.PicartoDirectory))
            {
                Directory.CreateDirectory(Constants.ConfigRootDirectory + Constants.LiveDirectory + Constants.PicartoDirectory);
            }
        }

        public static void SaveDiscordServer(DiscordServer server)
        {
            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + server.Id + ".json";
            File.WriteAllText(file, JsonConvert.SerializeObject(server));
        }

        public async static Task SaveDiscordServer(DiscordServer server, IGuild guild)
        {
            var guildOwner = await guild.GetOwnerAsync();
            server.Id = guild.Id;
            server.Name = guild.Name;
            server.OwnerId = guild.OwnerId;
            server.OwnerName = guildOwner == null ? "" : guildOwner.Username;

            var file = Constants.ConfigRootDirectory + Constants.GuildDirectory + server.Id + ".json";
            File.WriteAllText(file, JsonConvert.SerializeObject(server));
        }

        public static List<string> GetConfiguredServerFileNames()
        {
            var servers = new List<string>();

            // Get Servers
            foreach (var server in Directory.GetFiles(Constants.ConfigRootDirectory + Constants.GuildDirectory))
            {
                servers.Add(Path.GetFileName(server.Replace(".json", "")));
            }

            return servers;
        }

        public static List<DiscordServer> GetConfiguredServers()
        {
            var servers = new List<DiscordServer>();

            // Get Servers
            foreach (var server in Directory.GetFiles(Constants.ConfigRootDirectory + Constants.GuildDirectory))
            {
                try
                {
                    servers.Add(JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(server)));
                }
                catch(Exception ex)
                {
                    continue;
                }
            }

            return servers;
        }

        public static List<string> GetConfiguredServerPaths()
        {
            var servers = new List<string>();

            // Get Servers
            foreach (var server in Directory.GetFiles(Constants.ConfigRootDirectory + Constants.GuildDirectory))
            {
                servers.Add(server);
            }

            return servers;
        }

        public static List<DiscordServer> GetConfiguredServersWithLiveChannel()
        {
            var servers = new List<DiscordServer>();

            // Get Servers
            foreach (var s in Directory.GetFiles(Constants.ConfigRootDirectory + Constants.GuildDirectory))
            {
                try
                {
                    var server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(s));

                    if (!server.AllowLive)
                    {
                        continue;
                    }

                    if (server.Id == 0 || server.GoLiveChannel == 0)
                    {
                        continue;
                    }

                    servers.Add(server);
                }
                catch(Exception ex)
                {
                    continue;
                }
            }

            return servers;
        }

        public static List<DiscordServer> GetConfiguredServersWithOwnerLiveChannel()
        {
            var servers = new List<DiscordServer>();

            // Get Servers
            foreach (var s in Directory.GetFiles(Constants.ConfigRootDirectory + Constants.GuildDirectory))
            {
                try
                {
                    var server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(s));

                    if (!server.AllowLive)
                    {
                        continue;
                    }

                    if (server.Id == 0 || server.OwnerLiveChannel == 0)
                    {
                        continue;
                    }

                    servers.Add(server);
                }
                catch(Exception ex)
                {
                    continue;
                }
            }

            return servers;
        }

        public static DiscordServer GetConfiguredServerById(ulong id)
        {
            try
            {
                return JsonConvert.DeserializeObject<DiscordServer>(
                    File.ReadAllText(Constants.ConfigRootDirectory + Constants.GuildDirectory + id + ".json"));
            }
            catch(Exception ex)
            {
                // Error on finding file.
                return null;
            }
        }

        public static List<User> GetConfiguredUsers()
        {
            var users = new List<User>();

            // Get Users
            foreach (var user in Directory.GetFiles(Constants.ConfigRootDirectory + Constants.UserDirectory))
            {
                try
                {
                    users.Add(JsonConvert.DeserializeObject<User>(File.ReadAllText(user)));
                }
                catch(Exception ex)
                {
                    continue;
                }
            }

            return users;
        }

        public static List<LiveChannel> GetCurrentlyLiveBeamChannels()
        {
            var liveChannels = new List<LiveChannel>();

            // Get Live Channels
            foreach (var live in Directory.GetFiles(Constants.ConfigRootDirectory + Constants.LiveDirectory + Constants.MixerDirectory))
            {
                try
                {
                    liveChannels.Add(JsonConvert.DeserializeObject<LiveChannel>(File.ReadAllText(live)));
                }
                catch(Exception ex)
                {
                    continue;
                }
            }

            return liveChannels;
        }

        public static List<LiveChannel> GetCurrentlyLiveYouTubeChannels()
        {
            var liveChannels = new List<LiveChannel>();

            // Get Live Channels
            foreach (var live in Directory.GetFiles(Constants.ConfigRootDirectory + Constants.LiveDirectory + Constants.YouTubeDirectory))
            {
                try
                {
                    liveChannels.Add(JsonConvert.DeserializeObject<LiveChannel>(File.ReadAllText(live)));
                }
                catch (Exception ex)
                {
                    continue;
                }
            }

            return liveChannels;
        }

        public static List<LiveChannel> GetCurrentlyLiveTwitchChannels()
        {
            var liveChannels = new List<LiveChannel>();

            // Get Live Channels
            foreach (var live in Directory.GetFiles(Constants.ConfigRootDirectory + Constants.LiveDirectory + Constants.TwitchDirectory))
            {
                try
                {
                    liveChannels.Add(JsonConvert.DeserializeObject<LiveChannel>(File.ReadAllText(live)));
                }
                catch (Exception ex)
                {
                    continue;
                }
            }

            return liveChannels;
        }

        public static List<LiveChannel> GetCurrentlyLiveHitboxChannels()
        {
            var liveChannels = new List<LiveChannel>();

            // Get Live Channels
            foreach (var live in Directory.GetFiles(Constants.ConfigRootDirectory + Constants.LiveDirectory + Constants.SmashcastDirectory))
            {
                try
                {
                    liveChannels.Add(JsonConvert.DeserializeObject<LiveChannel>(File.ReadAllText(live)));
                }
                catch (Exception ex)
                {
                    continue;
                }
            }

            return liveChannels;
        }

        public static List<LiveChannel> GetCurrentlyLivePicartoChannels()
        {
            var liveChannels = new List<LiveChannel>();

            // Get Live Channels
            foreach (var live in Directory.GetFiles(Constants.ConfigRootDirectory + Constants.LiveDirectory + Constants.PicartoDirectory))
            {
                try
                {
                    liveChannels.Add(JsonConvert.DeserializeObject<LiveChannel>(File.ReadAllText(live)));
                }
                catch (Exception ex)
                {
                    continue;
                }
            }

            return liveChannels;
        }

        public static void DeleteLiveBeamChannel(string beamId)
        {
            File.Delete(Constants.ConfigRootDirectory + Constants.LiveDirectory + Constants.MixerDirectory + beamId + ".json");
        }
    }
}
