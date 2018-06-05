using System;
using System.Collections.Generic;
using System.Numerics;
using NLog;
using VinteR.Adapter;
using VinteR.Model;
using VinteR.Model.LeapMotion;
using VinteR.Transform;

namespace VinteR.Datamerge
{
    public class LeapMotionMerger : IDataMerger
    {
        private static readonly Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private IAdapterTracker _adapterTracker;
        private ITransformator _transformator;

        public LeapMotionMerger(IAdapterTracker adapterTracker, ITransformator transformator)
        {
            this._adapterTracker = adapterTracker;
            this._transformator = transformator;
        }

        public MocapFrame HandleFrame(MocapFrame frame)
        {
            foreach (var body in frame.Bodies)
            {
                if (body is Hand hand)
                {
                    Merge(hand, frame.SourceId);
                }
                else
                {
                    Logger.Warn("Could not frame for {0,15} by type {1}", frame.SourceId, frame.AdapterType);
                }
            }
            return frame;

        }

        /**
         * Method to convert hand into hand body consisting of joint points
         */
        private Body Merge(Hand hand, string sourceId)
        {
            var result = new Body { BodyType = Body.EBodyType.Hand };
            IList<Point> points = new List<Point>();
            Vector3? leapMotionPosition = _adapterTracker.Locate(sourceId);

            // Convert all joints to points (each position only once!)
            if (hand.Fingers != null)
            {
                foreach (Finger finger in hand.Fingers)
                {
                    //Logger.Info("Finger: " + finger.Type.ToString());
                    if (finger.Bones != null)
                    {
                        foreach (FingerBone bone in finger.Bones)
                        {
                            //Logger.Info("Finger Bone: " + bone.ToString());
                            if (bone.Type == EFingerBoneType.Metacarpal) // first bone in hand, needs start and end point added
                            {
                                //TODO rotation of the leap motion is missing
                                var boneGlobalStartPosition = _transformator.GetGlobalPosition(leapMotionPosition ?? Vector3.Zero, bone.LocalStartPosition,
                                    hand.LocalRotation);
                                points.Add(new Point(boneGlobalStartPosition));
                                //Logger.Info(finger.Type.ToString() + " point: " + bone.LocalStartPosition.ToString());

                                if (finger.Type != EFingerType.Thumb) // thumb has zero length metacarpal bone, so do not add end point as well
                                {
                                    var boneGlobalEndPosition = _transformator.GetGlobalPosition(leapMotionPosition ?? Vector3.Zero, 
                                        bone.LocalEndPosition,
                                        hand.LocalRotation);
                                    points.Add(new Point(boneGlobalEndPosition));
                                    //Logger.Info(finger.Type.ToString() + " point: " + bone.LocalEndPosition.ToString());
                                }
                            }
                            else // add all other bone end points
                            {
                                var boneGlobalEndPosition = _transformator.GetGlobalPosition(leapMotionPosition ?? Vector3.Zero,
                                    bone.LocalEndPosition,
                                    hand.LocalRotation);
                                points.Add(new Point(boneGlobalEndPosition));
                                //Logger.Info(finger.Type.ToString() + " point: " + bone.LocalEndPosition.ToString());
                            }
                        }
                    }
                }
            }

            //Logger.Info("Number of hand points (should be 24): " + points.Count);

            // alert that merged body is available
            result.Points = points;
            return result;
        }
    }
}
