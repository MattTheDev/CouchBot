using System;
using MTD.CouchBot.Dals;
using MTD.CouchBot.Dals.Implementations;
using MTD.CouchBot.Domain.Models.Bot;

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

        public void AddToPicartoAlertCount()
        {
            statisticsDal.AddToPicartoAlertCount();
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

        public bool ContainsRandomInt(int random)
        {
            return statisticsDal.ContainsRandomInt(random);
        }

        public void AddRandomInt(int random)
        {
            statisticsDal.AddRandomInt(random);
        }

        public void ClearRandomInts()
        {
            statisticsDal.ClearRandomInts();
        }

        public void AddToHaiBaiCount()
        {
            statisticsDal.AddToHaiBaiCount();
        }

        public int GetHaiBaiCount()
        {
            return statisticsDal.GetHaiBaiCount();
        }
    }
}
