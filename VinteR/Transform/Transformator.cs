using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using VinteR.Tracking;

namespace VinteR.Transform
{
    public class Transformator : ITransformator
    {
        public Vector3 GetGlobalPosition(Vector3 coordinateSystemPosition, Vector3 localPosition)
        {
            // Simply add the two vectors together to get
            return Vector3.Add(coordinateSystemPosition, localPosition);
        }

        public Vector3 GetGlobalPosition(Position coordinateSystemPosition, Vector3 localPosition)
        {
            /* Object is not rotated inside the local coordinate system,
             * but the coordinate system is rotated in the world
             * 1. Get the global position without respect to rotation
             * 2. Rotate the coordinate system
             */
            var result = Vector3.Transform(localPosition, coordinateSystemPosition.Rotation);
            result = Vector3.Add(result, coordinateSystemPosition.Location);
            return result;
        }

        public Vector3 GetCentroid(IEnumerable<Vector3> points)
        {
            var centroid = Vector3.Zero;
            var enumerable = points as Vector3[] ?? points.ToArray();
            if (!enumerable.Any())
            {
                return centroid;
            }

            centroid = enumerable.Aggregate(centroid, (current, point) => current + point);
            return centroid / enumerable.Count();
        }
    }
}