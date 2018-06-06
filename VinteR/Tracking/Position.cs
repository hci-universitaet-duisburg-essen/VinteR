using System.Numerics;

namespace VinteR.Tracking
{
    /// <summary>
    /// A position contains the location and rotation of an object or
    /// for example a local coordinate system inside a cartesian coordinate system.
    /// </summary>
    public class Position
    {
        public static readonly Position Zero = new Position()
        {
            Location = Vector3.Zero,
            Rotation = Quaternion.Identity
        };

        public Vector3 Location { get; set; }
        public Quaternion Rotation { get; set; }

        protected bool Equals(Position other)
        {
            return Location.Equals(other.Location) && Rotation.Equals(other.Rotation);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Position) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Location.GetHashCode() * 397) ^ Rotation.GetHashCode();
            }
        }

        public override string ToString()
        {
            return $"Location: {Location}, Rotation: {Rotation}";
        }
    }
}