using MTD.CouchBot.Domain.Models;

namespace MTD.CouchBot.Dals
{
    public interface IStatisticsDal
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
    }
}
