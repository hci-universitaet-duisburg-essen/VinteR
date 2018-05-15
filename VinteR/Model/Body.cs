using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace VinteR.Model
{
    /// <summary>
    /// A <code>Body</code> defines a single object that is build
    /// upon a collection of joints. For various input adapters the
    /// body may have a more specific type. In addition a body has
    /// a rotation based on the global coordinate system.
    /// </summary>
    public class Body
    {   
        // This just defines a BodyType Enum we need a separate of type BodyType to hold the information
        public enum BodyTypeEnum
        {
            Marker,
            MarkerSet,
            RigidBody,
            Skeleton
        }

        // The Body Type of the Body object
        public BodyTypeEnum BodyType { get; set; }

        /// <summary>
        /// Collection of joints that may be connected or are
        /// loose coupled and define the structure of this body.
        /// </summary>
        public IList<Joint> Joints { get; set; }

        /// <summary>
        /// Contains the rotation of this body inside the global
        /// coordinate system.
        /// </summary>
        public Quaternion Rotation { get; set; }
    }

    // TODO: remove these sample classes into specific input adapter implementations


    /*
     * The standard body type for the Kinect (Skeleton)
     * There is currently no explicit player tracking implemented,
     * player assignment will be likely done by matching a single marker from optitrack
     * to a Joint of the Skeleton i.e. Head.
     */
    public class KinectBody : Body {

        // Rotation, the Skeleton of a Kinect has no orientation information, it is always oriented towards the Kinect i.e. fixed

        // The Kinect has also a video frame and a depth frame with pixels, this is ignored here
        // and extension can be provided to the KinectBody once this information is required.

        public KinectBody(List<Joint> list, BodyTypeEnum type)
        {
            this.Joints = list;
            this.BodyType = type;
        }


    }

    public class OptiTrackBody : Body
    {
        public BodyType Type { get; set; }
    }
}
