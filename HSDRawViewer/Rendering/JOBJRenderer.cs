using System;
using System.Collections.Generic;
using HSDRaw.Common;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using HSDRaw.Common.Animation;

namespace HSDRawViewer.Rendering
{
    /// <summary>
    /// 
    /// </summary>
    public class JOBJManager
    {
        #region Settings

        public bool RenderBones { get; set; } = true;

        public bool RenderObjects { get; set; } = true;

        public bool RenderMaterials { get; set; } = true;

        public HSD_JOBJ SelectetedJOBJ = null;

        #endregion

        public float Frame { get; set; }

        private HSD_JOBJ RootJOBJ { get; set; }

        public DOBJManager DOBJManager = new DOBJManager();

        public float ModelScale { get => _modelScale;
            set
            {
                _modelScale = value;
            }
        }
        private float _modelScale = 1f;

        // caches
        private class JOBJCache
        {
            public JOBJCache Parent;

            public Matrix4 LocalTransform;
            public Matrix4 WorldTransform;
            public Matrix4 InvertedTransform;
            public Matrix4 BindTransform;

            public int Index;
        }

        private Dictionary<HSD_JOBJ, JOBJCache> jobjToCache = new Dictionary<HSD_JOBJ, JOBJCache>();

        /// <summary>
        /// Gets the world transform from the bone index
        /// Can be slow
        /// </summary>
        /// <param name="jobj"></param>
        /// <returns></returns>
        public HSD_JOBJ GetJOBJ(int index)
        {
            foreach (var v in jobjToCache)
                if (v.Value.Index == index)
                    return v.Key;

            return null;
        }

        /// <summary>
        /// Gets the world transform from the bone index
        /// Can be slow
        /// </summary>
        /// <param name="jobj"></param>
        /// <returns></returns>
        public Matrix4 GetWorldTransform(int index)
        {
            foreach (var v in jobjToCache)
                if (v.Value.Index == index)
                    return v.Value.WorldTransform;

            return Matrix4.Identity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobj"></param>
        /// <returns></returns>
        public Matrix4 GetWorldTransform(HSD_JOBJ jobj)
        {
            if (jobjToCache.ContainsKey(jobj))
                return jobjToCache[jobj].WorldTransform;
            else
                return Matrix4.Identity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobj"></param>
        /// <returns></returns>
        public Matrix4 GetBindTransform(HSD_JOBJ jobj)
        {
            if (jobjToCache.ContainsKey(jobj))
                return jobjToCache[jobj].BindTransform;
            else
                return Matrix4.Identity;
        }

        private Matrix4[] ShaderMatricies = new Matrix4[200];

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobj"></param>
        /// <returns></returns>
        public Matrix4[] GetShaderMatrices()
        {
            int i = 0;
            foreach (var v in jobjToCache)
            {
                ShaderMatricies[i] = v.Value.WorldTransform;
                //ShaderMatricies[i + 200] = v.Value.InvertedTransform * v.Value.WorldTransform;
                i++;
                if (i > 200)
                    break;
            }

            return ShaderMatricies;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobj"></param>
        /// <returns></returns>
        public int IndexOf(HSD_JOBJ jobj)
        {
            if (jobjToCache.ContainsKey(jobj))
                return jobjToCache[jobj].Index;
            else
                return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Matrix4[] GetWorldTransforms()
        {
            Matrix4[] mat = new Matrix4[jobjToCache.Count];

            int i = 0;
            foreach(var v in jobjToCache)
                mat[i++] = v.Value.WorldTransform;

            return mat;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Matrix4[] GetBindTransforms()
        {
            Matrix4[] mat = new Matrix4[jobjToCache.Count];

            int i = 0;
            foreach (var v in jobjToCache)
                mat[i++] = v.Value.InvertedTransform * v.Value.WorldTransform;

            return mat;
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClearRenderingCache()
        {
            jobjToCache.Clear();
            DOBJManager.ClearRenderingCache();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        public void SetJOBJ(HSD_JOBJ root)
        {
            ClearRenderingCache();
            RootJOBJ = root;
            UpdateNoRender();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Render(Camera cam)
        {
            UpdateTransforms(RootJOBJ);

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Front);

            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);

            // Render DOBJS
            if (RenderObjects)
            {
                HSD_JOBJ parent = null;
                List<Tuple<HSD_DOBJ, HSD_JOBJ>> XLU = new List<Tuple<HSD_DOBJ, HSD_JOBJ>>();
                foreach (var b in jobjToCache)
                {
                    if (b.Key.Dobj != null)
                    {
                        foreach (var dobj in b.Key.Dobj.List)
                        {
                            if (dobj == DOBJManager.SelectedDOBJ)
                                parent = b.Key;

                            if (dobj.Mobj.RenderFlags.HasFlag(RENDER_MODE.XLU))
                                XLU.Add(new Tuple<HSD_DOBJ, HSD_JOBJ>(dobj, b.Key));
                            else
                                DOBJManager.RenderDOBJShader(cam, dobj, b.Key, this);
                        }
                    }
                }

                // render xlu lookups last
                foreach(var xlu in XLU)
                {
                    DOBJManager.RenderDOBJShader(cam, xlu.Item1, xlu.Item2, this);
                }

                GL.Disable(EnableCap.DepthTest);

                if (DOBJManager.SelectedDOBJ != null && DOBJManager.OutlineSelected)
                {
                    DOBJManager.RenderDOBJShader(cam, DOBJManager.SelectedDOBJ, parent, this, true);
                }
            }

            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.DepthTest);
            // Render Bones
            if (RenderBones)
                foreach (var b in jobjToCache)
                {
                    RenderBone(b.Value, b.Key.Equals(SelectetedJOBJ));
                }
        }

        /// <summary>
        /// 
        /// </summary>
        public void UpdateNoRender()
        {
            UpdateTransforms(RootJOBJ);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="parentTransform"></param>
        private void RenderBone(JOBJCache jobj, bool selected)
        {
            Matrix4 transform = jobj.WorldTransform;
            var bonePosition = Vector3.TransformPosition(Vector3.Zero, transform);

            if (jobj.Parent != null)
            {
                Matrix4 parentTransform = jobj.Parent.WorldTransform;

                var parentPosition = Vector3.TransformPosition(Vector3.Zero, parentTransform);

                GL.LineWidth(1f);
                GL.Begin(PrimitiveType.Lines);
                GL.Color3(0f, 1f, 0f);
                GL.Vertex3(parentPosition);
                GL.Color3(0f, 0f, 1f);
                GL.Vertex3(bonePosition);
                GL.End();
            }

            if (selected)
            {
                GL.Color3(1f, 1f, 0f);
                GL.PointSize(7f);
            }
            else
            {
                GL.Color3(1f, 0f, 0f);
                GL.PointSize(5f);
            }
            GL.Begin(PrimitiveType.Points);
            GL.Vertex3(bonePosition);
            GL.End();
        }

        /// <summary>
        /// Updates the transforms
        /// </summary>
        /// <param name="root"></param>
        private void UpdateTransforms(HSD_JOBJ root, JOBJCache parent = null)
        {
            if (root == null)
                return;

            var index = -1;
            if(jobjToCache.ContainsKey(root))
                index = jobjToCache[root].Index;

            var local = CreateLocalTransform(root, index);
            var world = local;
            if (parent != null)
                world = local * parent.WorldTransform;

            if (!jobjToCache.ContainsKey(root))
            {
                var jcache = new JOBJCache()
                {
                    Parent = parent,
                    Index = jobjToCache.Count,
                    InvertedTransform = world.Inverted()
                };
                jobjToCache.Add(root, jcache);
            }

            if (parent == null)
                world *= Matrix4.CreateScale(ModelScale);

            var cache = jobjToCache[root];

            cache.LocalTransform = local;
            cache.WorldTransform = world;
            cache.BindTransform = cache.InvertedTransform * world;

            foreach (var child in root.Children)
            {
                UpdateTransforms(child, jobjToCache[root]);
            }
        }

        /// <summary>
        /// Creates a Matrix4 from a HSD_JOBJ
        /// </summary>
        /// <param name="jobj"></param>
        /// <returns></returns>
        private Matrix4 CreateLocalTransform(HSD_JOBJ jobj, int animatedBoneIndex = -1)
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

            if (animatedBoneIndex != -1 && animatedBoneIndex < Nodes.Count)
            {
                AnimNode node = Nodes[animatedBoneIndex];
                foreach (AnimTrack t in node.Tracks)
                {
                    switch (t.TrackType)
                    {
                        case JointTrackType.HSD_A_J_ROTX: RX = t.GetValue(Frame); break;
                        case JointTrackType.HSD_A_J_ROTY: RY = t.GetValue(Frame); break;
                        case JointTrackType.HSD_A_J_ROTZ: RZ = t.GetValue(Frame); break;
                        case JointTrackType.HSD_A_J_TRAX: TX = t.GetValue(Frame); break;
                        case JointTrackType.HSD_A_J_TRAY: TY = t.GetValue(Frame); break;
                        case JointTrackType.HSD_A_J_TRAZ: TZ = t.GetValue(Frame); break;
                        case JointTrackType.HSD_A_J_SCAX: SX = t.GetValue(Frame); break;
                        case JointTrackType.HSD_A_J_SCAY: SY = t.GetValue(Frame); break;
                        case JointTrackType.HSD_A_J_SCAZ: SZ = t.GetValue(Frame); break;
                    }
                }
                animatedBoneIndex++;
            }

            Matrix4 Transform = Matrix4.CreateScale(SX, SY, SZ) *
                Matrix4.CreateFromQuaternion(Math3D.FromEulerAngles(RZ, RY, RX)) *
                Matrix4.CreateTranslation(TX, TY, TZ);

            return Transform;
        }

        /// <summary>
        /// Removes a JOBJ from the tree
        /// </summary>
        /// <param name="jobj"></param>
        public void RemoveJOBJ(HSD_JOBJ jobj)
        {
            // find parent or prev in list

            HSD_JOBJ next = null;
            HSD_JOBJ parent = null;

            foreach (var v in RootJOBJ.BreathFirstList)
            {
                if (v.Next == jobj)
                    next = v;
                if (v.Child == jobj)
                    parent = v;
            }

            if(next != null)
            {
                next.Next = jobj.Next;
            }
            if(parent != null)
            {
                parent.Child = jobj.Next;
            }

            ClearRenderingCache();
        }

        #region Animation Loader

        public List<AnimNode> Nodes = new List<AnimNode>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="joint"></param>
        public int SetAnimJoint(HSD_AnimJoint joint)
        {
            Nodes.Clear();
            int max = 0;
            if (joint == null)
                return 0;
            foreach (var j in joint.BreathFirstList)
            {
                AnimNode n = new AnimNode();
                if (j.AOBJ != null)
                {
                    max = (int)Math.Max(max, j.AOBJ.EndFrame);

                    foreach (var fdesc in j.AOBJ.FObjDesc.List)
                    {
                        AnimTrack track = new AnimTrack();
                        track.TrackType = fdesc.AnimationType;
                        track.Keys = fdesc.GetDecodedKeys();
                        n.Tracks.Add(track);
                    }
                }
                Nodes.Add(n);
            }
            return max;
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetFigaTree(HSD_FigaTree tree)
        {
            Nodes.Clear();
            if (tree == null)
                return;

            foreach (var tracks in tree.Nodes)
            {
                AnimNode n = new AnimNode();
                foreach (HSD_Track t in tracks.Tracks)
                {
                    AnimTrack track = new AnimTrack();
                    track.TrackType = t.FOBJ.JointTrackType;
                    track.Keys = t.GetKeys();
                    n.Tracks.Add(track);
                }
                Nodes.Add(n);
            }
        }

        /// <summary>
        /// Hides DOBJs with given indices
        /// </summary>
        /// <param name="DOBJIndices"></param>
        public void HideDOBJs(List<int> DOBJIndices)
        {
            var i = 0;
            foreach(var j in RootJOBJ.BreathFirstList)
            {
                if(j.Dobj != null)
                {
                    foreach(var dobj in j.Dobj.List)
                    {
                        if (DOBJIndices.Contains(i))
                            DOBJManager.HiddenDOBJs.Add(dobj);
                        i++;
                    }
                }
            }
        }

        #endregion
    }
}
