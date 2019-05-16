using System.Collections.Generic;
using System.Threading.Tasks;
using MTD.CouchBot.Dals;
using MTD.CouchBot.Domain.Models.Mixer;

namespace MTD.CouchBot.Managers
{
    public class MixerManager
    {
        private readonly IMixerDal _mixerDal;

        public MixerManager(IMixerDal mixerDal)
        {
            _mixerDal = mixerDal;
        }

        public async Task<MixerChannel> GetChannelById(string id)
        {
            return await _mixerDal.GetChannelById(id);
        }

        public async Task<MixerUserResponse> GetUserById(string id)
        {
            return await _mixerDal.GetUserById(id);
        }

        public async Task<MixerTeamResponse> GetTeamByToken(string token)
        {
            return await _mixerDal.GetTeamByToken(token);
        }

        public async Task<List<MixerTeamUserResponse>> GetTeamUsersByTeamId(int teamId)
        {
            return await _mixerDal.GetTeamUsersByTeamId(teamId);
        }

        public async Task<MixerTeamResponse> GetTeamById(int id)
        {
            return await _mixerDal.GetTeamById(id);
        }

        public async Task<List<MixerUserTeamResponse>> GetTeamsByUserId(string id)
        {
            return await _mixerDal.GetTeamsByUserId(id);
        }

        public async Task<MixerChannel> GetChannelByName(string name)
        {
            return await _mixerDal.GetChannelByName(name);
        }
    }
}
