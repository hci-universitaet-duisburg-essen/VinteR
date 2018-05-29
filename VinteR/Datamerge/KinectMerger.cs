using System;
using VinteR.Model;
using VinteR.Model.Kinect;

namespace VinteR.Datamerge
{
    public partial class DataMerger
    {
        public Body Merge(KinectBody body)
        {
            //TODO complete kinect merge implementation
            var result = new Body { BodyType = Body.EBodyType.Skeleton};
            return result;
        }
    }
}
