using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using NUnit.Framework;
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
            this._transformator = new Transformator();
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
        public void TestGetGlobalPositionWithLocalRotation()
        {
            var localOrigin = new Vector3(1, 1, 1);
            var localPosition = new Vector3(0, 0, 0);

            var expected = new Vector3(1, 1, 1);
            var rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, ToRadians(90));
            var actual = _transformator.GetGlobalPosition(localOrigin, localPosition, rotation);
            actual = Round(actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestGetGlobalPositionWithLocalRotation2()
        {
            var localOrigin = new Vector3(1, 0, 1);
            var localPosition = new Vector3(1, 0, 1);

            var expected = new Vector3(0, 0, 2);
            var rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, ToRadians(-90));
            var actual = _transformator.GetGlobalPosition(localOrigin, localPosition, rotation);
            actual = Round(actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestGetGlobalPositionWithLocalRotation3()
        {
            var localOrigin = new Vector3(1, 0, 1);
            var localPosition = new Vector3(1, 0, 1);

            var expected = new Vector3(-2, 0, 0);

            var localRotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, ToRadians(-90));
            var localOriginRotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, ToRadians(-90));
            var actual = _transformator.GetGlobalPosition(localOrigin, localPosition, localRotation, localOriginRotation);
            actual = Round(actual);
            Assert.AreEqual(expected, actual);
        }

        private static Vector3 Round(Vector3 v)
        {
            var roundFloat = new Func<float, float>((float f) => (float) Math.Round(f, 3));
            return new Vector3(roundFloat(v.X), roundFloat(v.Y), roundFloat(v.Z));
        }

        private static float ToRadians(float val)
        {
            return ((float)Math.PI / 180) * val;
        }
    }
}
