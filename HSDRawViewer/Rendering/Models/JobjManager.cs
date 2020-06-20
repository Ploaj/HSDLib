using System;
using System.Collections.Generic;
using HSDRaw.Common;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using HSDRaw.Common.Animation;
using HSDRawViewer.Converters;
using System.Drawing;

namespace HSDRawViewer.Rendering
{
    public enum RenderMode
    {
        Default,
        Normals,
        Tangents, 
        BiNormal,
        VertexColor,
        UV0,
        UV1,
        UV2,
        UV3,
        TEX0,
        TEX1,
        TEX2,
        TEX3,
        AmbientColor,
        DiffuseColor,
        SpecularColor,
        ExtColor,
        DiffusePass,
        SpecularPass,
        BoneWeight,
    }

    /// <summary>
    /// 
    /// </summary>
    public class JOBJManagerSettings
    {
        public bool RenderBones { get; set; } = true;

        public bool RenderObjects { get; set; } = true;

        //public bool RenderMaterials { get; set; } = true;

        public bool RenderOrientation { get; set; } = false;

        public bool UseCameraLight { get; set; } = true;

        public float LightX { get; set; } = 0;

        public float LightY { get; set; } = 10;

        public float LightZ { get; set; } = 50;

        public float AmbientPower { get; set; } = 0.5f;
        public Color AmbientColor { get; set; } = Color.White;

        public float DiffusePower { get; set; } = 1;
        public Color DiffuseColor { get; set; } = Color.White;
    }

    /// <summary>
    /// 
    /// </summary>
    public class JOBJManager
    {
        public JOBJManagerSettings settings = new JOBJManagerSettings();

        public HSD_JOBJ SelectetedJOBJ = null;

        public float Frame { get; set; }

        private HSD_JOBJ RootJOBJ { get; set; }

        public DOBJManager DOBJManager = new DOBJManager();

        public bool EnableDepth { get; set; } = true;

        public RenderMode RenderMode { get; set; } = RenderMode.Default;

        public int JointCount { get => jobjToCache.Count; }

        public bool RefreshRendering = false;

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

            public bool SkipWorldUpdate = false;

            public int Index;
        }

        private Dictionary<HSD_JOBJ, JOBJCache> jobjToCache = new Dictionary<HSD_JOBJ, JOBJCache>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="transform"></param>
        public void SetWorldTransform(int index, Matrix4 transform)
        {
            foreach (var v in jobjToCache)
                if (v.Value.Index == index)
                {
                    v.Value.SkipWorldUpdate = true;
                    v.Value.WorldTransform = transform;
                    UpdateNoRender();
                    v.Value.SkipWorldUpdate = false;
                    break;
                }
        }

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
            if (jobj == null)
                return -1;

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
        public void CleanupRendering()
        {
            ClearRenderingCache();
        }

        /// <summary>
        /// 
        /// </summary>
        private void ClearRenderingCache()
        {
            jobjToCache.Clear();
            DOBJManager.ClearRenderingCache();
            RefreshRendering = false;
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
        public void RecalculateInverseBinds()
        {
            var anim = Animation;
            Animation = null;
            UpdateNoRender();
            Animation = anim;
            foreach(var v in jobjToCache)
            {
                v.Key.InverseWorldTransform = TKMatixToHSDMatrix(v.Value.WorldTransform.Inverted());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Render(Camera camera, bool update = true)
        {
            if (RefreshRendering)
                ClearRenderingCache();

            GL.PushAttrib(AttribMask.AllAttribBits);

            if(update)
                UpdateTransforms(RootJOBJ, cam: camera);

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Front);

            if (EnableDepth)
            {
                GL.Enable(EnableCap.DepthTest);
                GL.DepthFunc(DepthFunction.Lequal);
            }

            // Render DOBJS
            if (settings.RenderObjects)
            {
                HSD_JOBJ parent = null;
                List<Tuple<HSD_DOBJ, HSD_JOBJ, int, int>> XLU = new List<Tuple<HSD_DOBJ, HSD_JOBJ, int, int>>();
                MatAnimation.Frame = Frame;
                MatAnimation.JOBJIndex = 0;
                foreach (var b in jobjToCache)
                {
                    MatAnimation.DOBJIndex = 0;
                    if (b.Key.Dobj != null)
                    {
                        foreach (var dobj in b.Key.Dobj.List)
                        {
                            if (dobj == DOBJManager.SelectedDOBJ)
                                parent = b.Key;

                            if (dobj.Mobj.RenderFlags.HasFlag(RENDER_MODE.XLU))
                                XLU.Add(new Tuple<HSD_DOBJ, HSD_JOBJ, int, int>(dobj, b.Key, MatAnimation.JOBJIndex, MatAnimation.DOBJIndex));
                            else
                                DOBJManager.RenderDOBJShader(camera, dobj, b.Key, this, MatAnimation);

                            MatAnimation.DOBJIndex++;
                        }
                    }
                    MatAnimation.JOBJIndex++;
                }

                // render xlu lookups last
                foreach(var xlu in XLU)
                {
                    MatAnimation.JOBJIndex = xlu.Item3;
                    MatAnimation.DOBJIndex = xlu.Item4;
                    DOBJManager.RenderDOBJShader(camera, xlu.Item1, xlu.Item2, this, MatAnimation);
                }

                GL.Disable(EnableCap.DepthTest);

                if (DOBJManager.SelectedDOBJ != null && DOBJManager.OutlineSelected)
                {
                    DOBJManager.RenderDOBJShader(camera, DOBJManager.SelectedDOBJ, parent, this, null, true);
                }
            }

            GL.PopAttrib();


            GL.PushAttrib(AttribMask.AllAttribBits);

            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.DepthTest);

            float mag = 0;

            if(settings.RenderOrientation)
                mag = Vector3.TransformPosition(new Vector3(1, 0, 0), camera.MvpMatrix.Inverted()).Length / 30;

            if (settings.RenderBones)
                foreach (var b in jobjToCache)
                {
                    RenderBone(mag, b.Value, b.Key.Equals(SelectetedJOBJ));
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
        private void RenderBone(float mag, JOBJCache jobj, bool selected)
        {
            Matrix4 transform = jobj.WorldTransform;

            if (jobj.Parent != null)
            {
                Matrix4 parentTransform = jobj.Parent.WorldTransform;

                var bonePosition = Vector3.TransformPosition(Vector3.Zero, transform);
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

            GL.PushMatrix();
            GL.MultMatrix(ref transform);

            GL.Begin(PrimitiveType.Points);
            GL.Vertex3(0, 0, 0);
            GL.End();

            if (settings.RenderOrientation)
            {
                GL.LineWidth(2.5f);

                GL.Begin(PrimitiveType.Lines);
                GL.Color3(1f, 0f, 0f);
                GL.Vertex3(0, 0, 0);
                GL.Vertex3(mag, 0, 0);
                GL.Color3(0f, 1f, 0f);
                GL.Vertex3(0, 0, 0);
                GL.Vertex3(0, mag, 0);
                GL.Color3(0f, 0f, 1f);
                GL.Vertex3(0, 0, 0);
                GL.Vertex3(0, 0, mag);
                GL.End();
            }

            GL.PopMatrix();
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
            if(!cache.SkipWorldUpdate)
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
            Matrix4 Transform = Matrix4.CreateScale(jobj.SX, jobj.SY, jobj.SZ) *
                Matrix4.CreateFromQuaternion(Math3D.FromEulerAngles(jobj.RZ, jobj.RY, jobj.RX)) *
                Matrix4.CreateTranslation(jobj.TX, jobj.TY, jobj.TZ);

            if (animatedBoneIndex != -1 && Animation != null)
            {
                Transform = Animation.GetAnimatedState(Frame, animatedBoneIndex, jobj);
                animatedBoneIndex++;
            }
            
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

        public JointAnimManager Animation = new JointAnimManager();
        public MatAnimManager MatAnimation = new MatAnimManager();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="joint"></param>
        public void SetMatAnimJoint(HSD_MatAnimJoint joint)
        {
            RefreshRendering = true;
            MatAnimation = new MatAnimManager();
            MatAnimation.FromMatAnim(joint);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="joint"></param>
        public void SetAnimJoint(HSD_AnimJoint joint)
        {
            Animation = new JointAnimManager();
            Animation.FromAnimJoint(joint);
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetFigaTree(HSD_FigaTree tree)
        {
            Animation = new JointAnimManager();
            Animation.FromFigaTree(tree);
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetMOT(short[] jointTable, MOT_FILE file)
        {
            Animation = new MotAnimManager();
            ((MotAnimManager)Animation).SetMOT(jointTable, file);
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
        public static Matrix4 HSDMatrixToTKMatrix(HSD_Matrix4x3 mat)
        {
            return new Matrix4(
                mat.M11, mat.M21, mat.M31, 0,
                mat.M12, mat.M22, mat.M32, 0,
                mat.M13, mat.M23, mat.M33, 0,
                mat.M14, mat.M24, mat.M34, 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mat"></param>
        /// <returns></returns>
        public static HSD_Matrix4x3 TKMatixToHSDMatrix(Matrix4 mat)
        {
            return new HSD_Matrix4x3()
            {
                M11 = mat.M11,
                M21 = mat.M12,
                M31 = mat.M13,
                M12 = mat.M21,
                M22 = mat.M22,
                M32 = mat.M23,
                M13 = mat.M31,
                M23 = mat.M32,
                M33 = mat.M33,
                M14 = mat.M41,
                M24 = mat.M42,
                M34 = mat.M43,
            };
        }

        #endregion
    }
}
