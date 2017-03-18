using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTD.CouchBot.Domain
{
    /*
     * ame: CouchBot
Identifier: d5b089df2f36d4a0a8cff945015a5a4456584f5
Secret: d277157efb28735dccdc6783be21c10679a91d9
Endpoint: http://multiyt.tv/CouchBot
     */
    public static class Constants
    {
        public static readonly string ConfigRootDirectory = @"c:\programdata\CouchBot\";
        public static readonly string GuildDirectory = @"Guilds\";
        public static readonly string UserDirectory = @"Users\";
        public static readonly string LiveDirectory = @"Live\";
        public static readonly string TwitchDirectory = @"Twitch\";
        public static readonly string YouTubeDirectory = @"YouTube\";
        public static readonly string BeamDirectory = @"Beam\";
        public static readonly string HitboxDirectory = @"Hitbox\";
        public static readonly string BotStatistics = "BotStats.json";

        public static readonly string DiscordToken = "DISCORDBOTTOKEN";
        public static readonly string TwitchClientId = "TWITCHCLIENTID";

        public static readonly ulong CouchBotId = 1234567890; //Replace with your Bot ID.
    }
}
