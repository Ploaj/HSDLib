using System;
using System.Collections.Generic;
using HSDRaw.Common;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using HSDRaw.Common.Animation;

namespace HSDRawViewer.Rendering
{
    public enum RenderMode
    {
        Default,
        Normals,
        VertexColor,
        UV0,
        UV1,
        AmbientColor,
        DiffuseColor,
        SpecularColor,
        ExtColor,
        DiffusePass,
        SpecularPass,
    }
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

        public bool EnableDepth { get; set; } = true;

        public RenderMode RenderMode { get; set; } = RenderMode.Default;

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
        public void Render(Camera camera)
        {
            GL.PushAttrib(AttribMask.AllAttribBits);

            UpdateTransforms(RootJOBJ, cam: camera);

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Front);

            if (EnableDepth)
            {
                GL.Enable(EnableCap.DepthTest);
                GL.DepthFunc(DepthFunction.Lequal);
            }

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
                                DOBJManager.RenderDOBJShader(camera, dobj, b.Key, this);
                        }
                    }
                }

                // render xlu lookups last
                foreach(var xlu in XLU)
                {
                    DOBJManager.RenderDOBJShader(camera, xlu.Item1, xlu.Item2, this);
                }

                GL.Disable(EnableCap.DepthTest);

                if (DOBJManager.SelectedDOBJ != null && DOBJManager.OutlineSelected)
                {
                    DOBJManager.RenderDOBJShader(camera, DOBJManager.SelectedDOBJ, parent, this, true);
                }
            }

            GL.PopAttrib();


            GL.PushAttrib(AttribMask.AllAttribBits);

            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.DepthTest);

            if (RenderBones)
                foreach (var b in jobjToCache)
                {
                    RenderBone(b.Value, b.Key.Equals(SelectetedJOBJ));
                }

            GL.PopAttrib();
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
        private void UpdateTransforms(HSD_JOBJ root, JOBJCache parent = null, Camera cam = null)
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

            if(cam != null && 
                (root.Flags & (JOBJ_FLAG.BILLBOARD | JOBJ_FLAG.VBILLBOARD | JOBJ_FLAG.HBILLBOARD | JOBJ_FLAG.PBILLBOARD)) != 0)
            {
                var pos = Vector3.TransformPosition(Vector3.Zero, world);
                var campos = (cam.RotationMatrix * new Vector4(cam.Translation, 1)).Xyz;

                if(root.Flags.HasFlag(JOBJ_FLAG.BILLBOARD))
                    world = Matrix4.LookAt(pos, campos, Vector3.UnitY).Inverted();

                if (root.Flags.HasFlag(JOBJ_FLAG.VBILLBOARD))
                {
                    var temp = pos.Y;
                    pos.Y = 0;
                    campos.Y = 0;
                    world = Matrix4.LookAt(pos, campos, Vector3.UnitY).Inverted() * Matrix4.CreateTranslation(0, temp, 0);
                }

                if (root.Flags.HasFlag(JOBJ_FLAG.HBILLBOARD))
                {
                    var temp = pos.X;
                    pos.X = 0;
                    campos.X = 0;
                    world = Matrix4.LookAt(pos, campos, Vector3.UnitY).Inverted() * Matrix4.CreateTranslation(temp, 0, 0);
                }

                if (root.Flags.HasFlag(JOBJ_FLAG.RBILLBOARD))
                {
                    var temp = pos.Z;
                    pos.Z = 0;
                    campos.Z = 0;
                    world = Matrix4.LookAt(pos, campos, Vector3.UnitY).Inverted() * Matrix4.CreateTranslation(0, 0, temp);
                }
            }

            if (!jobjToCache.ContainsKey(root))
            {
                var jcache = new JOBJCache()
                {
                    Parent = parent,
                    Index = jobjToCache.Count,
                    InvertedTransform = world.Inverted()
                };
                if (root.Flags.HasFlag(JOBJ_FLAG.SKELETON) && root.InverseWorldTransform != null)
                {
                    jcache.InvertedTransform = HSDMatrixToTKMatrix(root.InverseWorldTransform);
                }
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
                UpdateTransforms(child, jobjToCache[root], cam);
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

            if (animatedBoneIndex != -1 && Animation != null && animatedBoneIndex < Animation.Nodes.Count)
            {
                AnimNode node = Animation.Nodes[animatedBoneIndex];
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

        public AnimManager Animation = new AnimManager();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="joint"></param>
        public void SetAnimJoint(HSD_AnimJoint joint)
        {
            Animation = new AnimManager();
            Animation.FromAnimJoint(joint);
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetFigaTree(HSD_FigaTree tree)
        {
            Animation = new AnimManager();
            Animation.FromFigaTree(tree);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Z"></param>
        /// <returns></returns>
        private static Matrix4 HSDMatrixToTKMatrix(HSD_Matrix4x3 mat)
        {
            return new Matrix4(
                mat.M11, mat.M21, mat.M31, 0,
                mat.M12, mat.M22, mat.M32, 0,
                mat.M13, mat.M23, mat.M33, 0,
                mat.M14, mat.M24, mat.M34, 1);
        }

        #endregion
    }
}
