using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinteR.Model
{
    /// <summary>
    /// A joint contains the global coordinates in millimeter
    /// where it is located inside the world. It may have a name
    /// to get specific information what this joint represents.
    /// </summary>
    public class Joint
    {
        /// <summary>
        /// Optional name of the joint
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Global x coordinate of the joint in millimeter
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Global y coordinate of the joint in millimeter
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// Global z coordinate of the joint in millimeter
        /// </summary>
        public float Z { get; set; }

        public Joint(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
    }
}
