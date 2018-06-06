using System.Numerics;
using Ninject;
using NUnit.Framework;
using VinteR.Adapter;
using VinteR.Tracking;

namespace VinteR.Tests
{
    [TestFixture]
    public class TestAdapterTracker
    {
        private IAdapterTracker _adapterTracker;

        [OneTimeSetUp]
        public void SetUpTestOnce()
        {
            var ninjectKernel = new StandardKernel(new VinterNinjectTestModule());
            ninjectKernel.Rebind<IAdapterTracker>().To<OptiTrackAdapterTracker>();

            _adapterTracker = ninjectKernel.Get<IAdapterTracker>();
        }

        [Test]
        public void TestLocateKinect()
        {
            var position = _adapterTracker.Locate("kinect");
            var expected = new Position()
            {
                Location = new Vector3(1, 0, 2),
                Rotation = Quaternion.Identity
            };
            Assert.AreEqual(expected, position);
        }

        [Test]
        public void TestLocateLeapMotion()
        {
            var position = _adapterTracker.Locate("leapmotion");
            var expected = new Position()
            {
                Location = new Vector3(4, 2, 2),
                Rotation = Quaternion.Identity
            };
            Assert.AreEqual(expected, position);
        }

        [Test]
        public void TestLocateNotFound()
        {
            var position = _adapterTracker.Locate("unknown");
            Assert.AreEqual(Position.Zero, position);
        }
    }
}