using Discord;

namespace CB.Shared;

public static class Constants
{
    public static class AllowOptions
    {
        public const string GreetingsOption = "greetings";
        public const string GoodbyesOption = "goodbyes";
        public const string LiveOption = "live";
        public const string PublishedOption = "published";
        public const string StreamVodOption = "stream vod";
        public const string ThumbnailsOption = "thumbnails";
        public const string FfaOption = "ffa";
        public const string CrosspostOption = "crosspost";
        public const string DeleteOfflineOption = "deleteoffline";
        public const string TextAnnouncements = "textannouncements";
        public const string DiscordLiveAnnouncements = "discordlive";
    }

    public static class Colors
    {
        public static Color YouTubeRed = new(0xFF0000);
        public static Color PicartoWhite = new(0xFFFFFF);
        public static Color TwitchPurple = new(0x6441A5);
    }
}