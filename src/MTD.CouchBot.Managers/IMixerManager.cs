using System.Collections.Generic;
using MTD.CouchBot.Domain.Models.Mixer;
using System.Threading.Tasks;

namespace MTD.CouchBot.Managers
{
    public interface IMixerManager
    {
        Task<MixerChannel> GetChannelByName(string name);
        Task<MixerChannel> GetChannelById(string id);
        Task<MixerUserResponse> GetUserById(string id);
        Task<MixerTeamResponse> GetTeamByToken(string token);
        Task<List<MixerTeamUserResponse>> GetTeamUsersByTeamId(int teamId);
        Task<MixerTeamResponse> GetTeamById(int id);
        Task<List<MixerUserTeamResponse>> GetTeamsByUserId(string id);
    }
}
