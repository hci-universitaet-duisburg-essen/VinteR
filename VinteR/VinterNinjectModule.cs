using Ninject.Modules;
using VinteR.Configuration;

namespace VinteR
{
    internal class VinterNinjectModule : NinjectModule
    {

        public override void Load()
        {
            Bind<IConfigurationService>().To<VinterConfigurationService>();
        }
    }
}