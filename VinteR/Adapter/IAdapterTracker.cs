using System.Numerics;

namespace VinteR.Adapter
{
    public interface IAdapterTracker
    {
        Vector3? Locate(string name);
    }
}