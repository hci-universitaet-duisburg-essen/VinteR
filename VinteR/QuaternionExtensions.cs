using System.Numerics;
using VinteR.Model.Gen;

namespace VinteR
{
    public static class QuaternionExtensions
    {
        public static MocapFrame.Types.Body.Types.Quaternion ToProto(this Quaternion quaternion)
        {
            return new MocapFrame.Types.Body.Types.Quaternion()
            {
                X = quaternion.X,
                Y = quaternion.Y,
                Z = quaternion.Z,
                W = quaternion.W,
            };
        }
    }
}