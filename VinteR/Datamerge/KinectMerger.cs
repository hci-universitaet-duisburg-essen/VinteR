using NLog;
using VinteR.Model;
using VinteR.Model.Kinect;
using VinteR.Tracking;
using VinteR.Transform;


namespace VinteR.Datamerge
{
    public class KinectMerger : IDataMerger
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IAdapterTracker _adapterTracker;
        private readonly ITransformator _transformator;

        public KinectMerger(IAdapterTracker adapterTracker, ITransformator transformator)
        {
            _adapterTracker = adapterTracker;
            _transformator = transformator;
        }

        public MocapFrame HandleFrame(MocapFrame frame)
        {
            foreach (var body in frame.Bodies)
            {
                if (body is KinectBody kinectBody)
                {
                    var mergedBody = Merge(kinectBody, frame.SourceId);
                    body.Load(mergedBody);
                }
                else
                {
                    Logger.Warn("Could not frame for {0,15} by type {1}", frame.SourceId, frame.AdapterType);
                }
            }
            return frame;
        }

        public Body Merge(KinectBody body, string sourceId)
        {
            var result = new Body { BodyType = Body.EBodyType.Skeleton};
            
            var kinectPosition = _adapterTracker.Locate(sourceId);

            Logger.Debug("OptiTrack KinectPos X: {0}", kinectPosition.Location.X );
            Logger.Debug("OptiTrack KinectPos Y: {0}", kinectPosition.Location.Y );
            Logger.Debug("OptiTrack KinectPos Z: {0}", kinectPosition.Location.Z );
            
            foreach (var point in body.Points)
            {
                var globalPosition = _transformator.GetGlobalPosition(kinectPosition, point.Position);
                var resultPoint = new Point(globalPosition)
                {
                    Name = point.Name,
                    State = point.State
                };
                result.Points.Add(resultPoint);
            }

            Logger.Debug("Result After Transform {0}", result);
            return result;
        }
    }
}
