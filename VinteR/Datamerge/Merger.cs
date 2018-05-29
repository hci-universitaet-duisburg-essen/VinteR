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
    }
}
