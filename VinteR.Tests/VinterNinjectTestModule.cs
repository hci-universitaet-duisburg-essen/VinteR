using VinteR.Adapter;
using VinteR.Adapter.OptiTrack;
using VinteR.Tests.Adapter.OptiTrack;

namespace VinteR.Tests
{
    public class VinterNinjectTestModule : VinterNinjectModule
    {
        public override void Load()
        {
            base.Load();
            Rebind<IOptiTrackClient>().To<OptiTrackMockClient>();
        }
    }
}