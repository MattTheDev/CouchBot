using MTD.CouchBot.Domain.Models.VidMe;
using System.Threading.Tasks;

namespace MTD.CouchBot.Managers
{
    public interface IVidMeManager
    {
        Task<VidMeUserVideos> GetChannelVideosById(int id);
        Task<int> GetIdByName(string name);
        Task<VidMeUser> GetUserById(int id);
    }
}
