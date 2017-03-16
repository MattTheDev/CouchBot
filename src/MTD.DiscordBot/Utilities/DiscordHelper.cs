using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTD.DiscordBot.Utilities
{
    public static class DiscordHelper
    {
        public static async Task<IMessageChannel> GetMessageChannel(ulong guildId, ulong channelId)
        {
            IGuild guild = null;
            IMessageChannel channel = null;

            if (guildId != 0 && channelId != 0)
            {
                guild = Program.client.GetGuild(guildId);

                if(guild != null)
                {
                    try
                    {
                        channel = (IMessageChannel)await guild.GetChannelAsync(channelId);
                    }
                    catch(Exception ex)
                    {
                        channel = null;
                    }
                }
            }

            return channel;
        }
    }
}
