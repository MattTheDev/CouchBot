using Discord;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MTD.CouchBot.Bot.Utilities
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

                if (guild != null)
                {
                    try
                    {
                        channel = (IMessageChannel)await guild.GetChannelAsync(channelId);
                    }
                    catch (Exception ex)
                    {
                        channel = null;
                    }
                }
            }

            return channel;
        }

        public static async Task<IRole> GetRoleByGuildAndId(ulong guildId, ulong roleId)
        {
            IGuild guild = null;
            IRole role = null;

            if (guildId != 0 && roleId != 0)
            {
                guild = Program.client.GetGuild(guildId);

                if (guild != null)
                {
                    try
                    {
                        role = guild.GetRole(roleId);
                    }
                    catch (Exception ex)
                    {
                        role = null;
                    }
                }
            }

            return role;
        }

        public static async Task DeleteMessage(ulong guildId, ulong channelId, ulong messageId)
        {
            IGuild guild = null;
            IMessageChannel channel = null;

            if (guildId != 0 && channelId != 0)
            {
                guild = Program.client.GetGuild(guildId);

                if (guild != null)
                {
                    try
                    {
                        var messages = new List<IMessage>();
                        channel = (IMessageChannel)await guild.GetChannelAsync(channelId);
                        messages.Add(await channel.GetMessageAsync(messageId));
                        await channel.DeleteMessagesAsync(messages);
                    }
                    catch (Exception ex)
                    {
                        channel = null;
                    }
                }
            }
        }

        public static async Task SetOfflineStream(ulong guildId, ulong channelId, ulong messageId)
        {
            IGuild guild = null;
            IMessageChannel channel = null;

            if (guildId != 0 && channelId != 0)
            {
                guild = Program.client.GetGuild(guildId);

                if (guild != null)
                {
                    try
                    {
                        channel = (IMessageChannel)await guild.GetChannelAsync(channelId);
                        var message = (IUserMessage) await channel.GetMessageAsync(messageId);
                        await message.ModifyAsync(m => m.Content += "This stream is now offline.");
                    }
                    catch (Exception ex)
                    {
                        channel = null;
                    }
                }
            }
        }
    }
}
