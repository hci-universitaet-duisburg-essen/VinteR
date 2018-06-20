using System.Numerics;
using Ninject;
using NUnit.Framework;
using VinteR.Tracking;
using VinteR.Transform;

namespace VinteR.Tests
{
    [TestFixture]
    public class TestTransformator
    {
        private ITransformator _transformator;

        [OneTimeSetUp]
        public void SetUp()
        {
            var ninjectKernel = new StandardKernel(new VinterNinjectTestModule());
            this._transformator = ninjectKernel.Get<ITransformator>();
        }

        [Test]
        public void TestGetGlobalPosition()
        {
            var localOrigin = new Vector3(-3, 10, 2);
            var localPosition = new Vector3(0, 0, 0);

            var expected = new Vector3(-3, 10, 2);
            var actual = _transformator.GetGlobalPosition(localOrigin, localPosition);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestGetGlobalPositionWithRotation()
        {
            var position = new Position()
            {
                Location = new Vector3(1, 1, 0),
                Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, 90f.ToRadians())
            };
            var localPosition = new Vector3(1, 1, 0);

            var expected = new Vector3(1, 2, -1);
            var actual = _transformator.GetGlobalPosition(position, localPosition).Round();
            Assert.AreEqual(expected, actual);
        }
    }
}
