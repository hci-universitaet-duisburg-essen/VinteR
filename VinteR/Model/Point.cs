using System.Numerics;

namespace VinteR.Model
{
    /// <summary>
    /// A point contains the global coordinates in millimeter
    /// where it is located inside the world. It may have a name
    /// to get specific information what this üoint represents.
    /// </summary>
    public class Point
    {
        /// <summary>
        /// Optional name of the point
        /// </summary>
        public string Name { get; set; } = "";

        /*
         * Allows to get information the validity of the Point
         * The point can be untracked (no acurate information available)
         * or tracked and there are valid information avaialble or
         * kust inferred (approximated by the position of other Points)
         */
        public string State { get; set; } = "";

        /// <summary>
        /// Global position of this point.
        /// </summary>
        public Vector3 Position { get; set; }

        public Point(float x, float y, float z)
        {
            this.Position = new Vector3(x, y, z);
        }

        public Point(Vector3 position)
        {
            this.Position = position;
        }
    }
}
