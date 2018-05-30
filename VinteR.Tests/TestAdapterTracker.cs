using System.Numerics;
using Ninject;
using NUnit.Framework;
using VinteR.Adapter;

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
            _adapterTracker = ninjectKernel.Get<IAdapterTracker>();
        }

        [Test]
        public void TestLocateKinect()
        {
            var position = _adapterTracker.Locate("kinect");
            var expected = new Vector3(1, 1, 1);
            Assert.AreEqual(expected, position);
        }

        [Test]
        public void TestLocateLeapMotion()
        {
            var position = _adapterTracker.Locate("leapmotion");
            var expected = new Vector3(2, 2, 2);
            Assert.AreEqual(expected, position);
        }

        [Test]
        public void TestLocateNotFound()
        {
            var position = _adapterTracker.Locate("unknown");
            Assert.IsNull(position);
        }
    }
}