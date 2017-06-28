using MTD.CouchBot.Domain.Models.VidMe;
using System.Threading.Tasks;

namespace MTD.CouchBot.Dals
{
    public interface IVidMeDal
    {
        Task<VidMeUserVideos> GetUserVideosById(int id);
        Task<int> GetIdByName(string name);
        Task<VidMeUser> GetUserById(int id);
    }
}
