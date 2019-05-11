using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTD.CouchBot.Services
{
    public class DiscordService
    {
        private readonly DiscordShardedClient _discord;

        public DiscordService(DiscordShardedClient discord)
        {
            _discord = discord;
        }

        public async Task<IMessageChannel> GetMessageChannel(ulong guildId, ulong channelId)
        {
            IGuild guild = null;
            IMessageChannel channel = null;

            if (guildId != 0 && channelId != 0)
            {
                guild = _discord.GetGuild(guildId);

                if (guild != null)
                {
                    try
                    {
                        channel = (IMessageChannel)await guild.GetChannelAsync(channelId);
                    }
                    catch (Exception)
                    {
                        channel = null;
                    }
                }
            }

            return channel;
        }

        public IRole GetRoleByGuildAndId(ulong guildId, ulong roleId)
        {
            IGuild guild = null;
            IRole role = null;

            if (guildId != 0 && roleId != 0)
            {
                guild = _discord.GetGuild(guildId);

                if (guild != null)
                {
                    try
                    {
                        role = guild.GetRole(roleId);
                    }
                    catch (Exception)
                    {
                        role = null;
                    }
                }
            }

            return role;
        }

        public async Task DeleteMessage(ulong guildId, ulong channelId, ulong messageId)
        {
            IGuild guild = null;
            IMessageChannel channel = null;

            if (guildId != 0 && channelId != 0)
            {
                guild = _discord.GetGuild(guildId);

                if (guild != null)
                {
                    try
                    {
                        var messages = new List<IMessage>();
                        channel = (IMessageChannel)await guild.GetChannelAsync(channelId);
                        var test = await channel.GetMessageAsync(messageId);
                        await test.DeleteAsync();
                    }
                    catch (Exception)
                    {
                        channel = null;
                    }
                }
            }
        }

        public async Task SetOfflineStream(ulong guildId, string offlineMessage, ulong channelId, ulong messageId)
        {
            IGuild guild = null;
            IMessageChannel channel = null;

            if (guildId != 0 && channelId != 0)
            {
                guild = _discord.GetGuild(guildId);

                if (guild != null)
                {
                    try
                    {
                        channel = (IMessageChannel)await guild.GetChannelAsync(channelId);
                        var message = (IUserMessage)await channel.GetMessageAsync(messageId);
                        await message.ModifyAsync(m => m.Content += offlineMessage);
                    }
                    catch (Exception)
                    {
                        channel = null;
                    }
                }
            }
        }

        public async Task<string> GetRandomGuildUser(IGuild guild)
        {
            var users = (await guild.GetUsersAsync(CacheMode.AllowDownload));

            var random = new Random().Next(0, users.Count - 1);

            return users.ElementAt(random).Username;
        }
    }
}
