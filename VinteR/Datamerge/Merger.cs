using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinteR.Model;
using VinteR.Model.Kinect;

namespace VinteR.Datamerge
{
    public delegate void BodyMergedEventHandler(Body body);
    public partial class DataMerger
    {
        public event BodyMergedEventHandler BodyMerged;

        // Publish the merged body, thus another application can go on with the merged body object
        public virtual void OnMergedBodyAvailable(Body body)
        {
            if (BodyMerged != null) // Check if there are subscribers to the event
            {
                BodyMerged(body);
            }
        }
        public void handleFrame(MocapFrame frame)
        {
            // extract the bodies here and process
            switch(frame.SourceId)
            {
                // Make a switch based on the sourceID e.g. Kinect, Leap and pass the data to the corresponding merge
                /*
                 * foreach(KinectBody body in frames.Bodies) {
                 *  //  Merge(body)
                 * }
                 */

            // We need the switch case, since we do not get specific bodies from mocapframe
            // and there is no generic Body Merge function implemented ...
            }
            
        }
    }
}
