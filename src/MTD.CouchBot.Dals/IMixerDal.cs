using System.Collections.Generic;
using MTD.CouchBot.Domain.Models.Mixer;
using System.Threading.Tasks;

namespace MTD.CouchBot.Dals
{
    public interface IMixerDal
    {
        Task<MixerChannel> GetChannelByName(string name);
        Task<MixerChannel> GetChannelById(string id);
        Task<MixerUserResponse> GetUserById(string id);
        Task<MixerTeamResponse> GetTeamByToken(string token);
        Task<MixerTeamResponse> GetTeamById(int id);
        Task<List<MixerTeamUserResponse>> GetTeamUsersByTeamId(int teamId);
        Task<List<MixerUserTeamResponse>> GetTeamsByUserId(string id);
    }
}
