using System.Numerics;

namespace VinteR.Model.LeapMotion
{
    /// <summary>
    /// A bone that is part of a finger. A bone is no single point
    /// but contains a start and a end position.
    /// </summary>
    public class FingerBone
    {
        /// <summary>
        /// Start position of the bone in relation to the leap
        /// motion coordinate system.
        /// </summary>
        public Vector3 LocalStartPosition { get; set; }

        /// <summary>
        /// End position of the bone in relation to the leap
        /// motion coordinate system.
        /// </summary>
        public Vector3 LocalEndPosition { get; set; }

        /// <summary>
        /// Type of the bone. <see cref="FingerBoneType"/>
        /// </summary>
        public FingerBoneType  Type { get; }

        public FingerBone(FingerBoneType type)
        {
            this.Type = type;
        }
    }

    public enum FingerBoneType
    {
        Metacarpal,
        Proximal,
        Intermediate,
        Distal
    }
}