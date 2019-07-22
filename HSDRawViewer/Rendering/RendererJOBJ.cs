using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw.GX;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HSDRawViewer.Rendering
{
    public class RendererJOBJ
    {
        // Matrix Cache
        private Dictionary<HSD_JOBJ, Matrix4> jobjToWorldTransformCache = new Dictionary<HSD_JOBJ, Matrix4>();
        private Dictionary<HSD_JOBJ, Matrix4> jobjToBindMatrixCache = new Dictionary<HSD_JOBJ, Matrix4>();

        // Vertex Cache
        private Dictionary<HSD_JOBJ, List<HSD_DOBJ>> jobjToDOBJList = new Dictionary<HSD_JOBJ, List<HSD_DOBJ>>();

        private Dictionary<HSD_POBJ, GX_DisplayList> pobjToDisplayList = new Dictionary<HSD_POBJ, GX_DisplayList>();

        // Texture cache
        private Dictionary<byte[], int> imageBufferToTexture = new Dictionary<byte[], int>();

        public void Render(HSD_JOBJ jobj)
        {
            GL.PushAttrib(AttribMask.AllAttribBits);

            BoneIndex = 0;
            jobjToDOBJList.Clear();
            RenderJOBJ(jobj);

            // Render DOBJs after JOBJ transforms have been calcuated
            foreach (var d in jobjToDOBJList)
            {
                foreach (var dobjs in d.Value)
                {
                    RenderDOBJ(dobjs, jobjToWorldTransformCache[d.Key]);
                }
            }
            GL.PopAttrib();
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClearCache()
        {
            jobjToBindMatrixCache.Clear();
            jobjToWorldTransformCache.Clear();
            jobjToDOBJList.Clear();
            pobjToDisplayList.Clear();

            foreach (var v in imageBufferToTexture)
                GL.DeleteTexture(v.Value);

            imageBufferToTexture.Clear();
        }

        /// <summary>
        /// Renders JOBJs
        /// </summary>
        /// <param name="jobj"></param>
        private void RenderJOBJ(HSD_JOBJ jobj)
        {
            GL.PointSize(5f);

            foreach (var list in jobj.List)
                TreeRenderJOBJ(list, Matrix4.Identity, Matrix4.Identity);
        }

        /// <summary>
        /// Renders JOBJs
        /// </summary>
        /// <param name="jobj"></param>
        /// <param name="Transform"></param>
        private void TreeRenderJOBJ(HSD_JOBJ jobj, Matrix4 Transform, Matrix4 bindTransform)
        {
            var bind = CreateJOBJTransform(jobj, false) * bindTransform;
            var transform = CreateJOBJTransform(jobj, true) * Transform;

            if (!jobjToWorldTransformCache.ContainsKey(jobj))
                jobjToWorldTransformCache.Add(jobj, transform);
            jobjToWorldTransformCache[jobj] = transform;

            if (!jobjToBindMatrixCache.ContainsKey(jobj))
                jobjToBindMatrixCache.Add(jobj, bind);
            jobjToBindMatrixCache[jobj] = bind.Inverted();

            GL.Begin(PrimitiveType.Points);
            GL.Vertex3(Vector3.TransformPosition(Vector3.Zero, transform));
            GL.End();

            // Render DOBJ
            // oh boy...
            if (jobj.Dobj != null)
            {
                jobjToDOBJList.Add(jobj, jobj.Dobj.List);
                foreach (var d in jobj.Dobj.List)
                {
                    if (d.Mobj != null && d.Mobj.Textures != null)
                        LoadTOBJ(d.Mobj.Textures);

                    if (d.Pobj != null)
                        foreach (var p in d.Pobj.List)
                        {
                            if (!pobjToDisplayList.ContainsKey(p))
                            {
                                pobjToDisplayList.Add(p, p.ToDisplayList());
                            }
                        }
                }
            }

            // children
            if (jobj.Child != null)
                foreach (var child in jobj.Children)
                {
                    TreeRenderJOBJ(child, transform, bind);
                }
        }

        private int BoneIndex = 0;

        /// <summary>
        /// Creates a Matrix4 from a HSD_JOBJ
        /// </summary>
        /// <param name="jobj"></param>
        /// <returns></returns>
        private Matrix4 CreateJOBJTransform(HSD_JOBJ jobj, bool animated)
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

            if (animated && Frame != -1 && Nodes.Count > BoneIndex)
            {
                AnimNode node = Nodes[BoneIndex];
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
                BoneIndex++;
            }

            Matrix4 Transform = Matrix4.CreateScale(SX, SY, SZ) *
                Matrix4.CreateFromQuaternion(Math3D.FromEulerAngles(RZ, RY, RX)) *
                Matrix4.CreateTranslation(TX, TY, TZ);

            return Transform;
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadTOBJ(HSD_TOBJ tobj)
        {
            if (tobj.ImageData != null && !imageBufferToTexture.ContainsKey(tobj.ImageData.ImageData))
            {
                int texid;
                GL.GenTextures(1, out texid);

                GL.BindTexture(TextureTarget.Texture2D, texid);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, tobj.ImageData.Width, tobj.ImageData.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, tobj.GetDecodedImageData());

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, 1);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
                GL.BindTexture(TextureTarget.Texture2D, 0);

                imageBufferToTexture.Add(tobj.ImageData.ImageData, texid);
                Console.WriteLine("Loaded Texture: " + texid + " " + tobj.ImageData.Format);
            }
        }

        /// <summary>
        /// Renders a DOBJ to the scene
        /// </summary>
        private void RenderDOBJ(HSD_DOBJ dobj, Matrix4 Transform)
        {
            if (dobj == null || dobj.Pobj == null)
                return;

            RenderDOBJ_Legacy(dobj, Transform);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mobj"></param>
        private void BindMOBJ_Legacy(HSD_MOBJ mobj, out float wscale, out float hscale)
        {
            wscale = 1;
            hscale = 1;
            if (mobj == null)
                return;

            var pp = mobj.PixelProcessing;
            if (pp != null)
            {
                //GL.AlphaFunc(GXTranslator.toAlphaFunction(pp.AlphaComp0), pp.AlphaRef0 / 255f);

                GL.BlendFunc(GXTranslator.toBlendingFactor(pp.SrcFactor), GXTranslator.toBlendingFactor(pp.DstFactor));

                GL.DepthFunc(GXTranslator.toDepthFunction(pp.DepthFunction));
                
                //GL.BlendEquation(GXTranslator.toBlendEquationMode(pp.BlendMode));
            }

            var color = mobj.MaterialColor;
            if(color != null)
            {
            }

            // Bind Textures
            if (mobj.Textures != null && imageBufferToTexture.ContainsKey(mobj.Textures.ImageData.ImageData))
            {
                GL.Enable(EnableCap.Texture2D);
                foreach (var tex in mobj.Textures.List)
                {
                    if (!imageBufferToTexture.ContainsKey(tex.ImageData.ImageData))
                        return;

                    var texid = imageBufferToTexture[tex.ImageData.ImageData];

                    GL.ActiveTexture(TextureUnit.Texture0);
                    GL.BindTexture(TextureTarget.Texture2D, texid);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GXTranslator.toWrapMode(tex.WrapS));
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GXTranslator.toWrapMode(tex.WrapT));
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GXTranslator.toMagFilter(tex.MagFilter));

                    wscale = tex.WScale;
                    hscale = tex.HScale;
                }
            }
            else
            {
                GL.Disable(EnableCap.Texture2D);
            }
        }

        /// <summary>
        /// Renders a data object using Legacy OpenGL
        /// Warning: this can be very slow
        /// </summary>
        /// <param name="dobj"></param>
        private void RenderDOBJ_Legacy(HSD_DOBJ dobj, Matrix4 Transform)
        {
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            GL.MultMatrix(ref Transform);

            // bind materials/textures

            float wscale = 1;
            float hscale = 1;
            //GL.Enable(EnableCap.AlphaTest);
            //GL.AlphaFunc(AlphaFunction.Lequal, 255);
            GL.Enable(EnableCap.Blend);
            GL.BlendEquation(BlendEquationMode.FuncAdd);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);
            BindMOBJ_Legacy(dobj.Mobj, out wscale, out hscale);

            // render primitives
            foreach (var p in dobj.Pobj.List)
            {
                if (!pobjToDisplayList.ContainsKey(p))
                    continue;

                var dl = pobjToDisplayList[p];

                var envelopeWeights = p.EnvelopeWeights;

                int offset = 0;
                foreach (var g in dl.Primitives)
                {
                    GL.Begin(GXTranslator.toPrimitiveType(g.PrimitiveType));
                    for (int i = 0; i < g.Count; i++)
                    {
                        var pos = GXTranslator.toVector3(dl.Vertices[offset + i].POS);
                        var tx0 = GXTranslator.toVector2(dl.Vertices[offset + i].TEX0);
                        tx0.X *= wscale;
                        tx0.Y *= hscale;
                        if (envelopeWeights != null)
                        {
                            if (dl.Vertices[offset + i].PNMTXIDX / 3 >= envelopeWeights.Length)
                                throw new Exception((dl.Vertices[offset + i].PNMTXIDX / 3) + " " + envelopeWeights.Length);
                            var en = envelopeWeights[dl.Vertices[offset + i].PNMTXIDX / 3];
                            if (en.EnvelopeCount == 0)
                            {

                            }
                            else
                            if (en.EnvelopeCount == 1)
                            {
                                var t = jobjToWorldTransformCache[en.GetJOBJAt(0)];
                                pos = Vector3.TransformPosition(pos, t);
                            }
                            else
                            {
                                Vector3 bindpos = Vector3.Zero;
                                for (int j = 0; j < en.EnvelopeCount; j++)
                                {
                                    var inv = jobjToBindMatrixCache[en.GetJOBJAt(j)];
                                    var anim = jobjToWorldTransformCache[en.GetJOBJAt(j)];
                                    bindpos += Vector3.TransformPosition(pos, inv * anim) * en.GetWeightAt(j);
                                }
                                pos = bindpos;
                            }
                        }


                        GL.TexCoord2(tx0);
                        GL.Vertex3(pos);
                    }
                    GL.End();
                    offset += g.Count;
                }
            }

            GL.PopMatrix();
        }



        #region Animation Loader

        private List<AnimNode> Nodes = new List<AnimNode>();
        public int Frame = 0;
        public float FrameCount { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="joint"></param>
        public void SetAnimJoint(HSD_AnimJoint joint)
        {
            FrameCount = 0;
            Nodes.Clear();
            if (joint == null)
                return;
            foreach (var j in joint.DepthFirstList)
            {
                AnimNode n = new AnimNode();
                if (j.AOBJ != null)
                {
                    FrameCount = Math.Max(FrameCount, j.AOBJ.EndFrame);

                    foreach (var fdesc in j.AOBJ.FObjDesc.List)
                    {
                        var fobj = fdesc.FOBJ;
                        AnimTrack track = new AnimTrack();
                        track.TrackType = fobj.AnimationType;
                        track.Keys = fobj.GetDecodedKeys();
                        n.Tracks.Add(track);
                    }
                }
                Nodes.Add(n);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetFigaTree(HSD_FigaTree tree)
        {
            Nodes.Clear();
            if (tree == null)
                return;
            FrameCount = tree.FrameCount;
            foreach (var tracks in tree.Nodes)
            {
                AnimNode n = new AnimNode();
                foreach (HSD_Track t in tracks.Tracks)
                {
                    AnimTrack track = new AnimTrack();
                    track.TrackType = t.FOBJ.AnimationType;
                    track.Keys = t.GetKeys();
                    n.Tracks.Add(track);
                }
                Nodes.Add(n);
            }
        }

#endregion
    }
}
