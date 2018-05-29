using System.Numerics;

namespace VinteR.Transform
{
    public class Transformator : ITransformator
    {
        public Vector3 GetGlobalPosition(Vector3 localOrigin, Vector3 localPosition)
        {
            // Simply add the two vectors together to get
            return Vector3.Add(localOrigin, localPosition);
        }

        public Vector3 GetGlobalPosition(Vector3 localOrigin, Vector3 localPosition, Quaternion localObjectRotation)
        {
            /*
             * 1. rotate the local position without respect to the global coordinate
             * system. The local coordinate system is treated as root.
             * 2. Position the rotated object onto the global coordinate system
             */
            var result = Vector3.Transform(localPosition, localObjectRotation);
            result = Vector3.Add(result, localOrigin);
            return result;
        }

        public Vector3 GetGlobalPosition(Vector3 localOrigin, Vector3 localPosition, Quaternion localObjectRotation,
            Quaternion localOriginRotation)
        {
            /*
             * 1. Get the global position with already performed local rotation
             * 2. Rotate the global located point
             */
            var result = GetGlobalPosition(localOrigin, localPosition, localObjectRotation);
            result = Vector3.Transform(result, localOriginRotation);
            return result;
        }

    }
}