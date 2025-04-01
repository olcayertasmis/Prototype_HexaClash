using System.Threading.Tasks;
using HexaClash.Game.Scripts.Data.ValueData;

namespace HexaClash.Game.Scripts.Interfaces
{
    public interface IResourceCollector
    {
        Task AnimateResourceCollection(ResourceAnimationData data);
    }
}