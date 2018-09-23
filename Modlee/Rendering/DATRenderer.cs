using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using MeleeLib.DAT.Helpers;
using MeleeLib.DAT;
using MeleeLib.GCX;

namespace Modlee
{
    public class DATRenderer
    {
        public struct GLVertex
        {
            public Vector3 Pos;
            public Vector3 Nrm;
            public Vector2 UV0;
            public Vector4 Node;
            public Vector4 Weight;
            public const int Stride = 4 * (3 + 3 + 2 + 4 + 4);
        }
        private DATRoot root;

        public int VBO;
        public int IBO;

        private GLVertex[] Vertices;
        private List<DatJOBJ> JOBJ = new List<DatJOBJ>();
        private Matrix4[] Bones;

        public DATRenderer(DATRoot Root)
        {
            this.root = Root;
            GL.GenBuffers(1, out VBO);
            GL.GenBuffers(1, out IBO);

            List<GLVertex> Vert = new List<GLVertex>();
            GXVertexDecompressor b = new GXVertexDecompressor(Root);
            foreach (DatDOBJ d in Root.GetDataObjects())
            {
                foreach (DatPolygon p in d.Polygons)
                {
                    GXVertex[] Verts = b.GetFormattedVertices(p);
                    foreach (GXVertex v in Verts)
                    {
                        GLVertex nv = new GLVertex()
                        {
                            Pos = new Vector3(v.Pos.X, v.Pos.Y, v.Pos.Z),
                            Nrm = new Vector3(v.Nrm.X, v.Nrm.Y, v.Nrm.Z),
                            UV0 = new Vector2(v.TX0.X, v.TX0.Y)
                        };
                        if (v.N != null)
                        {
                            if (v.N.Length == 1)
                            {
                                nv.Node = new Vector4(v.N[0], -1, -1, -1);
                                nv.Weight = new Vector4(v.W[0], 0, 0, 0);
                            }
                            if (v.N.Length == 2)
                            {
                                nv.Node = new Vector4(v.N[0], v.N[1], -1, -1);
                                nv.Weight = new Vector4(v.W[0], v.W[1], 0, 0);
                            }
                            if (v.N.Length == 3)
                            {
                                nv.Node = new Vector4(v.N[0], v.N[1], v.N[2], -1);
                                nv.Weight = new Vector4(v.W[0], v.W[1], v.W[2], 0);
                            }
                            
                        }
                        else
                        {
                            nv.Node = new Vector4(-1, -1, -1, -1);
                            nv.Weight = new Vector4(0, 0, 0, 0);
                        }
                        Vert.Add(nv);
                    }
                }
            }

            Vertices = Vert.ToArray();
            

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData<GLVertex>(BufferTarget.ArrayBuffer, (IntPtr)(Vertices.Length * GLVertex.Stride), Vertices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            JOBJ.AddRange(Root.GetJOBJinOrder());
            Bones = CalculateTransforms(Root);

            Shader = new Shader();
            Shader.LoadShader("Rendering//Shader//DAT.vert");
            Shader.LoadShader("Rendering//Shader//DAT.frag");
        }

        Shader Shader;

        public void Render(ref Matrix4 mvp)//, TreeNodeCollection DOBJNodes
        {
            GL.UseProgram(Shader.programId);
            Shader.EnableVertexAttributes();

            GL.UniformMatrix4(Shader.GetVertexAttributeUniformLocation("mvp"), false, ref mvp);
            if(Bones.Length > 0)
            GL.UniformMatrix4(Shader.GetVertexAttributeUniformLocation("bones"), Bones.Length, false, ref Bones[0].Row0.X);
            
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.VertexAttribPointer(Shader.GetVertexAttributeUniformLocation("in_pos"), 3, VertexAttribPointerType.Float, false, GLVertex.Stride, 0);
            GL.VertexAttribPointer(Shader.GetVertexAttributeUniformLocation("in_nrm"), 3, VertexAttribPointerType.Float, false, GLVertex.Stride, 12);
            GL.VertexAttribPointer(Shader.GetVertexAttributeUniformLocation("in_uv0"), 2, VertexAttribPointerType.Float, false, GLVertex.Stride, 24);
            GL.VertexAttribPointer(Shader.GetVertexAttributeUniformLocation("in_node"), 4, VertexAttribPointerType.Float, false, GLVertex.Stride, 32);
            GL.VertexAttribPointer(Shader.GetVertexAttributeUniformLocation("in_weight"), 4, VertexAttribPointerType.Float, false, GLVertex.Stride, 48);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.Enable(EnableCap.Texture2D);
            GL.Uniform1(Shader.GetVertexAttributeUniformLocation("TEX_DIF"), 0);

            int off = 0;
            foreach (DatDOBJ d in root.GetDataObjects())
            {
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, 0);
                float XS = 1;
                float YS = 1;
                int index = JOBJ.IndexOf(d.Parent);
                Shader.SetVector3("BONEPOS", Vector3.TransformPosition(Vector3.Zero, Bones[index == -1 ? 0 : index]));
                Shader.SetBoolToInt("HasTexture", d.Material.Textures.Length > 0);
                if (d.Material.Textures.Length > 0)
                {
                    {
                        int texid = TextureManager.GetGLID(d.Material.Textures[0].GetStaticBitmap());
                        GL.BindTexture(TextureTarget.Texture2D, texid);
                        GL.TextureParameter(texid, TextureParameterName.TextureWrapS, GXtoGL.GetGLWrapMode(d.Material.Textures[0].WrapS));
                        GL.TextureParameter(texid, TextureParameterName.TextureWrapT, GXtoGL.GetGLWrapMode(d.Material.Textures[0].WrapT));

                        XS = d.Material.Textures[0].WScale;
                        YS = d.Material.Textures[0].HScale;
                    }
                }
                GL.Uniform1(Shader.GetVertexAttributeUniformLocation("TWScale"), XS);
                GL.Uniform1(Shader.GetVertexAttributeUniformLocation("THScale"), YS);
                GL.Uniform3(Shader.GetVertexAttributeUniformLocation("DIF"), d.Material.MaterialColor.DIF.R / 255f, d.Material.MaterialColor.DIF.G / 255f, d.Material.MaterialColor.DIF.B / 255f);
                GL.Uniform3(Shader.GetVertexAttributeUniformLocation("AMB"), d.Material.MaterialColor.AMB.R / 255f, d.Material.MaterialColor.AMB.G / 255f, d.Material.MaterialColor.AMB.B / 255f);
                GL.Uniform3(Shader.GetVertexAttributeUniformLocation("SPC"), d.Material.MaterialColor.SPC.R / 255f, d.Material.MaterialColor.SPC.G / 255f, d.Material.MaterialColor.SPC.B / 255f);

                //GXPrimitiveType Last = GXPrimitiveType.Triangles;
                foreach (DatPolygon p in d.Polygons)
                {
                    foreach (GXDisplayList dl in p.DisplayLists)
                    {
                        //if(n.Checked)
                        switch (dl.PrimitiveType)
                        {
                            case GXPrimitiveType.Points: GL.DrawArrays(PrimitiveType.Points, off, dl.Count); break;
                            case GXPrimitiveType.Lines: GL.DrawArrays(PrimitiveType.Lines, off, dl.Count); break;
                            case GXPrimitiveType.LineStrip: GL.DrawArrays(PrimitiveType.LineStrip, off, dl.Count); break;
                            case GXPrimitiveType.Triangles: GL.DrawArrays(PrimitiveType.Triangles, off, dl.Count); break;
                            case GXPrimitiveType.TriangleStrip: GL.DrawArrays(PrimitiveType.TriangleStrip, off, dl.Count); break;
                            case GXPrimitiveType.TriangleFan: GL.DrawArrays(PrimitiveType.TriangleFan, off, dl.Count); break;
                            case GXPrimitiveType.Quads: GL.DrawArrays(PrimitiveType.Quads, off, dl.Count); break;
                            default:
                                GL.DrawArrays(PrimitiveType.Triangles, off, dl.Count);
                                break;
                        }
                        off += dl.Count;
                        //if(dl.PrimitiveType != 0)
                        //    Last = dl.PrimitiveType;
                    }
                }
            }

            Shader.DisableVertexAttributes();
            GL.UseProgram(0);
        }

        public static Matrix4[] CalculateTransforms(DATRoot Root)
        {
            Dictionary<DatJOBJ, Matrix4> T = new Dictionary<DatJOBJ, Matrix4>();

            List<Matrix4> Transforms = new List<Matrix4>();

            foreach (DatJOBJ jobj in Root.GetJOBJinOrder())
            {
                //Console.WriteLine(jobj.ID.ToString("x"));
                Matrix4 trans = Matrix4.CreateScale(jobj.SX, jobj.SY, jobj.SZ) *
                    Matrix4.CreateFromQuaternion(Math3D.FromEulerAngles(jobj.RZ, jobj.RY, jobj.RX)) *
                    Matrix4.CreateTranslation(jobj.TX, jobj.TY, jobj.TZ);
                if (jobj.Parent != null) trans *= T[jobj.Parent];
                Transforms.Add(trans);
                if(!T.ContainsKey(jobj))
                T.Add(jobj, trans);
            }

            Console.WriteLine(Transforms.Count);

            return Transforms.ToArray();
        }

        public static void Render(DATRoot DAT)
        {
            /*Dictionary<DatJOBJ, Matrix4> T = new Dictionary<DatJOBJ, Matrix4>();

            List<Matrix4> Transforms = new List<Matrix4>();

            Queue<DatNode> nodes = new Queue<DatNode>();
            if (DAT.Bones.Nodes.Count == 0) return;
            nodes.Enqueue((DatJOBJ)DAT.Bones.Nodes[0]);

            while(nodes.Count > 0)
            {
                DatNode n = nodes.Dequeue();
                if(n is DatJOBJ)
                {
                    DatJOBJ jobj = (DatJOBJ)n;
                    Matrix4 trans = Matrix4.CreateScale(jobj.SX, jobj.SY, jobj.SZ) *
                        Matrix4.CreateFromQuaternion(Math3D.FromEulerAngles(jobj.RZ, jobj.RY, jobj.RX)) *
                        Matrix4.CreateTranslation(jobj.TX, jobj.TY, jobj.TZ);
                    if (jobj.Parent is DatJOBJ) trans *= T[(DatJOBJ)jobj.Parent];
                    Transforms.Add(trans);
                    T.Add(jobj, trans);
                    foreach (DatNode m in jobj.Nodes)
                        nodes.Enqueue(m);
                }
            }

            GL.Color3(Color.Green);
            GL.PointSize(5f);
            GL.Begin(PrimitiveType.Points);
            foreach(Matrix4 Trans in Transforms)
            {
                GL.Vertex3(Vector3.TransformPosition(Vector3.Zero, Trans));
            }
            GL.End();

            GL.Enable(EnableCap.Texture2D);
            foreach(DatDOBJ obj in DAT.Objects.Nodes)
            {
                GL.ActiveTexture(TextureUnit.Texture0);
                if(obj.Material.Texture != null)
                GL.BindTexture(TextureTarget.Texture2D, TextureManager.GetGLID(obj.Material.Texture.Image));
                foreach (DatPolygon p in obj.Polygons)
                {
                    //if (p.Visible)
                        foreach (GXDisplayList dl in p.DisplayLists)
                        {
                            switch (dl.PrimitiveType)
                            {
                                case 0xB8: GL.Begin(PrimitiveType.Points); break;
                                case 0xA8: GL.Begin(PrimitiveType.Lines); break;
                                case 0xB0: GL.Begin(PrimitiveType.LineStrip); break;
                                case 0x90: GL.Begin(PrimitiveType.Triangles); break;
                                case 0x98: GL.Begin(PrimitiveType.TriangleStrip); break;
                                case 0xA0: GL.Begin(PrimitiveType.TriangleFan); break;
                                case 0x80: GL.Begin(PrimitiveType.Quads); break;
                                default:
                                    GL.Begin(PrimitiveType.Points);
                                    break;
                            }
                            foreach (GXVertex v in dl.Vertices)
                            {
                                Vector3 Pos = new Vector3(v.X, v.Y, v.Z);
                                Vector3 Nrm = new Vector3(v.NX, v.NY, v.NZ);
                                if (v.N != null && v.N.Length == 1 && v.N[0] != 0 )
                                {
                                    int id = DAT.GetJOBJIndex(v.N[0]);
                                    if(id != -1)
                                    Pos = Vector3.TransformPosition(Pos, Transforms[id]) * v.W[0];

                                    //Nrm = Vector3.TransformNormal(Nrm, Transforms[DAT.GetJOBJIndex(v.N[0])]) * v.W[0];
                                }
                                float shad = 0.5f + Vector3.Dot(Nrm, Vector3.UnitX);
                                GL.Color3(shad, shad, shad);
                                GL.TexCoord2(v.TX0 * obj.Material.Texture.WScale, v.TY0 * obj.Material.Texture.HScale);

                                GL.Vertex3(Pos);
                            }
                            GL.End();
                        }
                        }
            }*/
        }
        
        public static void RenderGXDisplayList(GXDisplayList DL)
        {
            /*GL.Color3(1, 1, 0);
            switch (DL.PrimitiveType)
            {
                case 0xB8: GL.Begin(PrimitiveType.Points); break;
                case 0xA8: GL.Begin(PrimitiveType.Lines); break;
                case 0xB0: GL.Begin(PrimitiveType.LineStrip); break;
                case 0x90: GL.Begin(PrimitiveType.Triangles); break;
                case 0x98: GL.Begin(PrimitiveType.TriangleStrip); break;
                case 0xA0: GL.Begin(PrimitiveType.TriangleFan); break;
                case 0x80: GL.Begin(PrimitiveType.Quads); break;
                default:
                    GL.Begin(PrimitiveType.Points);
                    break;
            }
            foreach (GXVertex v in DL.Vertices)
                GL.Vertex3(v.X, v.Y, v.Z);
            GL.End();*/
        }
    }
}
