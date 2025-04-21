using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw.GX;
using HSDRaw.Tools;
using HSDRawViewer.Rendering.Models;
using System;
using System.Collections.Generic;

namespace HSDRawViewer.Rendering
{
    public class MatAnimJoint
    {
        public List<MatAnim> Nodes = new();
    }

    public class MatAnim
    {
        public List<MatAnimTexture> TextureAnims = new();

        public List<FOBJ_Player> Tracks = new();

        public float Frame = 0;

        public void SetFrame(float frame)
        {
            Frame = frame;
            foreach (MatAnimTexture v in TextureAnims)
                v.SetFrame(frame);
        }
    }

    public class MatAnimTexture
    {
        public GXTexMapID TextureID;

        public List<FOBJ_Player> Tracks = new();

        public List<HSD_TOBJ> Textures = new();

        public float Frame = 0;

        public void SetFrame(float frame)
        {
            Frame = frame;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public class MatAnimManager : AnimManager
    {
        public override int NodeCount => Nodes.Count;

        public List<MatAnimJoint> Nodes { get; internal set; } = new List<MatAnimJoint>();

        /// <summary>
        /// 
        /// </summary>
        public MatAnimManager()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="joint"></param>
        public MatAnimManager(HSD_MatAnimJoint joint)
        {
            FromMatAnim(joint);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mobj"></param>
        /// <returns></returns>
        public void GetMaterialState(HSD_MOBJ mobj, int jobj_index, int dobj_index, ref LiveMaterial state)
        {
            if (Nodes.Count > jobj_index && Nodes[jobj_index].Nodes.Count > dobj_index)
            {
                MatAnim node = Nodes[jobj_index].Nodes[dobj_index];

                state.ApplyAnim(node.Tracks, node.Frame);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="boneIndex"></param>
        /// <param name="tobj"></param>
        public LiveTObj GetTextureAnimState(HSD_TOBJ tobj, int jobj_index, int dobj_index)
        {
            if (tobj == null)
                return null;

            LiveTObj m = new();
            m.Reset(tobj);

            if (Nodes.Count > jobj_index && Nodes[jobj_index].Nodes.Count > dobj_index)
            {
                MatAnim node = Nodes[jobj_index].Nodes[dobj_index];

                MatAnimTexture texAnim = node.TextureAnims.Find(e => e.TextureID == tobj.TexMapID);
                if (texAnim != null)
                    m.ApplyAnim(texAnim.Textures, texAnim.Tracks, texAnim.Frame);
            }

            return m;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="boneIndex"></param>
        /// <param name="tobj"></param>
        public LiveTObj GetTextureAnimState(GXTexMapID mapid, int jobj_index, int dobj_index, ref LiveTObj m)
        {
            if (Nodes.Count > jobj_index && Nodes[jobj_index].Nodes.Count > dobj_index)
            {
                MatAnim node = Nodes[jobj_index].Nodes[dobj_index];

                MatAnimTexture texAnim = node.TextureAnims.Find(e => e.TextureID == mapid);
                if (texAnim != null)
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

            foreach (HSD_MatAnimJoint j in joint.TreeList)
            {
                MatAnimJoint matjoint = new();
                if (j.MaterialAnimation != null)
                    foreach (HSD_MatAnim a in j.MaterialAnimation.List)
                    {
                        MatAnim anm = new();

                        if (a.AnimationObject != null)
                        {
                            FrameCount = (int)Math.Max(FrameCount, a.AnimationObject.EndFrame);

                            foreach (HSD_FOBJDesc fdesc in a.AnimationObject.FObjDesc.List)
                                anm.Tracks.Add(new FOBJ_Player(fdesc));
                        }

                        if (a.TextureAnimation != null)
                            foreach (HSD_TexAnim t in a.TextureAnimation.List)
                            {
                                MatAnimTexture tex = new();

                                tex.Textures.AddRange(t.ToTOBJs());

                                tex.TextureID = t.GXTexMapID;

                                if (t.AnimationObject != null)
                                {
                                    if (t.AnimationObject != null)
                                    {
                                        FrameCount = (int)Math.Max(FrameCount, t.AnimationObject.EndFrame);

                                        foreach (HSD_FOBJDesc fdesc in t.AnimationObject.FObjDesc.List)
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
        /// <returns></returns>
        public HSD_MatAnimJoint ToMatAnim()
        {
            // TODO:
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frame"></param>
        public void SetAllFrames(float frame)
        {
            foreach (MatAnimJoint v in Nodes)
                foreach (MatAnim n in v.Nodes)
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
            foreach (MatAnimJoint v in Nodes)
            {
                foreach (MatAnim n in v.Nodes)
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
            MatAnim node = GetMatAnimAtIndex(mat_index);
            if (node != null)
                node.SetFrame(frame);
        }
    }
}
