using System;
using MTD.CouchBot.Dals;
using MTD.CouchBot.Dals.Implementations;
using MTD.CouchBot.Domain.Models.Bot;

namespace MTD.CouchBot.Managers.Implementations
{
    public class StatisticsManager : IStatisticsManager
    {
        IStatisticsDal _statisticsDal;

        public StatisticsManager()
        {
            _statisticsDal = new StatisticsDal();
        }

        public void AddToBeamAlertCount()
        {
            _statisticsDal.AddToBeamAlertCount();
        }

        public void AddToTwitchAlertCount()
        {
            _statisticsDal.AddToTwitchAlertCount();
        }

        public void AddToYouTubeAlertCount()
        {
            _statisticsDal.AddToYouTubeAlertCount();
        }

        public void AddToHitboxAlertCount()
        {
            _statisticsDal.AddToHitboxAlertCount();
        }

        public void AddToPicartoAlertCount()
        {
            _statisticsDal.AddToPicartoAlertCount();
        }

        public void AddUptimeMinutes()
        {
            _statisticsDal.AddUptimeMinutes();
        }

        public int GetBeamAlertCount()
        {
            return _statisticsDal.GetBeamAlertCount();
        }

        public int GetTwitchAlertCount()
        {
            return _statisticsDal.GetTwitchAlertCount();
        }

        public int GetUptimeMinutes()
        {
            return _statisticsDal.GetUptimeMinutes();
        }

        public int GetYouTubeAlertCount()
        {
            return _statisticsDal.GetYouTubeAlertCount();
        }

        public int GetHitboxAlertCount()
        {
            return _statisticsDal.GetHitboxAlertCount();
        }

        public BotStats GetBotStats()
        {
            return _statisticsDal.GetBotStats();
        }

        public void LogRestartTime()
        {
            _statisticsDal.LogRestartTime();
        }

        public bool ContainsRandomInt(int random)
        {
            return _statisticsDal.ContainsRandomInt(random);
        }

        public void AddRandomInt(int random)
        {
            _statisticsDal.AddRandomInt(random);
        }

        public void ClearRandomInts()
        {
            _statisticsDal.ClearRandomInts();
        }

        public void AddToHaiBaiCount()
        {
            _statisticsDal.AddToHaiBaiCount();
        }

        public int GetHaiBaiCount()
        {
            return _statisticsDal.GetHaiBaiCount();
        }

        public void AddToFlipCount()
        {
            _statisticsDal.AddToFlipCount();
        }

        public int GetFlipCount()
        {
            return _statisticsDal.GetFlipCount();
        }

        public void AddToUnflipCount()
        {
            _statisticsDal.AddToUnflipCount();
        }

        public int GetUnflipCount()
        {
            return _statisticsDal.GetUnflipCount();
        }

        public void AddToVidMeAlertCount()
        {
            _statisticsDal.AddToVidMeAlertCount();
        }

        public int GetVidMeAlertCount()
        {
            return GetVidMeAlertCount();
        }
    }
}
