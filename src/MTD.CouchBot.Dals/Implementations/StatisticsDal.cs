using MTD.CouchBot.Domain;
using MTD.CouchBot.Domain.Models;
using Newtonsoft.Json;
using System;
using System.IO;

namespace MTD.CouchBot.Dals.Implementations
{
    public class StatisticsDal : IStatisticsDal
    {
        public BotStats GetBotStats()
        {
            var path = Constants.ConfigRootDirectory + Constants.BotStatistics;

            if(!File.Exists(path))
            {
                var stats = new BotStats();
                stats.BeamAlertCount = 0;
                stats.TwitchAlertCount = 0;
                stats.UptimeMinutes = 0;
                stats.YouTubeAlertCount = 0;
                stats.LoggingStartDate = DateTime.UtcNow;
                File.WriteAllText(path, JsonConvert.SerializeObject(stats));
            }
            return JsonConvert.DeserializeObject<BotStats>(File.ReadAllText(path));
        }

        public void SaveBotStats(BotStats stats)
        {
            var path = Constants.ConfigRootDirectory + Constants.BotStatistics;
            File.WriteAllText(path, JsonConvert.SerializeObject(stats));
        }

        public void AddToBeamAlertCount()
        {
            var stats = GetBotStats();
            stats.BeamAlertCount++;
            SaveBotStats(stats);
        }

        public void AddToHitboxAlertCount()
        {
            var stats = GetBotStats();
            stats.HitboxAlertCount++;
            SaveBotStats(stats);
        }

        public void AddToTwitchAlertCount()
        {
            var stats = GetBotStats();
            stats.TwitchAlertCount++;
            SaveBotStats(stats);
        }

        public void AddToYouTubeAlertCount()
        {
            var stats = GetBotStats();
            stats.YouTubeAlertCount++;
            SaveBotStats(stats);
        }

        public void AddUptimeMinutes()
        {
            var stats = GetBotStats();
            stats.UptimeMinutes+=5;
            SaveBotStats(stats);
        }

        public int GetBeamAlertCount()
        {
            return GetBotStats().BeamAlertCount;
        }

        public int GetHitboxAlertCount()
        {
            return GetBotStats().HitboxAlertCount;
        }

        public int GetTwitchAlertCount()
        {
            return GetBotStats().TwitchAlertCount;
        }

        public int GetUptimeMinutes()
        {
            return GetBotStats().UptimeMinutes;
        }

        public int GetYouTubeAlertCount()
        {
            return GetBotStats().YouTubeAlertCount;
        }

        public void LogRestartTime()
        {
            var stats = GetBotStats();
            stats.LastRestart = DateTime.UtcNow;
            SaveBotStats(stats);
        }

    }
}
