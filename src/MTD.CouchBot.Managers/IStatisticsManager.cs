using MTD.CouchBot.Domain.Models;

namespace MTD.CouchBot.Managers
{
    public interface IStatisticsManager
    {
        int GetYouTubeAlertCount();
        int GetTwitchAlertCount();
        int GetBeamAlertCount();
        int GetHitboxAlertCount();
        int GetUptimeMinutes();
        void AddToYouTubeAlertCount();
        void AddToTwitchAlertCount();
        void AddToBeamAlertCount();
        void AddToHitboxAlertCount();
        void AddUptimeMinutes();
        BotStats GetBotStats();
        void LogRestartTime();
        bool ContainsRandomInt(int random);
        void AddRandomInt(int random);
        void ClearRandomInts();
    }
}
