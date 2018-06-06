using System.Numerics;
using VinteR.Adapter;
using VinteR.Tracking;

namespace VinteR.Tests.Adapter.OptiTrack
{
    public class OptiTrackMockAdapterTracker : IAdapterTracker
    {
        public Position Locate(string name)
        {
            return new Position()
            {
                Location = Vector3.Zero,
                Rotation = Quaternion.Identity
            };
        }
    }
}