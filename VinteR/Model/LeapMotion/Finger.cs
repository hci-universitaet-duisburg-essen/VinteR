using System.Collections.Generic;

namespace VinteR.Model.LeapMotion
{
    /// <summary>
    /// A finger represents one finger of a hand with a specific type
    /// and a set of bones.
    /// </summary>
    public class Finger
    {
        /// <summary>
        /// Type of the finger, for example Thumb.
        /// </summary>
        public EFingerType Type { get; }

        /// <summary>
        /// All recognized bones by the leap motion
        /// </summary>
        public IList<FingerBone> Bones { get; set; }

        public Finger(EFingerType type)
        {
            this.Type = type;
        }
    }

    public enum EFingerType
    {
        Thumb,
        Index,
        Middle,
        Ring,
        Pinky
    }
}