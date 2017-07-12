using MTD.CouchBot.Domain;
using MTD.CouchBot.Domain.Models.Bot;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace MTD.CouchBot.Dals.Implementations
{
    public class StatisticsDal : IStatisticsDal
    {
        public BotStats GetBotStats()
        {
            var path = Constants.ConfigRootDirectory + Constants.BotStatistics;

            if (!File.Exists(path))
            {
                var stats = new BotStats();
                stats.BeamAlertCount = 0;
                stats.TwitchAlertCount = 0;
                stats.UptimeMinutes = 0;
                stats.YouTubeAlertCount = 0;
                stats.VidMeAlertCount = 0;
                stats.PicartoAlertCount = 0;
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

        public void AddToPicartoAlertCount()
        {
            var stats = GetBotStats();
            stats.PicartoAlertCount++;
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
            stats.UptimeMinutes += 1;
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

        public int GetHaiBaiCount()
        {
            return GetBotStats().HaiBaiCount;
        }

        public void AddRandomInt(int random)
        {
            var stats = GetBotStats();
            if (stats.BeamSubIds == null)
            {
                stats.BeamSubIds = new List<int>();
            }

            stats.BeamSubIds.Add(random);
            SaveBotStats(stats);
        }

        public bool ContainsRandomInt(int random)
        {
            var stats = GetBotStats();

            if (stats.BeamSubIds == null)
            {
                stats.BeamSubIds = new List<int>();
                SaveBotStats(stats);

                return false;
            }

            return stats.BeamSubIds.Contains(random);
        }

        public void LogRestartTime()
        {
            var stats = GetBotStats();
            stats.LastRestart = DateTime.UtcNow;
            SaveBotStats(stats);
        }

        public void ClearRandomInts()
        {
            var botStats = GetBotStats();

            botStats.BeamSubIds = new List<int>();

            SaveBotStats(botStats);
        }

        public void AddToHaiBaiCount()
        {
            var stats = GetBotStats();
            stats.HaiBaiCount += 1;
            SaveBotStats(stats);
        }

        public void AddToFlipCount()
        {
            var stats = GetBotStats();
            stats.FlipCount += 1;
            SaveBotStats(stats);
        }

        public int GetFlipCount()
        {
            var stats = GetBotStats();

            return stats.FlipCount;
        }

        public void AddToUnflipCount()
        {
            var stats = GetBotStats();
            stats.UnflipCount += 1;
            SaveBotStats(stats);
        }

        public int GetUnflipCount()
        {
            var stats = GetBotStats();

            return stats.UnflipCount;
        }

        public void AddToVidMeAlertCount()
        {
            var stats = GetBotStats();
            stats.VidMeAlertCount++;
            SaveBotStats(stats);
        }

        public int GetVidMeAlertCount()
        {
            return GetBotStats().VidMeAlertCount;
        }
    }
}
