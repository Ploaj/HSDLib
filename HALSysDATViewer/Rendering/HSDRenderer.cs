using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HSDLib;
using HSDLib.Common;
using HSDLib.GX;
using HSDLib.Helpers;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace HALSysDATViewer.Rendering
{
    public class HSDRenderer
    {
        public class RenderDOBJ
        {
            public HSD_MOBJ Material;
            public HSD_DOBJ DOBJ;
            public int JOBJIndex;
        }

        public class GLPrimitveGroup
        {
            public PrimitiveType PrimitiveType;
            public int Offset;
            public int Count;
            public RenderDOBJ RenderDOBJ;
        }

        public Shader Shader;
        public int VBO;

        private HSD_JOBJ RootNode
        {
            get
            {
                return _rootNode;
            }
            set
            {
                _rootNode = value;
                RefreshRendering();
            }
        }
        private HSD_JOBJ _rootNode;
        public List<GLPrimitveGroup> Primitives = new List<GLPrimitveGroup>();
        public GXVertex[] Vertices;
        public GLTexture TEX0;
        
        public HSDRenderer()
        {
            GL.GenBuffers(1, out VBO);
            Shader = new Shader();
            Shader.LoadShader("Rendering\\HSD.vert");
            Shader.LoadShader("Rendering\\HSD.frag");
        }

        public void SetHSD(HSD_JOBJ RootNode)
        {
            this.RootNode = RootNode;
        }

        private Matrix4[] InverseBinds;
        private Matrix4[] Binds;

        public Matrix4 FromGXMatrix(HSD_Matrix4x3 m)
        {
            return new Matrix4(m.M00, m.M01, m.M02, m.M03,
                m.M10, m.M11, m.M12, m.M13,
                m.M20, m.M21, m.M22, m.M23,
                0, 0, 0, 1);
        }

        public void UpdateBinds(HSD_JOBJ jobj, Matrix4 Parent, List<Matrix4> Matrices)
        {
            Matrix4 Transform = Matrix4.CreateScale(jobj.Transforms.SX, jobj.Transforms.SY, jobj.Transforms.SZ) *
                Matrix4.CreateFromQuaternion(Math3D.FromEulerAngles(jobj.Transforms.RZ, jobj.Transforms.RY, jobj.Transforms.RX)) *
                Matrix4.CreateTranslation(jobj.Transforms.TX, jobj.Transforms.TY, jobj.Transforms.TZ);

            if (Parent != null)
                Transform *= Parent;

            Matrices.Add(Transform);
            if (jobj.Child != null)
                foreach (HSD_JOBJ j in jobj.Children)
                {
                    UpdateBinds(j, Transform, Matrices);
                }
        }

        public void RefreshRendering()
        {
            TextureManager.Clear();

            List<HSD_JOBJ> BoneList = RootNode.DepthFirstList;
            List<RenderDOBJ> DOBJList = GetDOBJS(BoneList);

            InverseBinds = new Matrix4[BoneList.Count];
            for(int i =0; i < BoneList.Count; i++)
            {
                HSD_JOBJ jobj = BoneList[i];
                if (jobj.InverseMatrix != null)
                    InverseBinds[i] = FromGXMatrix(jobj.InverseMatrix);
                else
                    InverseBinds[i] = Matrix4.Identity;
            }

            List<Matrix4> Matrices = new List<Matrix4>(BoneList.Count);
            UpdateBinds(RootNode, Matrix4.Identity, Matrices);
            Binds = Matrices.ToArray();

            List<GXVertex> Vertices = new List<GXVertex>();
            int offset = 0;
            foreach(RenderDOBJ rdobj in DOBJList)
            {
                foreach(HSD_POBJ pobj in rdobj.DOBJ.POBJ.List)
                {
                    // Decode the Display List Data
                    GXDisplayList DisplayList = new GXDisplayList(pobj.DisplayListBuffer, pobj.VertexAttributes);
                    Vertices.AddRange(VertexAccessor.GetDecodedVertices(DisplayList, pobj, RootNode));
                    foreach(GXPrimitiveGroup g in DisplayList.Primitives)
                    {
                        GLPrimitveGroup GL = new GLPrimitveGroup();
                        GL.PrimitiveType = GXTranslator.toPrimitiveType(g.PrimitiveType);
                        GL.Offset = offset;
                        GL.Count = g.Indices.Length;
                        GL.RenderDOBJ = rdobj;
                        offset += GL.Count;
                        Primitives.Add(GL);
                    }
                }
            }
            this.Vertices = Vertices.ToArray();
            BindArray(Vertices.ToArray());
        }

        public void BindArray(GXVertex[] Vertices)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(GXVertex.Stride * Vertices.Length), Vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public List<RenderDOBJ> GetDOBJS(List<HSD_JOBJ> BoneList)
        {
            List<RenderDOBJ> DOBJS = new List<RenderDOBJ>();
            foreach (HSD_JOBJ JOBJ in BoneList)
                if(JOBJ.DOBJ != null)
                    foreach(HSD_DOBJ dobj in JOBJ.DOBJ.List)
                    {
                        RenderDOBJ render = new RenderDOBJ();
                        render.DOBJ = dobj;
                        render.Material = dobj.MOBJ;
                        render.JOBJIndex = BoneList.IndexOf(JOBJ);
                        DOBJS.Add(render);
                    }
            return DOBJS;
        }

        public void Render(ref Camera Camera)
        {
            GL.UseProgram(Shader.programId);
            Shader.EnableVertexAttributes();

            Matrix4 mvp = Camera.mvpMatrix;
            GL.UniformMatrix4(Shader.GetVertexAttributeUniformLocation("mvp"), false, ref mvp);
            //if (Bones.Length > 0)
            GL.UniformMatrix4(Shader.GetVertexAttributeUniformLocation("binds"), Binds.Length, false, ref Binds[0].Row0.X);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.VertexAttribPointer(Shader.GetVertexAttributeUniformLocation("in_pos"), 3, VertexAttribPointerType.Float, false, GXVertex.Stride, 0);
            GL.VertexAttribPointer(Shader.GetVertexAttributeUniformLocation("in_nrm"), 3, VertexAttribPointerType.Float, false, GXVertex.Stride, 12);
            GL.VertexAttribPointer(Shader.GetVertexAttributeUniformLocation("in_tex0"), 2, VertexAttribPointerType.Float, false, GXVertex.Stride, 24);
            GL.VertexAttribPointer(Shader.GetVertexAttributeUniformLocation("in_binds"), 4, VertexAttribPointerType.Int, false, GXVertex.Stride, 60);
            GL.VertexAttribPointer(Shader.GetVertexAttributeUniformLocation("in_weights"), 4, VertexAttribPointerType.Float, false, GXVertex.Stride, 76);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.Uniform1(Shader.GetVertexAttributeUniformLocation("TEX0"), 0);

            foreach (GLPrimitveGroup p in Primitives)
            {
                GL.ActiveTexture(TextureUnit.Texture0);
                GLTexture tex = TextureManager.GetGLTexture(p.RenderDOBJ.Material.Textures);
                Shader.SetInt("JOBJIndex", p.RenderDOBJ.JOBJIndex);
                if(tex != null)
                GL.BindTexture(TextureTarget.Texture2D, tex.ID);
                if(p.RenderDOBJ.Material.Textures != null)
                {
                    Shader.SetInt("UVSW", p.RenderDOBJ.Material.Textures.WScale);
                    Shader.SetInt("UVSH", p.RenderDOBJ.Material.Textures.HScale);
                }
                GL.DrawArrays(p.PrimitiveType, p.Offset, p.Count);
            }

            Shader.DisableVertexAttributes();
            GL.UseProgram(0);

        }

        // Do Not Use
        public void PrimitiveRender()
        {
            foreach(GLPrimitveGroup p in Primitives)
            {
                GL.PointSize(10f);
                GL.Begin(p.PrimitiveType);

                for (int i = 0; i < p.Count; i++)
                {
                    GL.Color3(Vertices[p.Offset + i].Pos.X, Vertices[p.Offset + i].Pos.Y, Vertices[p.Offset + i].Pos.Z);
                    GL.Vertex3(Vertices[p.Offset + i].Pos.X, Vertices[p.Offset + i].Pos.Y, Vertices[p.Offset + i].Pos.Z);
                }

                GL.End();
            }
        }
    }
}
