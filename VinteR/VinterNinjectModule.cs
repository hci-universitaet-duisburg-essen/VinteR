using Ninject.Modules;
using VinteR.Configuration;
using VinteR.Transform;

namespace VinteR
{
    internal class VinterNinjectModule : NinjectModule
    {

        public override void Load()
        {
            Bind<IConfigurationService>().To<VinterConfigurationService>();
            Bind<ITransformator>().To<Transformator>();
        }
    }
}