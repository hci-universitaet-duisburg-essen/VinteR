using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinteR.Model
{
    /// <summary>
    /// A motion capture frame contains data sent by one input adapter and
    /// contains all data regarding position and rotation of recognized
    /// objects.
    /// </summary>
    public class MocapFrame
    {

        // String for Timestamp (multiple Frames represent a Time Series)
        public string timestamp;

        /// <summary>
        /// Name of the input adapter that sends the frame
        /// </summary>
        public string SourceId { get; set; }

        /// <summary>
        /// There might be a gesture recognized through validation
        /// of previous frames. If the gesture is completely recorgnized
        /// this field contains the name of the gesture.
        /// </summary>
        public string Gesture { get; set; }

        private IList<Body> _bodies;

        /// <summary>
        /// Contains a list of bodies that the input adapter has
        /// detected.
        /// </summary>
        public IList<Body> Bodies
        {
            get => _bodies;
            set
            {
                if (value == null) _bodies.Clear();
                else _bodies = value;
            }
        }

        public MocapFrame(string sourceId)
        {
            this.Bodies = new List<Body>();
            this.SourceId = sourceId;
        }

        public void AddBody(ref Body body)
        {
            this.Bodies.Add(body);
        }
    }
}