using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw.GX;
using HSDRaw.Tools;
using OpenTK.Mathematics;
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
        public HSD_TOBJ TOBJ { get; set; }

        public float Blending { get; set; }

        public Matrix4 Transform { get; set; }

        public Vector4 Konst { get; set; }

        public MatAnimTextureState()
        {

        }

        public MatAnimTextureState(HSD_TOBJ tOBJ, float blending, Matrix4 transform, Vector4 konst)
        {
            TOBJ = tOBJ;
            Blending = blending;
            Transform = transform;
            Konst = konst;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public struct MatAnimMaterialState
    {
        public Vector4 Ambient;

        public Vector4 Diffuse;

        public Vector4 Specular;

        public float Shininess;

        public float Alpha;

        public float Ref0;

        public float Ref1;
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
        public void GetMaterialState(HSD_MOBJ mobj, ref MatAnimMaterialState state)
        {
            if (Nodes.Count > JOBJIndex && Nodes[JOBJIndex].Nodes.Count > DOBJIndex)
            {
                var node = Nodes[JOBJIndex].Nodes[DOBJIndex];

                foreach(var t in node.Tracks)
                {
                    switch ((MatTrackType)t.TrackType)
                    {
                        case MatTrackType.HSD_A_M_PE_REF0: state.Ref0 = t.GetValue(node.Frame); break;
                        case MatTrackType.HSD_A_M_PE_REF1: state.Ref1 = t.GetValue(node.Frame); break;
                        case MatTrackType.HSD_A_M_ALPHA: state.Alpha = t.GetValue(node.Frame); break;
                        case MatTrackType.HSD_A_M_AMBIENT_R: state.Ambient.X = t.GetValue(node.Frame); break;
                        case MatTrackType.HSD_A_M_AMBIENT_G: state.Ambient.Y = t.GetValue(node.Frame); break;
                        case MatTrackType.HSD_A_M_AMBIENT_B: state.Ambient.Z = t.GetValue(node.Frame); break;
                        case MatTrackType.HSD_A_M_DIFFUSE_R: state.Diffuse.X = t.GetValue(node.Frame); break;
                        case MatTrackType.HSD_A_M_DIFFUSE_G: state.Diffuse.Y = t.GetValue(node.Frame); break;
                        case MatTrackType.HSD_A_M_DIFFUSE_B: state.Diffuse.Z = t.GetValue(node.Frame); break;
                        case MatTrackType.HSD_A_M_SPECULAR_R: state.Specular.X = t.GetValue(node.Frame); break;
                        case MatTrackType.HSD_A_M_SPECULAR_G: state.Specular.Y = t.GetValue(node.Frame); break;
                        case MatTrackType.HSD_A_M_SPECULAR_B: state.Specular.Z = t.GetValue(node.Frame); break;
                    }
                }
            }
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
            Vector4 konst = Vector4.One;
            if (tobj.TEV != null)
            {
                konst.X = tobj.TEV.constant.R / 255f;
                konst.Y = tobj.TEV.constant.G / 255f;
                konst.Z = tobj.TEV.constant.B / 255f;
                konst.W = tobj.TEV.constantAlpha / 255f;
            }

            if (Nodes.Count > JOBJIndex && Nodes[JOBJIndex].Nodes.Count > DOBJIndex)
            {
                var node = Nodes[JOBJIndex].Nodes[DOBJIndex];

                var texAnim = node.TextureAnims.Find(e=>e.TextureID == tobj.TexMapID);
                if(texAnim != null)
                {
                    foreach(var t in texAnim.Tracks)
                    {
                        var value = t.GetValue(texAnim.Frame);

                        //TODO: TEV as usual
                        switch ((TexTrackType)t.TrackType)
                        {
                            case TexTrackType.HSD_A_T_TIMG:
                                tex = texAnim.Textures[(int)value];
                                break;
                            case TexTrackType.HSD_A_T_BLEND:
                            case TexTrackType.HSD_A_T_TS_BLEND:
                                blending = value;
                                break;
                            case TexTrackType.HSD_A_T_TRAU: TX = value; break;
                            case TexTrackType.HSD_A_T_TRAV: TY = value; break;
                            case TexTrackType.HSD_A_T_SCAU: SX = value; break;
                            case TexTrackType.HSD_A_T_SCAV: SY = value; break;
                            case TexTrackType.HSD_A_T_ROTX: RX = value; break;
                            case TexTrackType.HSD_A_T_ROTY: RY = value; break;
                            case TexTrackType.HSD_A_T_ROTZ: RZ = value; break;
                            case TexTrackType.HSD_A_T_KONST_R: konst.X = value; break;
                            case TexTrackType.HSD_A_T_KONST_G: konst.Y = value; break;
                            case TexTrackType.HSD_A_T_KONST_B: konst.Z = value; break;
                            case TexTrackType.HSD_A_T_KONST_A: konst.W = value; break;
                        }
                    }
                }
            }

            var transform = Matrix4.CreateScale(SX, SY, SZ) *
                Math3D.CreateMatrix4FromEuler(RX, RY, RZ) *
                Matrix4.CreateTranslation(TX, TY, TZ);

            if(SX != 0 && SY != 0 && SZ != 0)
                transform.Invert();

            return new MatAnimTextureState(tex, blending, transform, konst);
        }

        /// <summary>
        /// 
        /// </summary>
        public MatAnimManager FromMatAnim(HSD_MatAnimJoint joint)
        {
            Nodes.Clear();
            FrameCount = 0;

            if (joint == null)
                return this;

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

            return this;
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
