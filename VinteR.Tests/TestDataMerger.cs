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
using VinteR.Model.OptiTrack;
using VinteR.Tracking;

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
            _ninjectKernel.Rebind<IAdapterTracker>().To<DataMergerAdapterTracker>();
        }

        [Test]
        public void TestMergeKinectBody()
        {
            var merger = _ninjectKernel.Get<IDataMerger>(KinectAdapter.AdapterTypeName);
            var kinectBody = new KinectBody(new List<Point> {new Point(0f, 1000f, 1500f)}, Body.EBodyType.Skeleton);
            var frame = new MocapFrame("kinect", KinectAdapter.AdapterTypeName)
            {
                Bodies = new List<Body>() {kinectBody}
            };
            frame = merger.HandleFrame(frame);
            var position = frame.Bodies[0].Points[0].Position;
            position = position.Round();

            Assert.AreEqual(Body.EBodyType.Skeleton, frame.Bodies[0].BodyType);
            Assert.AreEqual(new Vector3(1000, 1080, 0), position);
        }

        [Test]
        public void TestMergeLeapMotionBody()
        {
            var merger = _ninjectKernel.Get<IDataMerger>(LeapMotionAdapter.AdapterTypeName);
            var frame = new MocapFrame("leapmotion", LeapMotionAdapter.AdapterTypeName)
            {
                Bodies = new List<Body> {Mock.MockHand(new Vector3(-20, 300, 0), false)}
            };
            frame = merger.HandleFrame(frame);
            var position = frame.Bodies[0].Points[0].Position;
            position = position.Round();

            Assert.AreEqual(Body.EBodyType.Hand, frame.Bodies[0].BodyType);
            Assert.AreEqual(new Vector3(1200, 1050, 480), position);
        }

        [Test]
        public void TestMergeOptiTrackBody()
        {
            var merger = _ninjectKernel.Get<IDataMerger>(OptiTrackAdapter.AdapterTypeName);
            var optiTrackBody = new OptiTrackBody("testbody")
            {
                Points = new List<Point>() {new Point(Vector3.One)},
                BodyType = Body.EBodyType.Marker
            };
            var frame = new MocapFrame("optitrack", OptiTrackAdapter.AdapterTypeName)
            {
                Bodies = new List<Body>() {optiTrackBody}
            };
            frame = merger.HandleFrame(frame);
            var position = frame.Bodies[0].Points[0].Position;

            Assert.AreEqual(Body.EBodyType.Marker, frame.Bodies[0].BodyType);
            Assert.AreEqual(Vector3.One, position);
        }
    }

    internal class DataMergerAdapterTracker : IAdapterTracker
    {
        public IDictionary<string, Position> Positions { get; set; }

        public DataMergerAdapterTracker()
        {
            Positions = new Dictionary<string, Position>
            {
                {"leapmotion", Mock.MockPosition(1200, 750, 500, Mock.MockYawPitchRoll(90f, 0f, 0f))},
                {"kinect", Mock.MockPosition(-500, 80, 0, Quaternion.Identity)}
            };
        }

        public Position Locate(string name)
        {
            Positions.TryGetValue(name, out var position);
            return position;
        }
    }
}