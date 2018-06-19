using System;
using System.Numerics;
using VinteR.Model.Gen;

namespace VinteR
{
    public static class Vector3Extensions
    {
        public static Vector3 Round(this Vector3 vector, int decimals = 2)
        {
            var v = new Vector3
            {
                X = (float) Math.Round(vector.X, decimals),
                Y = (float) Math.Round(vector.Y, decimals),
                Z = (float) Math.Round(vector.Z, decimals)
            };
            return v;
        }

        public static VinteR.Model.Gen.MocapFrame.Types.Body.Types.Vector3 ToProto(this Vector3 point)
        {
            return new MocapFrame.Types.Body.Types.Vector3()
            {
                X = point.X,
                Y = point.Y,
                Z = point.Z
            };
        }
    }
}