using System;
using NLog;
using VinteR.Model;
using VinteR.Model.Kinect;

namespace VinteR.Datamerge
{
    public class KinectMerger : IDataMerger
    {
        private static readonly Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public MocapFrame HandleFrame(MocapFrame frame)
        {
            foreach (var body in frame.Bodies)
            {
                if (body is KinectBody kinectBody)
                {
                    Merge(kinectBody);
                }
                else
                {
                    Logger.Warn("Could not frame for {0,15} by type {1}", frame.SourceId, frame.AdapterType);
                }
            }
            return frame;
        }

        public Body Merge(KinectBody body)
        {
            //TODO complete kinect merge implementation
            var result = new Body { BodyType = Body.EBodyType.Skeleton};
            return result;
        }
    }
}
