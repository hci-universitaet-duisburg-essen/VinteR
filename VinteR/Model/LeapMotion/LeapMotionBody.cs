using System.Collections.Generic;
using System.Numerics;

namespace VinteR.Model.LeapMotion
{
    /// <inheritdoc />
    /// <summary>
    /// A leap motion body contains all data retrieved from a leap motion
    /// device. It basically consists of one or two hands.
    /// </summary>
    public class LeapMotionBody : Body
    {
        /// <summary>
        /// Contains all hands that where recognized by the leap motion
        /// </summary>
        public IList<Hand> Hands { get; set; }
    }
}