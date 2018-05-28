using Ninject.Modules;
using VinteR.Adapter;
using VinteR.Adapter.Kinect;
using VinteR.Adapter.LeapMotion;
using VinteR.Configuration;

namespace VinteR
{
    internal class VinterNinjectModule : NinjectModule
    {

        public override void Load()
        {
            Bind<IConfigurationService>().To<VinterConfigurationService>().InSingletonScope();
            Bind<IInputAdapter>().To<LeapMotionAdapter>();
            Bind<IInputAdapter>().To<KinectAdapter>();
        }
    }
}