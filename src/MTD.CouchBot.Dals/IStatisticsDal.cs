using MTD.CouchBot.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
