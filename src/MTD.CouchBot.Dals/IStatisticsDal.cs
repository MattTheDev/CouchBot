using MTD.CouchBot.Domain.Models;
using MTD.CouchBot.Domain.Models.Bot;

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
        void AddToPicartoAlertCount();
        void AddToVidMeAlertCount();
        int GetVidMeAlertCount();
        void AddToHaiBaiCount();
        void AddUptimeMinutes();
        BotStats GetBotStats();
        void LogRestartTime();
        bool ContainsRandomInt(int random);
        void AddRandomInt(int random);
        void ClearRandomInts();
        int GetHaiBaiCount();
        int GetFlipCount();
        void AddToFlipCount();
        int GetUnflipCount();
        void AddToUnflipCount();
    }
}
