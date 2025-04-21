using HSDRawViewer.Converters;
using HSDRawViewer.Rendering.Models;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace HSDRawViewer.Rendering
{
    /// <summary>
    /// 
    /// </summary>
    public class MotAnimManager : JointAnimManager
    {
        private MOT_FILE _motFile;
        private short[] _motJointTable;

        public override void ApplyAnimation(LiveJObj jobj, float frame)
        {
            foreach (LiveJObj v in jobj.Enumerate)
                ApplyAnimation0(v, frame);
        }

        private void ApplyAnimation0(LiveJObj jobj, float frame)
        {
            List<MOT_JOINT> joints = _motFile.Joints.FindAll(
                e => e.BoneID >= 0 &&
                e.BoneID < _motJointTable.Length &&
                _motJointTable[e.BoneID] == jobj.Index);

            foreach (MOT_JOINT j in joints)
            {
                MOT_KEY key = j.GetKey(frame / 60f);
                if (key == null)
                {
                    continue;
                }

                if (j.TrackFlag.HasFlag(MOT_FLAGS.TRANSLATE))
                {
                    jobj.Translation = new Vector3(jobj.Desc.TX, jobj.Desc.TY, jobj.Desc.TZ) + new Vector3(key.X, key.Y, key.Z);
                }
                if (j.TrackFlag.HasFlag(MOT_FLAGS.SCALE))
                {
                    jobj.Scale = new Vector3(jobj.Desc.SX, jobj.Desc.SY, jobj.Desc.SZ) + new Vector3(key.X, key.Y, key.Z);
                }
                if (j.TrackFlag.HasFlag(MOT_FLAGS.ROTATE))
                {
                    Vector3 dir = new(key.X, key.Y, key.Z);
                    float angle = key.W;

                    ///
                    //var local_2a4 = new Vector3(org_axis_vector.X, org_axis_vector.Y, org_axis_vector.Z);
                    //if (dynamic_params.MaxAngleChange < Vector3.CalculateAngle(axis_vector, local_2a4))
                    //{
                    //    var VStack688 = Vector3.Cross(local_2a4, axis_vector).Normalized();
                    //    local_2a4.RotateAboutUnitAxis(dynamic_params.MaxAngleChange, VStack688); // lbvector_RotateAboutUnitAxis(dynamic_params.MaxAngleChange, ref local_2a4, VStack688);
                    //    axis_vector = local_2a4;
                    //}

                    //var rot = new Vector3(jobj.Desc.RX, jobj.Desc.RY, jobj.Desc.RZ);
                    //// var ang = Vector3.CalculateAngle(rot, dir);

                    //rot.RotateAboutUnitAxis(angle * (float)Math.PI / 180, Vector3.Cross(Vector3.UnitX, dir));
                    //jobj.Rotation.Xyz = rot;


                    //var rotationOverride = Math3D.EulerToQuat(jobj.Desc.RX, jobj.Desc.RY, jobj.Desc.RZ);

                    //float rot_angle = (float)Math.Acos(Vector3.Dot(Vector3.UnitX, dir));
                    //if (Math.Abs(rot_angle) > 0.000001f)
                    //{
                    //    Vector3 rot_axis = Vector3.Cross(Vector3.UnitX, dir).Normalized();
                    //    rotationOverride *= Quaternion.FromAxisAngle(rot_axis, rot_angle);
                    //}

                    //{
                    //    var rotationOverride = Math3D.EulerToQuat(jobj.Desc.RX, jobj.Desc.RY, jobj.Desc.RZ).ToAxisAngle();
                    //    rotationOverride.X = key.X;
                    //    rotationOverride.Y = key.Y;
                    //    rotationOverride.Z = key.Z;
                    //    rotationOverride.W += key.W * (float)Math.PI / 180;

                    //    Matrix4.CreateFromAxisAngle(rotationOverride.Xyz, rotationOverride.W, out Matrix4 aa);
                    //    jobj.Rotation.Xyz = aa.ExtractRotationEuler();
                    //    Console.WriteLine(rotationOverride.ToAxisAngle().ToString());
                    //}

                    {
                        jobj.Rotation.Z = (float)Math.Atan2(key.Y, key.X);
                        double len = Math.Sqrt(key.Y * key.Y + key.X * key.X);
                        jobj.Rotation.Y = (float)Math.Atan2(-key.Z, len);
                        jobj.Rotation.X = key.W * (float)Math.PI / 180;
                    }

                    {
                        //var rotationOverride = Math3D.EulerToQuat(jobj.Desc.RX, jobj.Desc.RY, jobj.Desc.RZ);

                        //float rot_angle = (float)Math.Acos(Vector3.Dot(Vector3.UnitX, dir));
                        //if (Math.Abs(rot_angle) > 0.000001f)
                        //{
                        //    Vector3 rot_axis = Vector3.Cross(Vector3.UnitX, dir).Normalized();
                        //    rotationOverride *= Quaternion.FromAxisAngle(rot_axis, rot_angle);
                        //}

                        //rotationOverride *= Quaternion.FromEulerAngles(angle * (float)Math.PI / 180, 0, 0);
                        //jobj.Rotation.Xyz = Matrix4.CreateFromQuaternion(rotationOverride).ExtractRotationEuler();

                        //System.Diagnostics.Debug.WriteLine(jobj.Index);
                        //System.Diagnostics.Debug.WriteLine($"\t{Math3D.EulerToQuat(jobj.Desc.RX, jobj.Desc.RY, jobj.Desc.RZ).ToAxisAngle().ToString()}");
                        //System.Diagnostics.Debug.WriteLine($"\t{key.X} {key.Y} {key.Z} {key.W * (float)Math.PI / 180}");
                        //System.Diagnostics.Debug.WriteLine($"\t{Quaternion.FromAxisAngle(new Vector3(key.X, key.Y, key.Z), key.W * (float)Math.PI / 180)}");
                        //System.Diagnostics.Debug.WriteLine($"\t{rotationOverride.ToAxisAngle().ToString()}");
                    }
                }
            }
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
            foreach (MOT_JOINT j in _motFile.Joints)
            {
                MOT_KEY startKey = j.GetKey(startFrame / 60f);
                MOT_KEY endKey = j.GetKey(endFrame / 60f);
                List<MOT_KEY> middleKeys = j.Keys.FindAll(k => k.Time >= startFrame / 60f && k.Time <= endFrame / 60f);

                j.Keys = new List<MOT_KEY>();
                if (middleKeys.Count == 0)
                {
                    j.Keys.Add(startKey);
                    j.Keys.Add(endKey);
                }
                else
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
                foreach (MOT_KEY k in j.Keys)
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
