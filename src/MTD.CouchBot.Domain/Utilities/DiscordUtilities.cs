using System;
using Discord;

namespace MTD.CouchBot.Domain.Utilities
{
    public static class DiscordUtilities
    {
        public static Color GetRandomColor()
        {
            var random = new Random();

            return new Color(
                random.Next(1, 256),
                random.Next(1, 256),
                random.Next(1, 256));
        }
    }
}