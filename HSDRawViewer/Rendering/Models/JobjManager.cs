using System;
using System.Collections.Generic;
using HSDRaw.Common;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using HSDRaw.Common.Animation;
using HSDRawViewer.Converters;
using HSDRawViewer.Rendering.Animation;
using HSDRawViewer.Rendering.GX;
using System.Drawing;
using HSDRaw.Melee.Pl;
using static HSDRawViewer.Rendering.Animation.PhysicsSimulator;

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

        public DOBJManager DOBJManager { get; internal set; } = new DOBJManager();

        public RenderMode RenderMode { get => _gxShader.RenderMode; set => _gxShader.RenderMode = value; }

        private GXShader _gxShader { get; set; } = new GXShader();

        public JobjDisplaySettings _settings { get; internal set; } = new JobjDisplaySettings();

        public GXLightParam _lightParam { get; internal set; } = new GXLightParam();

        public GXFogParam _fogParam { get; internal set; } = new GXFogParam();

        public int JointCount { get => RootJObj == null ? 0 : RootJObj.JointCount; }

        public bool RefreshRendering = false;

        private LiveJObj RootJObj;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="transform"></param>
        public void SetWorldTransform(int index, Matrix4 transform)
        {
            if (RootJObj == null)
                return;

            var jobj = RootJObj.GetJObjAtIndex(index);

            if (jobj != null)
            {
                jobj.WorldTransform = transform;
                jobj.Child?.RecalculateTransforms(null, true);
            }
        }

        /// <summary>
        /// Gets the JObj Descriptor at index
        /// </summary>
        /// <param name="jobj"></param>
        /// <returns></returns>
        public LiveJObj GetJOBJ(int index)
        {
            if (RootJObj == null)
                return null;

            return RootJObj.GetJObjAtIndex(index);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobj"></param>
        /// <returns></returns>
        public LiveJObj GetLiveJOBJ(HSD_JOBJ jobj)
        {
            if (RootJObj == null)
                return null;

            return RootJObj.GetJObjFromDesc(jobj);
        }

        /// <summary>
        /// Gets the world transform from the bone index
        /// Can be slow
        /// </summary>
        /// <param name="jobj"></param>
        /// <returns></returns>
        public Matrix4 GetWorldTransform(int index)
        {
            if (RootJObj == null)
                return Matrix4.Identity;

            var joint = RootJObj.GetJObjAtIndex(index);

            if (joint != null)
                return joint.WorldTransform;

            return Matrix4.Identity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobj"></param>
        /// <returns></returns>
        public Matrix4 GetWorldTransform(HSD_JOBJ jobj)
        {
            if (RootJObj == null)
                return Matrix4.Identity;

            var joint = RootJObj.GetJObjFromDesc(jobj);

            if (joint != null)
                return joint.WorldTransform;

            return Matrix4.Identity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobj"></param>
        /// <returns></returns>
        public Matrix4 GetBindTransform(HSD_JOBJ jobj)
        {
            var joint = RootJObj.GetJObjFromDesc(jobj);

            if (joint != null)
                return joint.BindTransform;

            return Matrix4.Identity;
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

            var j = RootJObj.GetJObjFromDesc(jobj);

            if (j != null)
                return j.Index;

            return -1;
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
            if (root == null)
                RootJObj = null;
            else
                RootJObj = new LiveJObj(root);
            UpdateNoRender();
        }

        /// <summary>
        /// 
        /// </summary>
        public void RecalculateInverseBinds()
        {
            RootJObj.RecalculateInverseBinds();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Render(Camera camera, bool update = true)
        {
            if (RootJObj == null)
                return;

            if (RefreshRendering)
                ClearRenderingCache();

            GL.PushAttrib(AttribMask.AllAttribBits);

            if(update)
                UpdateTransforms(camera);

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

            // no longer use shader
            _gxShader.Unbind();

            GL.PopAttrib();

            // render splines
            DrawSplines(camera);

            // bone overlay
            RenderBoneOverlay();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool BranchIsVisible(HSD_JOBJ jobj, LiveJObj cache)
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
            if (RootJObj == null)
                return;

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

                foreach (var b in RootJObj.Enumerate)
                {
                    MatAnimation.DOBJIndex = 0;
                    ShapeAnimation.DOBJIndex = 0;

                    if (!BranchIsVisible(b.Desc, b))
                    {
                        MatAnimation.JOBJIndex++;
                        ShapeAnimation.JOBJIndex++;
                        continue;
                    }

                    if (b.Desc.Dobj != null)
                    {
                        foreach (var dobj in b.Desc.Dobj.List)
                        {
                            // skip dobjs without mobjs
                            if (dobj.Mobj == null)
                                continue;

                            if (SelectedDOBJIndices.Contains(dobjIndex))
                                selected.Add(new Tuple<HSD_DOBJ, HSD_JOBJ>(dobj, b.Desc));

                            // check if hidden
                            if (!HiddenDOBJIndices.Contains(dobjIndex) &&
                                !(_settings.RenderObjects == ObjectRenderMode.Selected && !SelectedDOBJIndices.Contains(dobjIndex)))
                            {
                                // get shape blend amt
                                DOBJManager.ShapeBlend = ShapeAnimation.GetBlending();

                                if (dobj.Mobj.RenderFlags.HasFlag(RENDER_MODE.XLU))
                                    XLU.Add(new Tuple<HSD_DOBJ, HSD_JOBJ, int, int>(dobj, b.Desc, MatAnimation.JOBJIndex, MatAnimation.DOBJIndex));
                                else
                                    DOBJManager.RenderDOBJShader(GXShader._shader, dobj, b.Desc, this, MatAnimation);
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
            if (RootJObj == null)
                return;

            foreach (var j in RootJObj.Enumerate)
                if (j.Desc.Spline != null)
                    DrawShape.RenderSpline(j.Desc.Spline, Color.Yellow, Color.Blue);
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

            foreach (var b in RootJObj.Enumerate)
                RenderBone(mag, b, b.Desc.Equals(SelectedJOBJ));

            GL.PopAttrib();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shader"></param>
        public void SetupShader()
        {
            if (RootJObj == null)
                return;

            // ui
            _gxShader.SelectedBone = IndexOf(SelectedJOBJ);

            int i = 0;
            foreach (var v in RootJObj.Enumerate)
                if (i < Shader.MAX_BONES)
                {
                    _gxShader.WorldTransforms[i] = v.WorldTransform;
                    _gxShader.WorldTransforms[i + Shader.MAX_BONES] = v.BindTransform;
                    i++;
                }
                else
                    break;

            //_gxShader.SetBoneTransforms(ref BoneTransforms, ref BindTransforms);
        }


        /// <summary>
        /// 
        /// </summary>
        public void UpdateNoRender()
        {
            UpdateTransforms(null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="parentTransform"></param>
        private void RenderBone(float mag, LiveJObj jobj, bool selected)
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
        private void UpdateTransforms(Camera cam)
        {
            if (RootJObj == null)
                return;

            if (Animation != null)
                Animation.ApplyAnimation(RootJObj, Frame);

            // TODO: model scale
            //if (parent == null)
            //    world *= Matrix4.CreateScale(ModelScale);
            if (PhysicsAnimation != null)
                PhysicsAnimation.ProcessAndApplyDynamics(this, true);

            RootJObj.RecalculateTransforms(cam, true);//, Matrix4.CreateScale(ModelScale));
        }

        /// <summary>
        /// Removes a JOBJ from the tree
        /// </summary>
        /// <param name="jobj"></param>
        public void RemoveJOBJ(HSD_JOBJ jobj)
        {
            // TODO: remove root?
            //if (RootJObj.Desc == jobj)
            //{
            //    RootJObj = RootJObj.Child;
            //    return;
            //}

            foreach (var v in RootJObj.Enumerate)
            {
                if (v.Sibling != null && v.Sibling.Desc == jobj)
                {
                    v.Sibling.Desc.Next = v.Sibling.Sibling.Desc.Next;
                    v.Sibling = v.Sibling.Sibling;
                    break;
                }

                if (v.Child != null && v.Child.Desc == jobj)
                {
                    v.Child.Desc = v.Child.Sibling.Desc;
                    v.Child = v.Child.Sibling;
                    break;
                }
            }

            ClearRenderingCache();
        }

        #region Physics

        // private PhysicsPlayer dynamics;



        #endregion

        #region Animation Loader

        public JointAnimManager Animation { get; set; } = new JointAnimManager();
        public MatAnimManager MatAnimation { get; set; } = new MatAnimManager();
        public ShapeAnimManager ShapeAnimation { get; set; } = new ShapeAnimManager();
        public PhysicsPlayer PhysicsAnimation { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        private void ResetModel()
        {
            if (RootJObj != null)
            {
                RootJObj.ResetTransforms();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="desc"></param>
        /// <param name="bubbles"></param>
        public void LoadPhysics(IEnumerable<SBM_DynamicDesc> desc, IEnumerable<SBM_DynamicHitBubble> bubbles)
        {
            RootJObj?.RecalculateTransforms(null, true);
            PhysicsAnimation = PhysicsSimulator.Init(desc, bubbles, this);
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClearPhysics()
        {
            PhysicsAnimation = null;

            if (RootJObj != null)
                RootJObj.ResetTransforms();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="joint"></param>
        public void SetShapeAnimJoint(HSD_ShapeAnimJoint joint)
        {
            ResetModel();
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
            ResetModel();
            Animation = new JointAnimManager();
            Animation.FromAnimJoint(joint);
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetFigaTree(HSD_FigaTree tree)
        {
            ResetModel();
            Animation = new JointAnimManager();
            Animation.FromFigaTree(tree);
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetMOT(short[] jointTable, MOT_FILE file)
        {
            ResetModel();
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
            foreach (var j in RootJObj.Enumerate)
            {
                if (j.Desc.Dobj != null)
                {
                    foreach (var dobj in j.Desc.Dobj.List)
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
