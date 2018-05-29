using System.Collections.Generic;
using NUnit.Framework;
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
        private DataMerger merger;

        [SetUp]
        public void SetUpTest()
        {
            this.merger = new DataMerger();
        }

        [Test]
        public void TestMergeKinectBody()
        {
            var optiTrackBody = new KinectBody(new List<Point>(), Body.EBodyType.Skeleton);
            var body = merger.Merge(optiTrackBody);
            Assert.AreEqual(Body.EBodyType.Skeleton, body.BodyType);
        }

        [Test]
        public void TestMergeLeapMotionBody()
        {
            var hand = new Hand();
            var body = merger.Merge(hand);
            Assert.AreEqual(Body.EBodyType.Hand, body.BodyType);
        }

        [Test]
        public void TestMergeOptiTrackBody()
        {
            var optiTrackBody = new OptiTrackBody("testbody");
            var body = merger.Merge(optiTrackBody);
            Assert.AreEqual(Body.EBodyType.MarkerSet, body.BodyType);
        }
    }
}