using MTD.CouchBot.Domain.Models;
using System.Threading.Tasks;

namespace MTD.CouchBot.Dals
{
    public interface IHitboxDal
    {
        Task<HitboxChannel> GetChannelByName(string name);
    }
}
