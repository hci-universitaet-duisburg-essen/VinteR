using System;

namespace VinteR
{
    public static class FloatExtensions
    {
        public static double ToRadians(this double degrees)
        {
            return Math.PI * degrees / 180;
        }
        public static float ToRadians(this float degrees)
        {
            return (float) Math.PI * degrees / 180;
        }

        public static double ToDegrees(this double radians)
        {
            return radians * 180 / Math.PI;
        }
    }
}