using System.Numerics;

namespace VinteR.Model.OptiTrack
{
    /// <inheritdoc />
    /// <summary>
    /// Describes a RigidBody - A named object with position and orientation,
    /// typically defined by 3 or more markers
    /// </summary>
    public class RigidBody : OptiTrackBody
    {
        /// <summary>
        /// Rotation in relation to the optitrack coordinate system
        /// </summary>
        public Quaternion LocalRotation { get; set; }

        /// <summary>
        /// Position in relation to the optitrack coordinate system
        /// </summary>
        public Vector3 Position { get; set; }

        public RigidBody(string id) : base(id)
        {
            this.Type = BodyTypeEnum.RigidBody;
        }
    }
}