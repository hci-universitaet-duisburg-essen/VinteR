using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
        /// Global position of this joint.
        /// </summary>
        public Vector3 Position { get; set; }

        public Joint(float x, float y, float z)
        {
            this.Position = new Vector3(x, y, z);
        }

        public Joint(Vector3 position)
        {
            this.Position = position;
        }
    }
}
