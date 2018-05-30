using VinteR.Adapter;
using VinteR.Model;

namespace VinteR.Datamerge
{
    /// <summary>
    /// Delegate that is called after a body was merged. Each file that contains parts
    /// of the data merger must use the method <code>FireBodyMerged</code> to call
    /// delegates.
    /// </summary>
    /// <param name="body"></param>
    public delegate void BodyMergedEventHandler(Body body);

    /// <summary>
    /// A <code>DataMerger</code> is used to merge adapter specific objects (KinectBody,
    /// LeapMotionBody...) into the generalized model <code>Body</code> and position
    /// the results inside the global coordinate system. A partial class is used at
    /// this point to implement different types of merge. Maybe a interface based
    /// solution might be better.
    /// </summary>
    public partial class DataMerger
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public event BodyMergedEventHandler BodyMerged;

        /// <summary>
        /// Contains the adapter tracker that is used to position adapter
        /// specific objects in their local coordinate system into the global
        /// system.
        /// </summary>
        private readonly IAdapterTracker _adapterTracker;

        public DataMerger(IAdapterTracker adapterTracker)
        {
            this._adapterTracker = adapterTracker;
        }

        /// <summary>
        /// Publish the merged body, thus another application can go on with the merged body object 
        /// </summary>
        /// <param name="body"><code>Body</code> that was successful merged</param>
        public virtual void FireBodyMerged(Body body)
        {
            if (BodyMerged != null) // Check if there are subscribers to the event
            {
                BodyMerged(body);
            }
        }

        public void HandleFrame(MocapFrame frame)
        {
            // extract the bodies here and process
            switch (frame.SourceId)
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