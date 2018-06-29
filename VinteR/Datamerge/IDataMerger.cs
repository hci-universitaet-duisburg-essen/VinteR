using VinteR.Model;

namespace VinteR.Datamerge
{
    public interface IDataMerger
    {
        string MergerType { get; }

        MocapFrame HandleFrame(MocapFrame frame);
    }
}