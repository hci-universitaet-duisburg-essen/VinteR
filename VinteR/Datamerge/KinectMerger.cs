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

        private static readonly Quaternion KinectOptiTrackRotationAdjustment = Quaternion.CreateFromAxisAngle(Vector3.UnitY, -90f.ToRadians());
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
            
            // Logging Output to identify Hand Point
            foreach (Point point in body.Points )
            {
                // Logger.Debug("Kinect Body: Point: {0}, State: {1}", point.Name, point.State);
                if (point.Name == "HandRight" )
                {
                    Logger.Debug("HandRight, X: {0}, Y: {1}, Z: {2} ", point.Position.X, point.Position.Y, point.Position.Z);

                }
            }
            

            var kinectPosition = _adapterTracker.Locate(sourceId);

            Logger.Debug("OptiTrack KinectPos X: {0}", kinectPosition.Location.X );
            Logger.Debug("OptiTrack KinectPos Y: {0}", kinectPosition.Location.Y );
            Logger.Debug("OptiTrack KinectPos Z: {0}", kinectPosition.Location.Z );
            
            foreach (Point point in body.Points)
            {
                kinectPosition.Rotation = Quaternion.Multiply(kinectPosition.Rotation, KinectOptiTrackRotationAdjustment);
                var globalPosition = _transformator.GetGlobalPosition(kinectPosition, point.Position);
                var localPosition = point.Position;
                Point resultPoint = new Point(point.Position);
                resultPoint.Name = point.Name;
                resultPoint.State = point.State;
                result.Points.Add(resultPoint);
            }

            Logger.Debug("Result After Transform {0}", result);
            return result;
        }
    }
}
