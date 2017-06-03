using MTD.CouchBot.Domain.Models.VidMe;
using System.Threading.Tasks;

namespace MTD.CouchBot.Managers
{
    public interface IVidMeManager
    {
        Task<VidMeChannelVideos> GetChannelVideosById(int id);
        Task<int> GetIdByName(string name);
    }
}
