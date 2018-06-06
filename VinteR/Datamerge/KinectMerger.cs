using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NLog;
using VinteR.Model;
using VinteR.Model.Kinect;
using VinteR.Tracking;
using VinteR.Transform;

namespace VinteR.Datamerge
{
    public class KinectMerger : IDataMerger
    {
        private static readonly Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly IAdapterTracker _adapterTracker;
        private readonly ITransformator _transformator;

        public KinectMerger(IAdapterTracker adapterTracker, ITransformator transformator)
        {
            this._adapterTracker = adapterTracker;
            this._transformator = transformator;
        }

        public MocapFrame HandleFrame(MocapFrame frame)
        {
            foreach (var body in frame.Bodies)
            {
                if (body is KinectBody kinectBody)
                {
                    Merge(kinectBody, frame.SourceId);
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
            result.Points = body.Points
                .Select(point => _transformator.GetGlobalPosition(kinectPosition, point.Position))
                .Select(globalPosition => new Point(globalPosition))
                .ToList();

            return result;
        }
    }
}
