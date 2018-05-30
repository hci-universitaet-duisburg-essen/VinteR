using VinteR.Adapter;
using VinteR.Model;

namespace VinteR.Datamerge
{
    public partial class DataMerger
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly IAdapterTracker _adapterTracker;

        public DataMerger(IAdapterTracker adapterTracker)
        {
            this._adapterTracker = adapterTracker;
        }
    }
}