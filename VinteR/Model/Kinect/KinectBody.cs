using System.Collections.Generic;

namespace VinteR.Model.Kinect
{
    /// <inheritdoc/>
    /// <summary>
    /// The standard body type for the Kinect (Skeleton)
    /// There is currently no explicit player tracking implemented,
    /// player assignment will be likely done by matching a single marker from optitrack
    /// to a Point of the Skeleton i.e. Head.
    /// </summary>
    public class KinectBody : Body
    {

        // Rotation, the Skeleton of a Kinect has no orientation information, it is always oriented towards the Kinect i.e. fixed

        // The Kinect has also a video frame and a depth frame with pixels, this is ignored here
        // and extension can be provided to the KinectBody once this information is required.

        public KinectBody(IList<Point> list, EBodyType type)
        {
            this.Joints = list;
            this.BodyType = type;
        }
    }
}