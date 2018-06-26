using System.IO;
using VinteR.Model;

namespace VinteR.Serialization
{
    public interface ISerializer
    {
        /// <summary>
        /// Maps given mocap frame to a mocap frame defined inside the
        /// protobuf model. For faster calculation this should be
        /// done in each input adapter.
        /// </summary>
        /// <returns></returns>
        void ToProtoBuf(MocapFrame frame, out Model.Gen.MocapFrame output);
    }
}