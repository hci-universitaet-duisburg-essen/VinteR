using VinteR.Model;

namespace VinteR.Datamerge
{
    public interface IDataMerger
    {
        MocapFrame HandleFrame(MocapFrame frame);
    }
}