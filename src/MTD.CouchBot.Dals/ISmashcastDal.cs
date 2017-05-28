using MTD.CouchBot.Domain.Models.Smashcast;
using System.Threading.Tasks;

namespace MTD.CouchBot.Dals
{
    public interface ISmashcastDal
    {
        Task<SmashcastChannel> GetChannelByName(string name);
    }
}
