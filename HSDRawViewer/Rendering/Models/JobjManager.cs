using System;
using System.Collections.Generic;
using HSDRaw.Common;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using HSDRaw.Common.Animation;
using HSDRawViewer.Converters;
using HSDRawViewer.Rendering.Animation;
using HSDRawViewer.Rendering.GX;
using HSDRawViewer.Tools;
using System.Drawing;
using System.Linq;

namespace HSDRawViewer.Rendering.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class JOBJManager
    {
        public HSD_JOBJ SelectedJOBJ = null;

        public float Frame { get; set; }

        public bool EnableMaterialFrame { get; set; } = false;

        public bool EnableDepth { get; set; } = true;

        public bool RenderSplines { get; set; } = false;

        public bool EnableHiddenFlag { get; set; } = false;

        public float MaterialFrame { get; set; }

        private HSD_JOBJ RootJOBJ { get; set; }

        public DOBJManager DOBJManager = new DOBJManager();

        public RenderMode RenderMode { get => _gxShader.RenderMode; set => _gxShader.RenderMode = value; }

        private GXShader _gxShader = new GXShader();

        public JobjDisplaySettings _settings { get; internal set; } = new JobjDisplaySettings();

        public GXLightParam _lightParam { get; internal set; } = new GXLightParam();

        public GXFogParam _fogParam { get; internal set; } = new GXFogParam();

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
        public void SetWorldTransform(int index, Matrix4 transform, bool update = true)
        {
            foreach (var v in jobjToCache)
                if (v.Value.Index == index)
                {
                    v.Value.SkipWorldUpdate = true;
                    v.Value.WorldTransform = transform;
                    if (update)
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
        public int ParentIndex(HSD_JOBJ jobj)
        {
            if (jobj == null)
                return -1;

            if (jobjToCache.ContainsKey(jobj) && jobjToCache[jobj].Parent != null)
                return jobjToCache[jobj].Parent.Index;
            else
                return -1;
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
                v.Key.InverseWorldTransform = v.Value.WorldTransform.Inverted().ToHsdMatrix();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<HSD_DOBJ> GetDOBJs()
        {
            var list = new List<HSD_DOBJ>();

            foreach (var b in jobjToCache)
                if (b.Key.Dobj != null)
                    foreach (var d in b.Key.Dobj.List)
                        list.Add(d);

            return list;
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

            // prepare shader
            SetupShader();

            // render with shader
            _gxShader.Bind(camera, _lightParam, _fogParam);

            // Render DOBJS
            RenderDOBJs(camera);

            GL.PopAttrib();
            GL.UseProgram(0);

            // render splines
            DrawSplines(camera);

            // bone overlay
            RenderBoneOverlay();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool BranchIsVisible(HSD_JOBJ jobj, JOBJCache cache)
        {
            var parent = cache;
            var visible = !jobj.Flags.HasFlag(JOBJ_FLAG.HIDDEN);

            while (parent != null)
            {
                if (Animation != null && 
                    Animation.GetJointBranchState(Frame, parent.Index, out float branch))
                    return branch != 0;

                parent = parent.Parent;
            }

            if (EnableHiddenFlag)
                return visible;
            else
                return true;
        }

        /// <summary>
        /// 
        /// </summary>
        private void RenderDOBJs(Camera camera)
        {
            if (_settings.RenderObjects != ObjectRenderMode.None)
            {
                List<Tuple<HSD_DOBJ, HSD_JOBJ, int, int>> XLU = new List<Tuple<HSD_DOBJ, HSD_JOBJ, int, int>>();
                List<Tuple<HSD_DOBJ, HSD_JOBJ>> selected = new List<Tuple<HSD_DOBJ, HSD_JOBJ>>();

                if (!EnableMaterialFrame)
                    MatAnimation.SetAllFrames(Frame);
                MatAnimation.JOBJIndex = 0;
                ShapeAnimation.JOBJIndex = 0;
                ShapeAnimation.SetAllFrames(Frame);

                int dobjIndex = 0;

                foreach (var b in jobjToCache)
                {
                    MatAnimation.DOBJIndex = 0;
                    ShapeAnimation.DOBJIndex = 0;

                    if (!BranchIsVisible(b.Key, b.Value))
                    {
                        MatAnimation.JOBJIndex++;
                        ShapeAnimation.JOBJIndex++;
                        continue;
                    }

                    if (b.Key.Dobj != null)
                    {
                        foreach (var dobj in b.Key.Dobj.List)
                        {
                            // skip dobjs without mobjs
                            if (dobj.Mobj == null)
                                continue;

                            if (SelectedDOBJIndices.Contains(dobjIndex))
                                selected.Add(new Tuple<HSD_DOBJ, HSD_JOBJ>(dobj, b.Key));

                            // check if hidden
                            if (!HiddenDOBJIndices.Contains(dobjIndex) &&
                                !(_settings.RenderObjects == ObjectRenderMode.Selected && !SelectedDOBJIndices.Contains(dobjIndex)))
                            {
                                // get shape blend amt
                                DOBJManager.ShapeBlend = ShapeAnimation.GetBlending();

                                if (dobj.Mobj.RenderFlags.HasFlag(RENDER_MODE.XLU))
                                    XLU.Add(new Tuple<HSD_DOBJ, HSD_JOBJ, int, int>(dobj, b.Key, MatAnimation.JOBJIndex, MatAnimation.DOBJIndex));
                                else
                                    DOBJManager.RenderDOBJShader(GXShader._shader, dobj, b.Key, this, MatAnimation);
                            }

                            MatAnimation.DOBJIndex++;
                            ShapeAnimation.DOBJIndex++;
                            dobjIndex++;
                        }
                    }

                    MatAnimation.JOBJIndex++;
                    ShapeAnimation.JOBJIndex++;
                }

                //XLU.OrderBy(e => CameraDistance(camera, e.Item2));

                // render xlu lookups last
                foreach (var xlu in XLU)
                {
                    MatAnimation.JOBJIndex = xlu.Item3;
                    MatAnimation.DOBJIndex = xlu.Item4;

                    ShapeAnimation.JOBJIndex = xlu.Item3;
                    ShapeAnimation.DOBJIndex = xlu.Item4;

                    DOBJManager.ShapeBlend = ShapeAnimation.GetBlending();
                    DOBJManager.RenderDOBJShader(GXShader._shader, xlu.Item1, xlu.Item2, this, MatAnimation);
                }

                //GL.Disable(EnableCap.DepthTest);
                GL.DepthFunc(DepthFunction.Always);

                if (_settings.OutlineSelected)
                    foreach (var i in selected)
                    {
                        DOBJManager.RenderDOBJShader(GXShader._shader, i.Item1, i.Item2, this, null, true);
                    }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void DrawSplines(Camera camera)
        {
            foreach (var j in jobjToCache)
            {
                if (j.Key.Spline != null)
                    DrawShape.RenderSpline(j.Key.Spline, Color.Yellow, Color.Blue);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <param name="jobj"></param>
        /// <returns></returns>
        public float CameraDistance(Camera c, HSD_JOBJ jobj)
        {
            var jointPosition = Vector3.TransformPosition(Vector3.Zero, GetWorldTransform(jobj));
            var cameraPos = c.TransformedPosition;

            return Vector3.DistanceSquared(jointPosition, cameraPos);
        }

        /// <summary>
        /// 
        /// </summary>
        private void RenderBoneOverlay()
        {
            if (!_settings.RenderBones)
                return;

            GL.PushAttrib(AttribMask.AllAttribBits);

            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.DepthTest);

            float mag = 0;

            if (_settings.RenderOrientation)
                mag = 2; //Vector3.TransformPosition(new Vector3(1, 0, 0), camera.MvpMatrix.Inverted()).Length / 30;

            foreach (var b in jobjToCache)
                RenderBone(mag, b.Value, b.Key.Equals(SelectedJOBJ));

            GL.PopAttrib();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shader"></param>
        public void SetupShader()
        {
            // ui
            _gxShader.SelectedBone = IndexOf(SelectedJOBJ);
            _gxShader.WorldTransforms = GetWorldTransforms();
            _gxShader.BindTransforms = GetBindTransforms();
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

            if (_settings.RenderOrientation)
            {
                GL.LineWidth(1.5f);

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

            var local = GetLocalTransform(root, index);
            var world = local;

            if (parent != null)
            {
                if (root.Flags.HasFlag(JOBJ_FLAG.MTX_SCALE_COMPENSATE))
                {
                    // get parent scale
                    var parentTrackTransform = parent.LocalTransform.ExtractScale();

                    // get animated vectors
                    GetLocalTransformValues(root, out Vector3 trans, out Quaternion rot, out Vector3 scale, index);

                    // scale the position only
                    trans *= parentTrackTransform;

                    // build the local matrix
                    local = Matrix4.CreateScale(scale) *
                    Matrix4.CreateFromQuaternion(rot) *
                    Matrix4.CreateTranslation(trans);

                    // calculate the compensate scale
                    var scaleCompensation = Matrix4.CreateScale(1f / parentTrackTransform.X, 1f / parentTrackTransform.Y, 1f / parentTrackTransform.Z);

                    // apply scale compensate
                    world = local * scaleCompensation;

                }
                // multiply parent transform
                world = world * parent.WorldTransform;
            }

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
                if (root.Flags.HasFlag(JOBJ_FLAG.SKELETON) || root.Flags.HasFlag(JOBJ_FLAG.SKELETON_ROOT) && root.InverseWorldTransform != null)
                {
                    jcache.InvertedTransform = root.InverseWorldTransform.ToTKMatrix();
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
        public Matrix4 GetLocalTransform(HSD_JOBJ jobj, int animatedBoneIndex = -1)
        {
            if (animatedBoneIndex != -1 && Animation != null)
                return Animation.GetAnimatedMatrix(Frame, animatedBoneIndex, jobj);
            else
                return Matrix4.CreateScale(jobj.SX, jobj.SY, jobj.SZ) *
                Matrix4.CreateFromQuaternion(Math3D.FromEulerAngles(jobj.RZ, jobj.RY, jobj.RX)) *
                Matrix4.CreateTranslation(jobj.TX, jobj.TY, jobj.TZ);
        }

        /// <summary>
        /// Creates a Matrix4 from a HSD_JOBJ
        /// </summary>
        /// <param name="jobj"></param>
        /// <returns></returns>
        public void GetLocalTransformValues(HSD_JOBJ jobj, out Vector3 trans, out Quaternion rot, out Vector3 scale, int animatedBoneIndex = -1)
        {
            if (animatedBoneIndex != -1 && Animation != null)
            {
                Animation.GetAnimatedValues(Frame, animatedBoneIndex, jobj, out trans, out rot, out scale);
            }
            else
            {
                trans = new Vector3(jobj.TX, jobj.TY, jobj.TZ);
                rot = Math3D.FromEulerAngles(jobj.RZ, jobj.RY, jobj.RX);
                scale = new Vector3(jobj.SX, jobj.SY, jobj.SZ);
            }
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
        public ShapeAnimManager ShapeAnimation = new ShapeAnimManager();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="joint"></param>
        public void SetShapeAnimJoint(HSD_ShapeAnimJoint joint)
        {
            ShapeAnimation = new ShapeAnimManager();
            ShapeAnimation.FromShapeAnim(joint);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="joint"></param>
        public void SetMatAnimJoint(HSD_MatAnimJoint joint)
        {
            RefreshRendering = true;
            MatAnimation = new MatAnimManager();
            MatAnimation.FromMatAnim(joint);
            DOBJManager.PreLoadMatAnim(MatAnimation);
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
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public HSD_DOBJ GetDOBJAtIndex(int index)
        {
            var i = 0;
            foreach (var j in RootJOBJ.BreathFirstList)
            {
                if (j.Dobj != null)
                {
                    foreach (var dobj in j.Dobj.List)
                    {
                        if (i == index)
                            return dobj;
                        i++;
                    }
                }
            }
            return null;
        }


        private HashSet<int> HiddenDOBJIndices = new HashSet<int>();
        private HashSet<int> SelectedDOBJIndices = new HashSet<int>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selected"></param>
        public void SetSelectedDOBJs(IEnumerable<int> selected)
        {
            SelectedDOBJIndices.Clear();
            foreach (var i in selected)
                SelectedDOBJIndices.Add(i);
        }


        /// <summary>
        /// 
        /// </summary>
        public void SetHiddenDOBJs(IEnumerable<int> indices)
        {
            HiddenDOBJIndices.Clear();
            foreach (var i in indices)
                HiddenDOBJIndices.Add(i);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<int> GetHiddenDOBJIndices()
        {
            foreach (var h in HiddenDOBJIndices)
                yield return h;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void ShowDOBJ(int index)
        {
            HiddenDOBJIndices.Remove(index);
        }

        /// <summary>
        /// 
        /// </summary>
        public void ShowAllDOBJs()
        {
            HiddenDOBJIndices.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void HideDOBJ(int index)
        {
            HiddenDOBJIndices.Add(index);
        }

        /// <summary>
        /// 
        /// </summary>
        public void HideAllDOBJs()
        {
            for (int i = 0; i < DOBJManager.DOBJCount; i++)
                HiddenDOBJIndices.Add(i);
        }

        #endregion
    }
}
