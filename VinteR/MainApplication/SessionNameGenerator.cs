using CodenameGenerator;

namespace VinteR.MainApplication
{
    public class SessionNameGenerator : ISessionNameGenerator
    {
        private Generator _generator;

        public SessionNameGenerator()
        {
            _generator = new Generator();
        }

        public string Generate()
        {
            return _generator.Generate();
        }
    }
}