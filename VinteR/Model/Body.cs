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
    /// upon a collection of points. For various input adapters the
    /// body may have a more specific type. In addition a body has
    /// a rotation based on the global coordinate system.
    /// </summary>
    public class Body
    {   
        // This just defines a BodyType Enum we need a separate of type BodyType to hold the information
        public enum EBodyType
        {
            Marker,
            MarkerSet,
            RigidBody,
            Skeleton,
            Hand
        }

        // The Body Type of the Body object
        public EBodyType BodyType { get; set; }

        /// <summary>
        /// Collection of points that may be connected or are
        /// loose coupled and define the structure of this body.
        /// </summary>
        public IList<Point> Points { get; set; }

        /// <summary>
        /// Contains the rotation of this body inside the global
        /// coordinate system.
        /// </summary>
        public Quaternion Rotation { get; set; }

        /// <summary>
        /// Loads all values from properties of given source object
        /// into this body.
        /// </summary>
        /// <param name="source"></param>
        public void Load(Body source)
        {
            this.BodyType = source.BodyType;
            this.Points = source.Points;
            this.Rotation = source.Rotation;
        }
    }
}
