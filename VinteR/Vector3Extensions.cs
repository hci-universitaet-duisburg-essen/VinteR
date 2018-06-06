using System;
using System.Numerics;

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
    }
}