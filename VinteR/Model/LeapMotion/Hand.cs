using System.Collections.Generic;
using System.Numerics;

namespace VinteR.Model.LeapMotion
{
    /// <inheritdoc />
    /// <summary>
    /// A leap motion hand reprsents one hand that was detected
    /// by a Leap Motion device.
    /// </summary>
    public class Hand : Body
    {
        public Hand()
        {
            this.BodyType = EBodyType.Hand;
        }

        /// <summary>
        /// Rotation of the object in relation to the coordinate
        /// system of the Leap Motion
        /// </summary>
        public Quaternion LocalRotation { get; set; }

        /// <summary>
        /// Position of the object in relation to the coordinate
        /// system of the Leap Motion
        /// </summary>
        public Vector3 LocalPosition { get; set; }

        /// <summary>
        /// Contains all recognized fingers by the leap motion
        /// </summary>
        public IList<Finger> Fingers { get; set; }

        /// <summary>
        /// Left or right hand.
        /// </summary>
        public ESideType Side { get; set; }
    }
}