using Ninject.Modules;
using VinteR.Adapter;
using VinteR.Adapter.Kinect;
using VinteR.Adapter.LeapMotion;
using VinteR.Adapter.OptiTrack;
using VinteR.Configuration;
using VinteR.Datamerge;
using VinteR.MainApplication;
using VinteR.Transform;

namespace VinteR
{
    public class VinterNinjectModule : NinjectModule
    {

        public override void Load()
        {
            Bind<IMainApplication>().To<MainApplicaiton>();
            Bind<IConfigurationService>().To<VinterConfigurationService>().InSingletonScope();
            Bind<IInputAdapter>().To<LeapMotionAdapter>();
            Bind<IInputAdapter>().To<KinectAdapter>();
            Bind<IInputAdapter>().To<OptiTrackAdapter>();
            Bind<ITransformator>().To<Transformator>();
            Bind<IAdapterTracker>().To<OptiTrackAdapterTracker>();
            Bind<IOptiTrackClient>().To<OptiTrackClient>().InSingletonScope();
            Bind<DataMerger>().To<DataMerger>();
        }
    }
}