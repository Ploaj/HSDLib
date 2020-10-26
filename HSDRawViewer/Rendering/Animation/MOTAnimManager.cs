using HSDRawViewer.Converters;
using System;
using HSDRaw.Common;
using OpenTK;
using System.Diagnostics;
using System.Collections.Generic;

namespace HSDRawViewer.Rendering
{
    /// <summary>
    /// 
    /// </summary>
    public class MotAnimManager : JointAnimManager
    {
        private MOT_FILE _motFile;
        private short[] _motJointTable;

        public override Matrix4 GetAnimatedMatrix(float frame, int boneIndex, HSD_JOBJ jobj)
        {
            float TX = jobj.TX;
            float TY = jobj.TY;
            float TZ = jobj.TZ;
            float RX = jobj.RX;
            float RY = jobj.RY;
            float RZ = jobj.RZ;
            float SX = jobj.SX;
            float SY = jobj.SY;
            float SZ = jobj.SZ;

            Quaternion rotationOverride = Math3D.FromEulerAngles(RZ, RY, RX);

            var joints = _motFile.Joints.FindAll(e => e.BoneID >= 0 && e.BoneID < _motJointTable.Length && _motJointTable[e.BoneID] == boneIndex);

            foreach (var j in joints)
            {
                var key = j.GetKey(frame / 60f);
                if (key == null)
                {
                    continue;
                }

                if (j.TrackFlag.HasFlag(MOT_FLAGS.TRANSLATE))
                {
                    TX += key.X;
                    TY += key.Y;
                    TZ += key.Z;
                }
                if (j.TrackFlag.HasFlag(MOT_FLAGS.SCALE))
                {
                    SX += key.X;
                    SY += key.Y;
                    SZ += key.Z;
                }
                if (j.TrackFlag.HasFlag(MOT_FLAGS.ROTATE))
                {
                    rotationOverride = Math3D.FromEulerAngles(RZ, RY, RX);

                    var dir = new Vector3(key.X, key.Y, key.Z);
                    var angle = key.W;

                    float rot_angle = (float)Math.Acos(Vector3.Dot(Vector3.UnitX, dir));
                    if (Math.Abs(rot_angle) > 0.000001f)
                    {
                        Vector3 rot_axis = Vector3.Cross(Vector3.UnitX, dir).Normalized();
                        rotationOverride *= Quaternion.FromAxisAngle(rot_axis, rot_angle);
                    }

                    rotationOverride *= Quaternion.FromEulerAngles(angle * (float)Math.PI / 180, 0, 0);
                }
            }

            return Matrix4.CreateScale(SX, SY, SZ) *
                Matrix4.CreateFromQuaternion(rotationOverride) *
                Matrix4.CreateTranslation(TX, TY, TZ);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jointTable"></param>
        /// <param name="file"></param>
        public void SetMOT(short[] jointTable, MOT_FILE file)
        {
            _motFile = file;
            _motJointTable = jointTable;
            FrameCount = (int)Math.Ceiling(_motFile.EndTime * 60);
        }

        /// <summary>
        /// Trims frames to given region
        /// </summary>
        /// <param name="startFrame"></param>
        /// <param name="endFrame"></param>
        public override void Trim(int startFrame, int endFrame)
        {
            FrameCount = endFrame - startFrame;
            _motFile.EndTime = 0f;
            foreach (var j in _motFile.Joints)
            {
                var startKey = j.GetKey(startFrame / 60f);
                var endKey = j.GetKey(endFrame / 60f);
                var middleKeys = j.Keys.FindAll(k => k.Time >= startFrame / 60f && k.Time <= endFrame / 60f);

                j.Keys = new List<MOT_KEY>();
                if (middleKeys.Count == 0)
                {
                    j.Keys.Add(startKey);
                    j.Keys.Add(endKey);
                } else
                {
                    if (middleKeys[0].Time - startKey.Time > 0.001)
                    {
                        j.Keys.Add(startKey);
                    }

                    j.Keys.AddRange(middleKeys);

                    if (endKey.Time - middleKeys[middleKeys.Count - 1].Time > 0.001)
                    {
                        j.Keys.Add(endKey);
                    }
                }

                Debug.Assert(j.Keys.Count > 0);
                foreach (var k in j.Keys)
                {
                    k.Time -= startFrame / 60f;
                }
                j.MaxTime = j.Keys[j.Keys.Count - 1].Time;
                _motFile.EndTime = Math.Max(_motFile.EndTime, j.MaxTime);
            }
        }

        public MOT_FILE GetMOT()
        {
            return _motFile;
        }

    }
}
