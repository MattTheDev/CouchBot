using MTD.CouchBot.Domain.Models.Smashcast;
using System.Threading.Tasks;

namespace MTD.CouchBot.Managers
{
    public interface ISmashcastManager
    {
        Task<SmashcastChannel> GetChannelByName(string name);
    }
}
