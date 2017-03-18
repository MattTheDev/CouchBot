using MTD.CouchBot.Dals;
using MTD.CouchBot.Dals.Implementations;
using MTD.CouchBot.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTD.CouchBot.Managers.Implementations
{
    public class StatisticsManager : IStatisticsManager
    {
        IStatisticsDal statisticsDal;

        public StatisticsManager()
        {
            statisticsDal = new StatisticsDal();
        }

        public void AddToBeamAlertCount()
        {
            statisticsDal.AddToBeamAlertCount();
        }

        public void AddToTwitchAlertCount()
        {
            statisticsDal.AddToTwitchAlertCount();
        }

        public void AddToYouTubeAlertCount()
        {
            statisticsDal.AddToYouTubeAlertCount();
        }

        public void AddToHitboxAlertCount()
        {
            statisticsDal.AddToHitboxAlertCount();
        }

        public void AddUptimeMinutes()
        {
            statisticsDal.AddUptimeMinutes();
        }

        public int GetBeamAlertCount()
        {
            return statisticsDal.GetBeamAlertCount();
        }

        public int GetTwitchAlertCount()
        {
            return statisticsDal.GetTwitchAlertCount();
        }

        public int GetUptimeMinutes()
        {
            return statisticsDal.GetUptimeMinutes();
        }

        public int GetYouTubeAlertCount()
        {
            return statisticsDal.GetYouTubeAlertCount();
        }

        public int GetHitboxAlertCount()
        {
            return statisticsDal.GetHitboxAlertCount();
        }

        public BotStats GetBotStats()
        {
            return statisticsDal.GetBotStats();
        }

        public void LogRestartTime()
        {
            statisticsDal.LogRestartTime();
        }
    }
}
