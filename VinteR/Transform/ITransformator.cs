using System.Collections.Generic;
using System.Numerics;
using VinteR.Tracking;

namespace VinteR.Transform
{
    /// <summary>
    /// A <code>ITransformator</code> is able to compute transforms of local and world
    /// coordinate system coordinates if they use the same units, for example millimeters,
    /// and the same system (right or left handed).
    ///
    /// The transformer operates on one global and one local coordinate system. For example
    /// a hand is tracked by the leap motion. The leap motion offers the local coordinate
    /// system, the hand contains all coordinates of fingers, bones and so on. The leap
    /// motion itself is tracked by another system that offers the global coordinate
    /// system. To calculate the global position of the hand, the position and rotation
    /// of the leap motion are necessary.
    /// </summary>
    public interface ITransformator
    {
        /// <summary>
        /// Returns the world coordinates of a point that is located inside a
        /// local coordinate system
        /// </summary>
        /// <param name="coordinateSystemPosition">Contains the global coordinate of the local coordinate system root</param>
        /// <param name="localPosition">Local position to the local coordinate system root</param>
        /// <returns>The computed world position</returns>
        Vector3 GetGlobalPosition(Vector3 coordinateSystemPosition, Vector3 localPosition);

        /// <summary>
        /// Returns the world coordinates of an object that is located inside a
        /// local coordinate system and was rotated inside the local coordination system
        /// system.
        /// </summary>
        /// <param name="coordinateSystemPosition">Contains the global coordinate of the local coordinate system root</param>
        /// <param name="localPosition">Position relative to the local coordinate system root</param>
        /// <param name="localObjectRotation">Rotation relative to the local coordinate system root</param>
        /// <returns>The computed world rotation</returns>
        Vector3 GetGlobalPosition(Vector3 coordinateSystemPosition, Vector3 localPosition,
            Quaternion localObjectRotation);

        /// <summary>
        /// Returns the rotation inside the world of a rotation given inside a local coordinate system.
        /// system.
        /// </summary>
        /// <param name="coordinateSystemPosition">Contains the global coordinate of the local coordinate system root</param>
        /// <param name="coordinateSystemRotation">Rotation of the root of the local coordinate system</param>
        /// <param name="localPosition">Position relative to the local coordinate system root</param>
        /// <param name="localObjectRotation">Rotation relative to the local coordinate system root</param>
        /// <returns>The computed world rotation</returns>
        Vector3 GetGlobalPosition(Vector3 coordinateSystemPosition, Quaternion coordinateSystemRotation,
            Vector3 localPosition, Quaternion localObjectRotation);

        /// <summary>
        /// Returns the rotation inside the world of a rotation given inside a local coordinate system.
        /// system.
        /// </summary>
        /// <param name="coordinateSystemPosition">Contains the global position of the local coordinate system root</param>
        /// <param name="localPosition">Position relative to the local coordinate system root</param>
        /// <param name="localObjectRotation">Rotation relative to the local coordinate system root</param>
        /// <returns>The computed world rotation</returns>
        Vector3 GetGlobalPosition(Position coordinateSystemPosition, Vector3 localPosition, Quaternion localObjectRotation);

        /// <summary>
        /// Returns the centroid of all given points
        /// </summary>
        /// <param name="points">All points that are relevant for the calculation</param>
        /// <returns>A <code>Vector3</code> instance</returns>
        Vector3 GetCentroid(IEnumerable<Vector3> points);
    }
}