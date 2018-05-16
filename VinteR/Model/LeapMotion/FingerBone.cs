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
        /// Type of the bone. <see cref="EFingerBoneType"/>
        /// </summary>
        public EFingerBoneType  Type { get; }

        public FingerBone(EFingerBoneType type)
        {
            this.Type = type;
        }
    }

    public enum EFingerBoneType
    {
        Metacarpal,
        Proximal,
        Intermediate,
        Distal
    }
}