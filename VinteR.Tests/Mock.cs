using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NatNetML;
using VinteR.Model.LeapMotion;
using VinteR.Tracking;

namespace VinteR.Tests
{
    public class Mock
    {
        internal static MarkerSetData MockMarkerSetData(string name, IReadOnlyList<Vector3> points)
        {
            var result = new MarkerSetData()
            {
                MarkerSetName = name,
                nMarkers = points.Count()
            };
            var markers = new Marker[points.Count];
            for (var i = 0; i < markers.Length; i++)
            {
                var vector3 = points[i];
                markers[i] = MockMarker(vector3.X, vector3.Y, vector3.Z);
            }

            result.Markers = markers;
            return result;
        }

        internal static Marker MockMarker(float x = 0, float y = 0, float z = 0)
        {
            return new Marker() { x = x, y = y, z = z };
        }

        internal static Position MockPosition(float x, float y, float z, Quaternion q)
        {
            return new Position()
            {
                Location = new Vector3(x, y, z),
                Rotation = q
            };
        }

        /// <summary>
        /// All angles must be given in degrees.
        /// <inheritdoc cref="Quaternion.CreateFromYawPitchRoll"/>
        /// </summary>
        /// <param name="yaw"></param>
        /// <param name="pitch"></param>
        /// <param name="roll"></param>
        /// <returns></returns>
        internal static Quaternion MockYawPitchRoll(float yaw, float pitch, float roll)
        {
            return Quaternion.CreateFromYawPitchRoll(yaw.ToRadians(), pitch.ToRadians(), roll.ToRadians());
        }

        internal static Quaternion MockAxisAngle(Vector3 axis, float angle)
        {
            return Quaternion.CreateFromAxisAngle(axis, angle.ToRadians());
        }

        internal static Hand MockHand(Vector3 fingerBonePosition, bool isEndPosition = true)
        {
            var bone = isEndPosition
                ? new FingerBone(EFingerBoneType.Metacarpal) {LocalEndPosition = fingerBonePosition}
                : new FingerBone(EFingerBoneType.Metacarpal) {LocalStartPosition = fingerBonePosition};

            var hand = new Hand()
            {
                Fingers = new List<Finger>
                {
                    new Finger(EFingerType.Thumb)
                    {
                        Bones = new List<FingerBone> { bone }
                    }
                }
            };
            return hand;
        }
    }
}