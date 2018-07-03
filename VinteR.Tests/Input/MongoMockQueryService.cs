using System;
using System.Collections.Generic;
using System.Numerics;
using VinteR.Input;
using VinteR.Model;

namespace VinteR.Tests.Input
{
    public class MongoMockQueryService : IQueryService
    {
        public string GetStorageName()
        {
            return "MongoDB";
        }

        public IList<Session> GetSessions()
        {
            return new List<Session>()
            {
                new Session("testsession")
                {
                    Duration = 20,
                    Datetime = DateTime.MinValue.ToUniversalTime()
                }
            };
        }

        public Session GetSession(string name, uint startTimestamp = 0, int endTimestamp = -1)
        {
            return new Session("testsession")
            {
                Duration = 20,
                Datetime = DateTime.MinValue.ToUniversalTime(),
                MocapFrames = new List<MocapFrame>()
                {
                    new MocapFrame("optitrack", "optitrack", new List<Body>()
                    {
                        new Body()
                        {
                            Name = "dynamite",
                            BodyType = Body.EBodyType.RigidBody,
                            Centroid = Vector3.One,
                            Rotation = Quaternion.Identity
                        }
                    })
                }
            };
        }
    }
}