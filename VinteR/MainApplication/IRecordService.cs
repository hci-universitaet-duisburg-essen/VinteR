using Ninject;

namespace VinteR.MainApplication
{
    public interface IRecordService
    {
        bool IsRecording { get; }

        void Start();

        void Stop();
    }
}