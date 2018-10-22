using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Configuration;
using MTD.CouchBot.Domain.Dtos.Discord;
using MTD.CouchBot.Domain.Utilities;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;

namespace MTD.CouchBot.Dals.Implementations
{
    public class GuildDal : IGuildDal
    {
        private readonly IConfiguration _configuration;

        public GuildDal(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        #region : Get Methods :

        public async Task<Guild> GetGuildById(ulong id)
        {
            using (var connection = new MySqlConnection(_configuration["ConnectionStrings:CouchBot"]))
            {
                await connection.OpenAsync();

                var query = "SELECT * FROM couchbot.guilds WHERE GuildId = @GuildId";

                return await connection.QueryFirstOrDefaultAsync<Guild>(query, new {GuildId = Cryptography.Encrypt(id.ToString())});
            }
        }

        public async Task<GuildConfiguration> GetGuildConfigurationByGuildId(ulong id)
        {
            using (var connection = new MySqlConnection(_configuration["ConnectionStrings:CouchBot"]))
            {
                await connection.OpenAsync();

                var query = "SELECT * FROM couchbot.guildconfigurations WHERE GuildId = @GuildId";

                return await connection.QueryFirstOrDefaultAsync<GuildConfiguration>(query, new { GuildId = Cryptography.Encrypt(id.ToString()) });
            }
        }

        #endregion

        #region : Create Methods :

        public async Task CreateGuild(Guild guild)
        {
            using (var connection = new MySqlConnection(_configuration["ConnectionStrings:CouchBot"]))
            {
                await connection.OpenAsync();
                await connection.InsertAsync(guild);
            }
        }

        public async Task CreateGuildConfiguration(GuildConfiguration guildConfiguration)
        {
            using (var connection = new MySqlConnection(_configuration["ConnectionStrings:CouchBot"]))
            {
                await connection.OpenAsync();
                await connection.InsertAsync(guildConfiguration);
            }
        }

        #endregion

        #region : Update Methods :

        public async Task UpdateGuildConfiguration(GuildConfiguration guildConfiguration)
        {
            using (var connection = new MySqlConnection(_configuration["ConnectionStrings:CouchBot"]))
            {
                await connection.OpenAsync();
                await connection.UpdateAsync(guildConfiguration);
            }
        }

        #endregion

        #region : Delete Methods :

        public async Task DeleteGuild(Guild guild)
        {
            using (var connection = new MySqlConnection(_configuration["ConnectionStrings:CouchBot"]))
            {
                await connection.OpenAsync();
                await connection.DeleteAsync(guild);
            }
        }

        public async Task DeleteGuildConfiguration(GuildConfiguration guildConfiguration)
        {
            using (var connection = new MySqlConnection(_configuration["ConnectionStrings:CouchBot"]))
            {
                await connection.OpenAsync();
                await connection.DeleteAsync(guildConfiguration);
            }
        }

        #endregion
    }
}