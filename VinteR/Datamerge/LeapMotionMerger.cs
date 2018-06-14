using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NLog;
using VinteR.Adapter;
using VinteR.Model;
using VinteR.Model.LeapMotion;
using VinteR.Tracking;
using VinteR.Transform;

namespace VinteR.Datamerge
{
    public class LeapMotionMerger : IDataMerger
    {
        private const string NameDivider = "_";
        private static readonly Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly IAdapterTracker _adapterTracker;
        private readonly ITransformator _transformator;

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
                    var mergedHand = Merge(hand, frame.SourceId);
                    body.Load(mergedHand);
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
            var result = new Body {BodyType = Body.EBodyType.Hand, Side = hand.Side };
            IList<Point> points = new List<Point>();
            var leapMotionPosition = _adapterTracker.Locate(sourceId);
//            Logger.Debug("Leap motion position: {0}", leapMotionPosition);

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
                                var boneGlobalStartPosition = _transformator.GetGlobalPosition(leapMotionPosition, bone.LocalStartPosition);
                                points.Add(CreatePoint(boneGlobalStartPosition, finger.Type, bone.Type));
                                //                                Logger.Debug(finger.Type.ToString() + " point: " + bone.LocalStartPosition.ToString());
                                //                                Logger.Debug(finger.Type.ToString() + " point: " + boneGlobalStartPosition);

                                if (finger.Type != EFingerType.Thumb) // thumb has zero length metacarpal bone, so do not add end point as well
                                {
                                    var boneGlobalEndPosition = _transformator.GetGlobalPosition(leapMotionPosition,
                                        bone.LocalEndPosition,
                                        hand.LocalRotation);
                                    points.Add(CreatePoint(boneGlobalEndPosition, finger.Type, bone.Type));
                                    //Logger.Info(finger.Type.ToString() + " point: " + bone.LocalEndPosition.ToString());
                                }
                            }
                            else // add all other bone end points
                            {
                                var boneGlobalEndPosition = _transformator.GetGlobalPosition(leapMotionPosition,
                                    bone.LocalEndPosition,
                                    hand.LocalRotation);
                                points.Add(CreatePoint(boneGlobalEndPosition, finger.Type, bone.Type));
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

        private static Point CreatePoint(Vector3 position, EFingerType fingerType, EFingerBoneType boneType)
        {
            return new Point(position)
            {
                Name = string.Join(NameDivider, fingerType.ToString(), boneType.ToString())
            };
        }
    }
}