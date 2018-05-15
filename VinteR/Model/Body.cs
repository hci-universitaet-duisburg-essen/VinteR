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
        public enum BodyType
        {
            Marker,
            MarkerSet,
            RigidBody,
            Skeleton
        }

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

    public class KinectBody : Body { }
}
