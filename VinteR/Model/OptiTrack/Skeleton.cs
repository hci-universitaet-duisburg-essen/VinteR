using System.Collections.Generic;

namespace VinteR.Model.OptiTrack
{
    /// <inheritdoc />
    /// <summary>
    /// Describes a skeleton as defined by opti track.
    /// </summary>
    public class Skeleton : OptiTrackBody
    {
        /// <summary>
        /// Collection of rigid bodies this skeleton consists of.
        /// </summary>
        public IList<RigidBody> RigidBodies {
            get => _rigidBodies;
            set
            {
                if (value == null) _rigidBodies.Clear();
                else _rigidBodies = value;
            }
        }

        private IList<RigidBody> _rigidBodies;

        public Skeleton(string id) : base(id)
        {
            this._rigidBodies = new List<RigidBody>();
            this.Type = EBodyType.Skeleton;
        }
    }
}