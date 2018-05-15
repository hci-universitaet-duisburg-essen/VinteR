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
        public FingerType Type { get; }

        /// <summary>
        /// All recognized bones by the leap motion
        /// </summary>
        public IList<FingerBone> Bones { get; set; }

        public Finger(FingerType type)
        {
            this.Type = type;
        }
    }

    public enum FingerType
    {
        Thumb,
        Index,
        Middle,
        Ring,
        Pinky
    }
}