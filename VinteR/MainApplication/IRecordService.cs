using Ninject;
using VinteR.Model;

namespace VinteR.MainApplication
{
    public interface IRecordService
    {
        Session Session { get; }

        Session Start();

        void Stop();
    }
}