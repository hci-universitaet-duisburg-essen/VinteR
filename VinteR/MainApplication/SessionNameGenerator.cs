using CodenameGenerator;

namespace VinteR.MainApplication
{
    public class SessionNameGenerator : ISessionNameGenerator
    {
        private readonly Generator _generator;

        public SessionNameGenerator()
        {
            _generator = new Generator {Separator = "-"};
        }

        public string Generate()
        {
            return _generator.Generate();
        }
    }
}