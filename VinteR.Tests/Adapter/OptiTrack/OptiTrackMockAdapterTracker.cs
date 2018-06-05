using System.Numerics;
using VinteR.Adapter;

namespace VinteR.Tests.Adapter.OptiTrack
{
    public class OptiTrackMockAdapterTracker : IAdapterTracker
    {
        public Vector3? Locate(string name)
        {
            return Vector3.Zero;
        }
    }
}