using System;
using System.Collections.Generic;
using System.Numerics;
using Ninject;
using NUnit.Framework;
using VinteR.Adapter.Kinect;
using VinteR.Adapter.LeapMotion;
using VinteR.Adapter.OptiTrack;
using VinteR.Datamerge;
using VinteR.Model;
using VinteR.Model.Kinect;
using VinteR.Model.LeapMotion;
using VinteR.Model.OptiTrack;

namespace VinteR.Tests
{
    [TestFixture]
    public class TestDataMerger
    {
        private StandardKernel _ninjectKernel;

        [OneTimeSetUp]
        public void SetUpTestOnce()
        {
            _ninjectKernel = new StandardKernel(new VinterNinjectTestModule());
        }

        [Test]
        public void TestMergeKinectBody()
        {
            var merger = _ninjectKernel.Get<IDataMerger>(KinectAdapter.AdapterTypeName);
            var kinectBody = new KinectBody(new List<Point>(), Body.EBodyType.Skeleton);
            var frame = new MocapFrame("kinect", KinectAdapter.AdapterTypeName)
            {
                Bodies = new List<Body>() {kinectBody}
            };
            frame = merger.HandleFrame(frame);
            Assert.AreEqual(Body.EBodyType.Skeleton, frame.Bodies[0].BodyType);
        }

        [Test]
        public void TestMergeLeapMotionBody()
        {
            var merger = _ninjectKernel.Get<IDataMerger>(LeapMotionAdapter.AdapterTypeName);
            var hand = new Hand()
            {
                LocalRotation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, 90f.ToRadians()),
                Fingers = new List<Finger>()
                {
                    new Finger(EFingerType.Thumb)
                    {
                        Bones = new List<FingerBone>()
                        {
                            new FingerBone(EFingerBoneType.Metacarpal) {LocalStartPosition = new Vector3(0, 1, 0)}
                        }
                    }
                }
            };
            var frame = new MocapFrame("leapmotion", LeapMotionAdapter.AdapterTypeName)
            {
                Bodies = new List<Body>() {hand}
            };
            frame = merger.HandleFrame(frame);
            var position = frame.Bodies[0].Points[0].Position;
            position = position.Round();

            Assert.AreEqual(Body.EBodyType.Hand, frame.Bodies[0].BodyType);
            Assert.AreEqual(new Vector3(0, 1, 0), position);
        }

        [Test]
        public void TestMergeOptiTrackBody()
        {
            var merger = _ninjectKernel.Get<IDataMerger>(OptiTrackAdapter.AdapterTypeName);
            var optiTrackBody = new OptiTrackBody("testbody") {Points = new List<Point>() {new Point(Vector3.One)}};
            var frame = new MocapFrame("optitrack", OptiTrackAdapter.AdapterTypeName)
            {
                Bodies = new List<Body>() {optiTrackBody}
            };
            frame = merger.HandleFrame(frame);
            Assert.AreEqual(Body.EBodyType.Marker, frame.Bodies[0].BodyType);
        }
    }
}