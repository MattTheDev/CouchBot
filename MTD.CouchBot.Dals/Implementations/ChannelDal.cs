using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Configuration;
using MTD.CouchBot.Domain.Dtos.Discord;
using MTD.CouchBot.Domain.Enums;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTD.CouchBot.Dals.Implementations
{
    public class ChannelDal : IChannelDal
    {
        private readonly IConfiguration _configuration;

        public ChannelDal(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<GuildGroupChannel>> GetAllChannelsByGuildId(int guildGroupId)
        {
            using (var connection = new MySqlConnection(_configuration["ConnectionStrings:CouchBot"]))
            {
                await connection.OpenAsync();

                var query = "SELECT * FROM couchbot.guildgroupchannels WHERE GuildGroupId = @GuildGroupId";

                return (await connection.QueryAsync<GuildGroupChannel>(query, new { GuildGroupId = guildGroupId })).ToList();
            }
        }

        public async Task<List<GuildGroupChannel>> GetChannelsByGuildIdAndPlatform(int guildGroupId, Platform platform)
        {
            using (var connection = new MySqlConnection(_configuration["ConnectionStrings:CouchBot"]))
            {
                await connection.OpenAsync();

                var query = "SELECT * FROM couchbot.guildgroupchannels WHERE GuildGroupId = @GuildGroupId AND Platform = @Platform";

                return (await connection.QueryAsync<GuildGroupChannel>(query, new { GuildGroupId = guildGroupId, Platform = platform })).ToList();
            }
        }

        public async Task<List<GuildGroupChannel>> GetChannelByChannelId(string channelId)
        {
            using (var connection = new MySqlConnection(_configuration["ConnectionStrings:CouchBot"]))
            {
                await connection.OpenAsync();

                var query = "SELECT * FROM couchbot.guildgroupchannels WHERE ChannelId = @ChannelId";

                return (await connection.QueryAsync<GuildGroupChannel>(query, new { ChannelId = channelId })).ToList();
            }
        }

        public async Task<GuildGroupChannel> GetChannelByGuildIdAndChannelId(int guildGroupId, string channelId)
        {
            using (var connection = new MySqlConnection(_configuration["ConnectionStrings:CouchBot"]))
            {
                await connection.OpenAsync();

                var query = "SELECT * FROM couchbot.guildgroupchannels WHERE GuildGroupId = @GuildGroupId AND ChannelId = @ChannelId";

                return await connection.QueryFirstOrDefaultAsync<GuildGroupChannel>(query, new { GuildGroupId = guildGroupId, ChannelId = channelId });
            }
        }

        public async Task AddChannel(GuildGroupChannel channel)
        {
            using (var connection = new MySqlConnection(_configuration["ConnectionStrings:CouchBot"]))
            {
                await connection.OpenAsync();
                await connection.InsertAsync(channel);
            }
        }

        public async Task RemoveChannel(GuildGroupChannel channel)
        {
            using (var connection = new MySqlConnection(_configuration["ConnectionStrings:CouchBot"]))
            {
                await connection.OpenAsync();
                await connection.DeleteAsync(channel);
            }
        }
    }
}