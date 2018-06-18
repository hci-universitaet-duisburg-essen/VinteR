using System;
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

        public static Vector3 ToEulers(this Quaternion q)
        {
            var ysqr = q.Y * q.Y;

            var t0 = +2.0 * (q.W * q.X + q.Y * q.Z);
            var t1 = +1.0 - 2.0 * (q.X * q.X + ysqr);
            var X = Math.Atan2(t0, t1).ToDegrees();

            var t2 = +2.0 * (q.W * q.Y - q.Z * q.X);
            t2 = t2 > +1.0 ? 1.0 : t2;
            t2 = t2 < -1.0 ? -1.0 : t2;
            var Y = Math.Asin(t2).ToDegrees();

            var t3 = +2.0 * (q.W * q.Z + q.X * q.Y);
            var t4 = +1.0 - 2.0 * (ysqr * q.Z * q.Z);
            var Z = Math.Atan2(t3, t4).ToDegrees();

            return new Vector3(Convert.ToSingle(X), Convert.ToSingle(Y), Convert.ToSingle(Z));
        }
    }
}