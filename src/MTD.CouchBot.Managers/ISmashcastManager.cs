using MTD.CouchBot.Domain.Models;
using System.Threading.Tasks;

namespace MTD.CouchBot.Managers
{
    public interface ISmashcastManager
    {
        Task<HitboxChannel> GetChannelByName(string name);
    }
}
