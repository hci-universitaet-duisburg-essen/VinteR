using System.Linq;
using VinteR.Model;
using VinteR.Model.OptiTrack;

namespace VinteR.Datamerge
{
    public partial class DataMerger
    {
        public Body Merge(OptiTrackBody body)
        {
            Body result;
            switch (body)
            {
                case RigidBody _:
                    result = MergeRigidBody(body as RigidBody);
                    break;
                case Skeleton _:
                    result = MergeSkeleton(body as Skeleton);
                    break;
                default:
                    result = MergeDefault(body);
                    break;
            }

            FireBodyMerged(result);
            return result;
        }

        private static Body MergeRigidBody(RigidBody rigidBody)
        {
            var body = new Body
            {
                BodyType = Body.EBodyType.RigidBody,
                Points = rigidBody.Points,
                Rotation = rigidBody.LocalRotation
            };
            return body;
        }

        private static Body MergeSkeleton(Skeleton skeleton)
        {
            var points = skeleton.RigidBodies.SelectMany(rb => rb.Points).ToList();

            var body = new Body
            {
                BodyType = Body.EBodyType.Skeleton,
                Points = points,
                Rotation = skeleton.Rotation
            };
            return body;
        }

        private static Body MergeDefault(OptiTrackBody body)
        {
            var result = new Body
            {
                BodyType = Body.EBodyType.MarkerSet,
                Points = body.Points,
                Rotation = body.Rotation
            };
            if (result.Points?.Count == 1)
                result.BodyType = Body.EBodyType.Marker;
            return result;
        }
    }
}