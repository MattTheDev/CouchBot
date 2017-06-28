using MTD.CouchBot.Dals;
using MTD.CouchBot.Dals.Implementations;
using MTD.CouchBot.Domain.Models.VidMe;
using System.Threading.Tasks;
using System;

namespace MTD.CouchBot.Managers.Implementations
{
    public class VidMeManager : IVidMeManager
    {
        IVidMeDal _vidMeDal;

        public VidMeManager()
        {
            _vidMeDal = new VidMeDal();
        }

        public async Task<VidMeUserVideos> GetChannelVideosById(int id)
        {
            return await _vidMeDal.GetUserVideosById(id);
        }

        public async Task<int> GetIdByName(string name)
        {
            return await _vidMeDal.GetIdByName(name);
        }

        public async Task<VidMeUser> GetUserById(int id)
        {
            return await _vidMeDal.GetUserById(id);
        }
    }
}
