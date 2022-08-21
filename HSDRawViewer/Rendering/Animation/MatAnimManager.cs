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

        public Vector4 Konst;
        public Vector4 Tev0;
        public Vector4 Tev1;

        private float TX;
        private float TY;
        private float TZ;
        private float RX;
        private float RY;
        private float RZ;
        private float SX;
        private float SY;
        private float SZ;

        /// <summary>
        /// 
        /// </summary>
        public MatAnimTextureState()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mobj"></param>
        public void Reset(HSD_TOBJ t)
        {
            if (t == null)
                return;

            // initialize tobj data
            TOBJ = t;
            Blending = t.Blending;

            // transform
            TX = t.TX;
            TY = t.TY;
            TZ = t.TZ;
            SX = t.SX;
            SY = t.SY;
            SZ = t.SZ;
            RX = t.RX;
            RY = t.RY;
            RZ = t.RZ;

            // calculate transform
            Transform =
                Matrix4.CreateScale(SX, SY, SZ) *
                Math3D.CreateMatrix4FromEuler(RX, RY, RZ) *
                Matrix4.CreateTranslation(TX, TY, TZ);

            if (SX != 0 && SY != 0 && SZ != 0)
                Transform.Invert();

            Konst = Vector4.Zero;
            Tev0 = Vector4.Zero;
            Tev1 = Vector4.Zero;

            // initialize tev data
            if (t.TEV != null)
            {
                var k = t.TEV.constant;
                var t0 = t.TEV.tev0;
                var t1 = t.TEV.tev1;

                if (t.TEV.active.HasFlag(TOBJ_TEVREG_ACTIVE.KONST_R))
                    Konst.X = k.R / 255f;
                if (t.TEV.active.HasFlag(TOBJ_TEVREG_ACTIVE.KONST_G))
                    Konst.Y = k.G / 255f;
                if (t.TEV.active.HasFlag(TOBJ_TEVREG_ACTIVE.KONST_B))
                    Konst.Z = k.B / 255f;
                if (t.TEV.active.HasFlag(TOBJ_TEVREG_ACTIVE.KONST_A))
                    Konst.W = t.TEV.constantAlpha / 255f;

                if (t.TEV.active.HasFlag(TOBJ_TEVREG_ACTIVE.TEV0_R))
                    Tev0.X = t0.R / 255f;
                if (t.TEV.active.HasFlag(TOBJ_TEVREG_ACTIVE.TEV0_G))
                    Tev0.Y = t0.G / 255f;
                if (t.TEV.active.HasFlag(TOBJ_TEVREG_ACTIVE.TEV0_B))
                    Tev0.Z = t0.B / 255f;
                if (t.TEV.active.HasFlag(TOBJ_TEVREG_ACTIVE.TEV0_A))
                    Tev0.W = t.TEV.tev0Alpha / 255f;

                if (t.TEV.active.HasFlag(TOBJ_TEVREG_ACTIVE.TEV1_R))
                    Tev1.X = t1.R / 255f;
                if (t.TEV.active.HasFlag(TOBJ_TEVREG_ACTIVE.TEV1_G))
                    Tev1.Y = t1.G / 255f;
                if (t.TEV.active.HasFlag(TOBJ_TEVREG_ACTIVE.TEV1_B))
                    Tev1.Z = t1.B / 255f;
                if (t.TEV.active.HasFlag(TOBJ_TEVREG_ACTIVE.TEV1_A))
                    Tev1.W = t.TEV.tev1Alpha / 255f;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tracks"></param>
        /// <param name="frame"></param>
        public void ApplyAnim(List<HSD_TOBJ> textures, List<FOBJ_Player> tracks, float frame)
        {
            foreach (var t in tracks)
            {
                var value = t.GetValue(frame);

                switch ((TexTrackType)t.TrackType)
                {
                    case TexTrackType.HSD_A_T_TIMG:
                        if (value < textures.Count)
                            TOBJ = textures[(int)value];
                        else if (textures.Count > 0)
                            TOBJ = textures[0];
                        else
                            TOBJ = null;
                        break;
                    case TexTrackType.HSD_A_T_BLEND:
                    case TexTrackType.HSD_A_T_TS_BLEND:
                        Blending = value;
                        break;
                    case TexTrackType.HSD_A_T_TRAU: TX = value; break;
                    case TexTrackType.HSD_A_T_TRAV: TY = value; break;
                    case TexTrackType.HSD_A_T_SCAU: SX = value; break;
                    case TexTrackType.HSD_A_T_SCAV: SY = value; break;
                    case TexTrackType.HSD_A_T_ROTX: RX = value; break;
                    case TexTrackType.HSD_A_T_ROTY: RY = value; break;
                    case TexTrackType.HSD_A_T_ROTZ: RZ = value; break;
                    case TexTrackType.HSD_A_T_KONST_R: Konst.X = value; break;
                    case TexTrackType.HSD_A_T_KONST_G: Konst.Y = value; break;
                    case TexTrackType.HSD_A_T_KONST_B: Konst.Z = value; break;
                    case TexTrackType.HSD_A_T_KONST_A: Konst.W = value; break;
                    case TexTrackType.HSD_A_T_TEV0_R: Tev0.X = value; break;
                    case TexTrackType.HSD_A_T_TEV0_G: Tev0.Y = value; break;
                    case TexTrackType.HSD_A_T_TEV0_B: Tev0.Z = value; break;
                    case TexTrackType.HSD_A_T_TEV0_A: Tev0.W = value; break;
                    case TexTrackType.HSD_A_T_TEV1_R: Tev1.X = value; break;
                    case TexTrackType.HSD_A_T_TEV1_G: Tev1.Y = value; break;
                    case TexTrackType.HSD_A_T_TEV1_B: Tev1.Z = value; break;
                    case TexTrackType.HSD_A_T_TEV1_A: Tev1.W = value; break;
                }
            }

            Transform =
                Matrix4.CreateScale(SX, SY, SZ) *
                Math3D.CreateMatrix4FromEuler(RX, RY, RZ) *
                Matrix4.CreateTranslation(TX, TY, TZ);

            if (SX != 0 && SY != 0 && SZ != 0)
                Transform.Invert();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class MatAnimMaterialState
    {
        public Vector4 Ambient;

        public Vector4 Diffuse;

        public Vector4 Specular;

        public float Shininess;

        public float Alpha;

        public float Ref0;

        public float Ref1;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mobj"></param>
        public void Reset(HSD_MOBJ mobj)
        {
            if (mobj == null || mobj.Material == null)
                return;

            var color = mobj.Material;

            Ambient.X = color.AMB_R / 255f;
            Ambient.Y = color.AMB_G / 255f;
            Ambient.Z = color.AMB_B / 255f;
            Ambient.W = color.AMB_A / 255f;

            Diffuse.X = color.DIF_R / 255f;
            Diffuse.Y = color.DIF_G / 255f;
            Diffuse.Z = color.DIF_B / 255f;
            Diffuse.W = color.DIF_A / 255f;

            Specular.X = color.SPC_R / 255f;
            Specular.Y = color.SPC_G / 255f;
            Specular.Z = color.SPC_B / 255f;
            Specular.W = color.SPC_A / 255f;

            Shininess = color.Shininess;
            Alpha = color.Alpha;

            var pp = mobj.PEDesc;
            if (pp != null)
            {
                Ref0 = pp.AlphaRef0 / 255f;
                Ref1 = pp.AlphaRef1 / 255f;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tracks"></param>
        /// <param name="frame"></param>
        public void ApplyAnim(List<FOBJ_Player> tracks, float frame)
        {
            foreach (var t in tracks)
            {
                switch ((MatTrackType)t.TrackType)
                {
                    case MatTrackType.HSD_A_M_PE_REF0: Ref0 = t.GetValue(frame); break;
                    case MatTrackType.HSD_A_M_PE_REF1: Ref1 = t.GetValue(frame); break;
                    case MatTrackType.HSD_A_M_ALPHA: Alpha = t.GetValue(frame); break;
                    case MatTrackType.HSD_A_M_AMBIENT_R: Ambient.X = t.GetValue(frame); break;
                    case MatTrackType.HSD_A_M_AMBIENT_G: Ambient.Y = t.GetValue(frame); break;
                    case MatTrackType.HSD_A_M_AMBIENT_B: Ambient.Z = t.GetValue(frame); break;
                    case MatTrackType.HSD_A_M_DIFFUSE_R: Diffuse.X = t.GetValue(frame); break;
                    case MatTrackType.HSD_A_M_DIFFUSE_G: Diffuse.Y = t.GetValue(frame); break;
                    case MatTrackType.HSD_A_M_DIFFUSE_B: Diffuse.Z = t.GetValue(frame); break;
                    case MatTrackType.HSD_A_M_SPECULAR_R: Specular.X = t.GetValue(frame); break;
                    case MatTrackType.HSD_A_M_SPECULAR_G: Specular.Y = t.GetValue(frame); break;
                    case MatTrackType.HSD_A_M_SPECULAR_B: Specular.Z = t.GetValue(frame); break;
                }
            }
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
        public void GetMaterialState(HSD_MOBJ mobj, ref MatAnimMaterialState state)
        {
            if (Nodes.Count > JOBJIndex && Nodes[JOBJIndex].Nodes.Count > DOBJIndex)
            {
                var node = Nodes[JOBJIndex].Nodes[DOBJIndex];

                state.ApplyAnim(node.Tracks, node.Frame);
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

            var m = new MatAnimTextureState();
            m.Reset(tobj);

            if (Nodes.Count > JOBJIndex && Nodes[JOBJIndex].Nodes.Count > DOBJIndex)
            {
                var node = Nodes[JOBJIndex].Nodes[DOBJIndex];

                var texAnim = node.TextureAnims.Find(e=>e.TextureID == tobj.TexMapID);
                if(texAnim != null)
                    m.ApplyAnim(texAnim.Textures, texAnim.Tracks, texAnim.Frame);
            }

            return m;
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
