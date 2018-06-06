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
            var result = Vector3.Add(coordinateSystemPosition.Location, localPosition);
            result = Vector3.Transform(result, coordinateSystemPosition.Rotation);
            return result;
        }

        public Vector3 GetGlobalPosition(Vector3 coordinateSystemPosition, Vector3 localPosition,
            Quaternion localObjectRotation)
        {
            /*
             * 1. rotate the local position without respect to the global coordinate
             * system. The local coordinate system is treated as root.
             * 2. Position the rotated object onto the global coordinate system
             */
            var result = Vector3.Transform(localPosition, localObjectRotation);
            result = Vector3.Add(result, coordinateSystemPosition);
            return result;
        }

        public Vector3 GetGlobalPosition(Vector3 coordinateSystemPosition, Quaternion coordinateSystemRotation,
            Vector3 localPosition, Quaternion localObjectRotation)
        {
            /*
             * 1. Get the global position with already performed local rotation
             * 2. Rotate the global located point
             */
            var result = GetGlobalPosition(coordinateSystemPosition, localPosition, localObjectRotation);
            result = Vector3.Transform(result, coordinateSystemRotation);
            return result;
        }

        public Vector3 GetGlobalPosition(Position coordinateSystemPosition, Vector3 localPosition,
            Quaternion localObjectRotation)
        {
            return GetGlobalPosition(coordinateSystemPosition.Location, coordinateSystemPosition.Rotation,
                localPosition, localObjectRotation);
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