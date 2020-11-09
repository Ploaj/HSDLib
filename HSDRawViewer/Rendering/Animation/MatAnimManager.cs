using HSDRaw;
using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw.GX;
using HSDRaw.Tools;
using OpenTK;
using System;
using System.Collections.Generic;

namespace HSDRawViewer.Rendering
{
    public class MatAnimJoint
    {
        public List<MatAnim> Nodes = new List<MatAnim>();
    }

    public class MatAnim
    {
        public List<MatAnimTexture> TextureAnims = new List<MatAnimTexture>();

        public List<FOBJ_Player> Tracks = new List<FOBJ_Player>();

        public float Frame = 0;

        public void SetFrame(float frame)
        {
            Frame = frame;
            foreach (var v in TextureAnims)
                v.SetFrame(frame);
        }
    }

    public class MatAnimTexture
    {
        public GXTexMapID TextureID;

        public List<FOBJ_Player> Tracks = new List<FOBJ_Player>();

        public List<HSD_TOBJ> Textures = new List<HSD_TOBJ>();

        public float Frame = 0;

        public void SetFrame(float frame)
        {
            Frame = frame;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class MatAnimTextureState
    {
        public HSD_TOBJ TOBJ { get; }

        public float Blending { get; }

        public Matrix4 Transform { get; }

        public MatAnimTextureState(HSD_TOBJ tOBJ, float blending, Matrix4 transform)
        {
            TOBJ = tOBJ;
            Blending = blending;
            Transform = transform;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class MatAnimManager : AnimManager
    {
        public override int NodeCount => Nodes.Count;

        public List<MatAnimJoint> Nodes { get; internal set; } = new List<MatAnimJoint>();

        public int JOBJIndex = 0;

        public int DOBJIndex = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mobj"></param>
        /// <returns></returns>
        public HSD_Material GetMaterialState(HSD_MOBJ mobj)
        {
            HSD_Material mat = HSDAccessor.DeepClone<HSD_Material>(mobj.Material);

            if (Nodes.Count > JOBJIndex && Nodes[JOBJIndex].Nodes.Count > DOBJIndex)
            {
                var node = Nodes[JOBJIndex].Nodes[DOBJIndex];

                foreach(var t in node.Tracks)
                {
                    switch ((MatTrackType)t.TrackType)
                    {
                        case MatTrackType.HSD_A_M_ALPHA: mat.Alpha = t.GetValue(node.Frame); break;
                        case MatTrackType.HSD_A_M_AMBIENT_R: mat.AMB_R = (byte)(t.GetValue(node.Frame) * 0xFF); break;
                        case MatTrackType.HSD_A_M_AMBIENT_G: mat.AMB_G = (byte)(t.GetValue(node.Frame) * 0xFF); break;
                        case MatTrackType.HSD_A_M_AMBIENT_B: mat.AMB_B = (byte)(t.GetValue(node.Frame) * 0xFF); break;
                        case MatTrackType.HSD_A_M_DIFFUSE_R: mat.DIF_R = (byte)(t.GetValue(node.Frame) * 0xFF); break;
                        case MatTrackType.HSD_A_M_DIFFUSE_G: mat.DIF_G = (byte)(t.GetValue(node.Frame) * 0xFF); break;
                        case MatTrackType.HSD_A_M_DIFFUSE_B: mat.DIF_B = (byte)(t.GetValue(node.Frame) * 0xFF); break;
                        case MatTrackType.HSD_A_M_SPECULAR_R: mat.SPC_R = (byte)(t.GetValue(node.Frame) * 0xFF); break;
                        case MatTrackType.HSD_A_M_SPECULAR_G: mat.SPC_G = (byte)(t.GetValue(node.Frame) * 0xFF); break;
                        case MatTrackType.HSD_A_M_SPECULAR_B: mat.SPC_B = (byte)(t.GetValue(node.Frame) * 0xFF); break;
                    }
                }
            }

            return mat;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="boneIndex"></param>
        /// <param name="tobj"></param>
        public MatAnimTextureState GetTextureAnimState(HSD_TOBJ tobj)
        {
            if (tobj == null)
                return null;

            var tex = tobj;
            var blending = tobj.Blending;
            var TX = tobj.TX;
            var TY = tobj.TY;
            var TZ = tobj.TZ;
            var RX = tobj.RX;
            var RY = tobj.RY;
            var RZ = tobj.RZ;
            var SX = tobj.SX;
            var SY = tobj.SY;
            var SZ = tobj.SZ;

            if (Nodes.Count > JOBJIndex && Nodes[JOBJIndex].Nodes.Count > DOBJIndex)
            {
                var node = Nodes[JOBJIndex].Nodes[DOBJIndex];

                var texAnim = node.TextureAnims.Find(e=>e.TextureID == tobj.TexMapID);
                if(texAnim != null)
                {
                    foreach(var t in texAnim.Tracks)
                    {
                        //TODO: TEV as usual
                        switch ((TexTrackType)t.TrackType)
                        {
                            case TexTrackType.HSD_A_T_TIMG:
                                tex = texAnim.Textures[(int)t.GetValue(texAnim.Frame)];
                                break;
                            case TexTrackType.HSD_A_T_BLEND:
                            case TexTrackType.HSD_A_T_TS_BLEND:
                                blending = t.GetValue(texAnim.Frame);
                                break;
                            case TexTrackType.HSD_A_T_TRAU: TX = t.GetValue(texAnim.Frame); break;
                            case TexTrackType.HSD_A_T_TRAV: TY = t.GetValue(texAnim.Frame); break;
                            case TexTrackType.HSD_A_T_SCAU: SX = t.GetValue(texAnim.Frame); break;
                            case TexTrackType.HSD_A_T_SCAV: SY = t.GetValue(texAnim.Frame); break;
                            case TexTrackType.HSD_A_T_ROTX: RX = t.GetValue(texAnim.Frame); break;
                            case TexTrackType.HSD_A_T_ROTY: RY = t.GetValue(texAnim.Frame); break;
                            case TexTrackType.HSD_A_T_ROTZ: RZ = t.GetValue(texAnim.Frame); break;
                        }
                    }
                }
            }

            var transform = Matrix4.CreateScale(SX, SY, SZ) *
                Matrix4.CreateFromQuaternion(Math3D.FromEulerAngles(RZ, RY, RX)) *
                Matrix4.CreateTranslation(TX, TY, TZ);

            if(SX != 0 && SY != 0 && SZ != 0)
                transform.Invert();

            return new MatAnimTextureState(tex, blending, transform);
        }

        /// <summary>
        /// 
        /// </summary>
        public void FromMatAnim(HSD_MatAnimJoint joint)
        {
            Nodes.Clear();
            FrameCount = 0;

            if (joint == null)
                return;

            foreach (var j in joint.BreathFirstList)
            {
                MatAnimJoint matjoint = new MatAnimJoint();
                if (j.MaterialAnimation != null)
                    foreach (var a in j.MaterialAnimation.List)
                    {
                        MatAnim anm = new MatAnim();

                        if (a.AnimationObject != null)
                        {
                            FrameCount = (int)Math.Max(FrameCount, a.AnimationObject.EndFrame);

                            foreach (var fdesc in a.AnimationObject.FObjDesc.List)
                                anm.Tracks.Add(new FOBJ_Player(fdesc));
                        }

                        if (a.TextureAnimation != null)
                            foreach (var t in a.TextureAnimation.List)
                            {
                                MatAnimTexture tex = new MatAnimTexture();

                                tex.Textures.AddRange(t.ToTOBJs());

                                tex.TextureID = t.GXTexMapID;

                                if (t.AnimationObject != null)
                                {
                                    if (t.AnimationObject != null)
                                    {
                                        FrameCount = (int)Math.Max(FrameCount, t.AnimationObject.EndFrame);

                                        foreach (var fdesc in t.AnimationObject.FObjDesc.List)
                                            tex.Tracks.Add(new FOBJ_Player(fdesc));
                                    }
                                }

                                anm.TextureAnims.Add(tex);
                            }

                        matjoint.Nodes.Add(anm);
                    }
                Nodes.Add(matjoint);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frame"></param>
        public void SetAllFrames(float frame)
        {
            foreach (var v in Nodes)
                foreach (var n in v.Nodes)
                    n.SetFrame(frame);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public MatAnim GetMatAnimAtIndex(int index)
        {
            int i = 0;
            foreach(var v in Nodes)
            {
                foreach(var n in v.Nodes)
                {
                    if (index == i)
                        return n;
                    i++;
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="frame"></param>
        public void SetFrame(int mat_index, float frame)
        {
            var node = GetMatAnimAtIndex(mat_index);
            if (node != null)
                node.SetFrame(frame);
        }
    }
}
