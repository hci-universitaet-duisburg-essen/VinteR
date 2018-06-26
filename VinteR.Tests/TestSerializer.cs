using System;
using System.Collections.Generic;
using System.Numerics;
using Google.Protobuf;
using Ninject;
using NUnit.Framework;
using VinteR.Model;
using VinteR.Serialization;

namespace VinteR.Tests
{
    [TestFixture]
    public class TestSerializer
    {
        private ISerializer _serializer;

        [OneTimeSetUp]
        public void OnSetupFixture()
        {
            var ninjectKernel = new StandardKernel(new VinterNinjectTestModule());
            _serializer = ninjectKernel.Get<ISerializer>();
        }

        [Test]
        public void TestInvalid()
        {
            try
            {
                _serializer.ToProtoBuf(new MocapFrame(null, null), out var protoFrame);
            }
            catch (Exception e)
            {
                Assert.IsInstanceOf<ArgumentNullException>(e);
            }
        }

        [Test]
        public void TestValid()
        {
            _serializer.ToProtoBuf(new MocapFrame("root", "optitrack")
            {
                ElapsedMillis = 10,
                Latency = 10.3f,
                Gesture = ""
            }, out var protoFrame);
            var parsedFrame = Model.Gen.MocapFrame.Parser.ParseFrom(protoFrame.ToByteArray());

            Assert.AreEqual(10, parsedFrame.ElapsedMillis);
            Assert.AreEqual(10.3f, parsedFrame.Latency);
            Assert.AreEqual(string.Empty, parsedFrame.Gesture);
        }

        [Test]
        public void TestValidComplete()
        {
            _serializer.ToProtoBuf(new MocapFrame("root", "optitrack")
            {
                ElapsedMillis = 30,
                Latency = 5.3f,
                Gesture = "",
                Bodies = new List<Body>()
                {
                    new Body()
                    {
                        BodyType = Body.EBodyType.Marker,
                        Rotation = Quaternion.Identity,
                        Centroid = Vector3.One,
                        Points = new List<Point>()
                        {
                            new Point(-3.2f, 4.0f, 5.657f)
                            {
                                Name = "P0",
                                State = "tracked"
                            }
                        },
                        Name = "dynamite"
                    }
                }
            }, out var protoFrame);

            var parsedFrame = Model.Gen.MocapFrame.Parser.ParseFrom(protoFrame.ToByteArray());

            Assert.AreEqual(30, parsedFrame.ElapsedMillis);
            Assert.AreEqual(5.3f, parsedFrame.Latency);
            Assert.AreEqual(string.Empty, parsedFrame.Gesture);

            var parsedFrameBody = parsedFrame.Bodies[0];
            Assert.AreEqual(Model.Gen.MocapFrame.Types.Body.Types.EBodyType.Marker, parsedFrameBody.BodyType);
            Assert.AreEqual(new Model.Gen.MocapFrame.Types.Body.Types.Quaternion() {X = 0, Y = 0, Z = 0, W = 1},
                parsedFrameBody.Rotation);
            Assert.AreEqual(new Model.Gen.MocapFrame.Types.Body.Types.Vector3() {X = 1, Y = 1, Z = 1},
                parsedFrameBody.Centroid);
            Assert.AreEqual("dynamite", parsedFrameBody.Name);

            var point = parsedFrameBody.Points[0];
            Assert.AreEqual(new Model.Gen.MocapFrame.Types.Body.Types.Vector3() {X = -3.2f, Y = 4f, Z = 5.657f},
                point.Position);
            Assert.AreEqual("P0", point.Name);
            Assert.AreEqual("tracked", point.State);
        }
    }
}