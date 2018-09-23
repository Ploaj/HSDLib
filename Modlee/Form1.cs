using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MeleeLib;
using System.IO;
using MeleeLib.DAT;
using MeleeLib.IO;
using MeleeLib.GCX;
using MeleeLib.DAT.Melee;

using MeleeLib.DAT.Animation;
using MeleeLib.DAT.Helpers;

using MeleeLib.KAR;

using HSDLib;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Modlee
{
    public partial class Form1 : Form
    {
        public class DOBJNode : TreeNode
        {
            public DatDOBJ DOBJ;

            public DOBJNode(DatDOBJ d)
            {
                DOBJ = d;
                Checked = true;
            }
        }

        Camera Camera = new Camera();
        bool ReadyToRender = false;
        List<DATRenderer> Renderers = new List<DATRenderer>();
        DATFile File;

        public Form1()
        {
            InitializeComponent();

            Console.WriteLine("File1");
            File = Decompiler.Decompile(System.IO.File.ReadAllBytes("C:\\Users\\Owen\\Desktop\\Modlee\\KAR\\VcStarNormal.dat"));
            //Compiler.Compile(File, "C:\\Users\\Owen\\Desktop\\Modlee\\KAR\\StarNormal");
            //Console.WriteLine("Roots: " + ((KAR_HSD_Vehicle)File.Roots[0].ExtraNodes[0]).Models.ModelRoot.GetJOBJinOrder().Length);
            //Console.WriteLine("File2");
            //File = Decompiler.Decompile(System.IO.File.ReadAllBytes("C:\\Users\\Owen\\Desktop\\Modlee\\KAR\\StarNormal"));
            
            //FileCheck----------------------------------------------------------
            /*string[] files = Directory.GetFiles("C:\\Users\\Owen\\Desktop\\Modlee\\Melee\\root\\");
            foreach(string f in files)
            {
                if (Path.GetFileName(f).StartsWith("Gr") || (Path.GetFileName(f).StartsWith("Pl") && Path.GetFileName(f).EndsWith("Nr.dat")))
                {
                    try
                    {
                        Decompiler.Decompile(System.IO.File.ReadAllBytes(f));
                    } catch(Exception)
                    {
                        Console.WriteLine("Can't Open " + Path.GetFileName(f));
                    }
                }
            }*/

            //System.IO.File.WriteAllBytes("C:\\Users\\Owen\\Desktop\\Modlee\\Tests\\CompTextureDecom.raw", TPL.ToCMP(System.IO.File.ReadAllBytes("C:\\Users\\Owen\\Desktop\\Modlee\\Tests\\CompTexture.raw"), 128, 128));

            //Compiler.Compile(File, "C:\\Users\\Owen\\Desktop\\Modlee\\Melee\\root\\PlClNr.dat");
            //Console.WriteLine("Test--------------------------------");
            //File = Decompiler.Decompile(System.IO.File.ReadAllBytes("C:\\Users\\Owen\\Desktop\\Modlee\\Melee\\root\\PlClNr.dat"));



            //DATFile aFile = Decompiler.Decompile(System.IO.File.ReadAllBytes("C:\\Users\\Owen\\Desktop\\Modlee\\Tests\\NessNair.dat"));
            //Tests.ExportDatToSMD("C:\\Users\\Owen\\Desktop\\Modlee\\NessNair.smd", File);
            //Tests.TestAnimationBuild(File, aFile.Roots[0].Animations[0], "C:\\Users\\Owen\\Desktop\\Modlee\\NessNair.smd");

            /*GXVertexCompressor comp = new GXVertexCompressor();
            GXVertexDecompressor decom = new GXVertexDecompressor(File);

            List<int> UsedBones = new List<int>();
            foreach(DatDOBJ d in File.Roots[0].GetDataObjects())
            {
                foreach(DatPolygon p in d.Polygons)
                {
                    List<GXDisplayList> newDL = new List<GXDisplayList>();
                    foreach(GXDisplayList dl in p.DisplayLists)
                    {
                        GXVertex[] verts = decom.GetFormattedVertices(dl, p);
                        foreach(GXVertex v in verts)
                        {
                            if (v.N == null) continue;
                            foreach(int b in v.N)
                            {
                                if (!UsedBones.Contains(b))
                                    UsedBones.Add(b);
                            }
                        }
                        newDL.Add(comp.CompressDisplayList(decom.GetFormattedVertices(dl, p), dl.PrimitiveType, p.AttributeGroup));
                    }
                    p.DisplayLists = newDL;
                }
            }
            comp.CompileChanges();
            /*UsedBones.Sort();
            foreach (int b in UsedBones)
                Console.WriteLine("Bone_"+b);*/

            /*Compiler.Compile(File, "TestNess.dat");
            File = Decompiler.Decompile(System.IO.File.ReadAllBytes("TestNess.dat"));*/

            //System.IO.File.WriteAllBytes("Test1.bin", File.DataBuffer);
            //File.DataBuffer = comp.GetBuffer();
            //System.IO.File.WriteAllBytes("Test2.bin", File.DataBuffer);

            //Compiler.Compile(File, "C:\\Users\\Owen\\Desktop\\Modlee\\Tests\\PlPkNr.dat");
            //File = Decompiler.Decompile(System.IO.File.ReadAllBytes("C:\\Users\\Owen\\Desktop\\Modlee\\Tests\\PlPkNr.dat"));

            if (1==2)
            {
                DatAnimation a = new DatAnimation();
                int FrameCount;
                List<AnimationHelperNode> Nodes = CHR0AnimLoader.GetTracks("C:\\Users\\Owen\\Desktop\\Modlee\\NewCHR.chr0", out FrameCount);

                string[] bs = System.IO.File.ReadAllLines("C:\\Users\\Owen\\Desktop\\Modlee\\LucasImport\\NessBoneNames.txt");
                Dictionary<int, String> BoneNameLookup = new Dictionary<int, string>();
                foreach(string s in bs) { BoneNameLookup.Add(int.Parse(s.Split(' ')[0]), s.Split(' ')[1]); }

                a.FrameCount = FrameCount;
                
                Console.WriteLine("Node Count " + Nodes.Count + " " + File.Roots[0].GetJOBJinOrder().Length);
                DatJOBJ[] jobjs = File.Roots[0].GetJOBJinOrder();
                for (int j = 0; j < 63; j++)
                {
                    DatAnimationNode node = new DatAnimationNode();
                    string BoneName = "Bone_"+j;
                    //if (BoneNameLookup.ContainsKey(j))
                    //    BoneName = BoneNameLookup[j];
                    //Console.WriteLine(j + " " + BoneName + " " + jobjs[j].RX + " " + jobjs[j].RY + " " + jobjs[j].SZ);
                    foreach (AnimationHelperNode n in Nodes)
                    {
                        if (n.Name.Equals(BoneName))
                        {
                            node = (AnimationKeyFrameHelper.EncodeKeyFrames(n.Tracks.ToArray(), (int)a.FrameCount));
                        }
                    }
                    a.Nodes.Add(node);
                }



                DATFile f = new DATFile();
                DATRoot anim = new DATRoot();
                anim.Text = "PlyNess5K_Share_ACTION_AttackAirF_figatree";
                f.AddRoot(anim);
                anim.Animations.Add(a);
                Compiler.Compile(f, "C:\\Users\\Owen\\Desktop\\LucasTestAnim.dat");

                DATFile compi = Decompiler.Decompile(System.IO.File.ReadAllBytes("C:\\Users\\Owen\\Desktop\\LucasTestAnim.dat"));
                Tests.TestAnimationBuild(File, compi.Roots[0].Animations[0], "C:\\Users\\Owen\\Desktop\\LucasIdle2.smd");

                {
                    /*DATFile Animdat = Decompiler.Decompile(System.IO.File.ReadAllBytes("C:\\Users\\Owen\\Desktop\\PlNsAJ.dat"));
                    DatAnimation an = Animdat.Roots[0].Animations[0];

                    for(int i = 0; i < an.Nodes.Count; i++)
                    {
                        an.Nodes[i] = AnimationKeyFrameHelper.EncodeKeyFrames(AnimationKeyFrameHelper.DecodeKeyFrames(an.Nodes[i]).ToArray());
                    }

                    Compiler.Compile(Animdat, "C:\\Users\\Owen\\Desktop\\LucasTestAnim.dat");*/
                }

            }

            //return;
            //AnimationHelperTrack[] Tracks = AnimationKeyFrameHelper.DecodeKeyFrames(File.Roots[0].Animations[0].Nodes[0]);
            //Console.WriteLine(File.Roots[0].GetJOBJinOrder().Length);

            int ni = 0;
            foreach(DATRoot r in File.Roots)
            {
                if(r.Bones.Count > 1)
                {
                    foreach (DatDOBJ d in r.GetDataObjects())
                    {
                        DOBJNode node = new DOBJNode(d);
                        node.Text = ni++ + "_DOBJ";
                        treeView1.Nodes.Add(node);
                    }
                    break;
                }

                
            }


            /*DatMatAnim[] MatAnims = File.Roots[1].GetMatAnimsinOrder();
            DatJOBJ[] JOBJS = File.Roots[0].GetJOBJinOrder();
            for(int i = 0; i < MatAnims.Length; i++)
            {
                if(MatAnims[i].Groups.Count > 0)
                {
                    //Console.WriteLine(i);
                    //Console.WriteLine(MatAnims[i].Groups.Count + " " + JOBJS[i].DataObjects.Count);
                    for(int j = 0; j < MatAnims[i].Groups.Count; j++)
                    {
                        if(MatAnims[i].Groups[j].Data.Count > 0)
                        {
                            //JOBJS[i].DataObjects[j].Material.Texture_Diffuse.ImageData.Data = MatAnims[i].Groups[j].Data[0].Textures[2].Data;
                        }
                    }
                }
            }*/

            /*bool done = true;
            foreach (DatJOBJ j in File.Roots[0].GetJOBJinOrder())
            {

                if (j.DataObjects.Count > 0)
                {
                    //int flags = j.DataObjects[0].Material.Flags;
                    //Console.WriteLine(j.DataObjects[0].Material.Texture.UnkFlags.ToString("x"));
                    foreach (DatDOBJ da in j.DataObjects)
                    {
                        //Console.WriteLine(da.Material.Flags.ToString("x") + " " + (da.Material.Texture_Diffuse == null));
                        //da.Material.Flags = da.Material.Flags & 0xF0 | 0x04;
                        // 0x4 Smooth blending
                        // 0xC hard white light
                        //da.Material.Texture_Diffuse = null;
                        //if (da.Material.Texture_Specular != null)
                        //    da.Material.Texture_Specular.GetBitmap().Save(j.DataObjects.IndexOf(da) + ".png");
                        //da.Material.Texture_Specular = null;
                        //da.Polygons.Clear();
                        //Console.WriteLine((da.Material.Texture!=null) + " " + da.Polygons[0].Flags.ToString("x") + " " + da.Material.Flags.ToString("x"));
                    }
                    //j.DataObjects.Clear();

                    if (!done)
                    {
                        DatDOBJ d = new DatDOBJ();
                        j.DataObjects.RemoveAt(0);
                        d.Node = j;
                        j.DataObjects.Add(d);
                        d.Material.MaterialColor.AMB = Color.AliceBlue;
                        d.Material.Flags = 0xC; //0xC for no texture
                        //d.Material.Texture_Diffuse = new DatTexture();
                        //d.Material.Texture_Diffuse.SetFromBitmap(new Bitmap("C:\\Users\\Owen\\Desktop\\Modlee\\TestTrophy\\roomitems_016_sonic.png"), TPL_TextureFormat.RGBA8);

                        DatPolygon p = new DatPolygon();
                        //p.Flags = 0x0001;
                        p.ParentDOBJ = d;
                        GCX.GXVertex[] Verts = OBJTriangleLoader.GetTrianglesFromFile("C:\\Users\\Owen\\Desktop\\Modlee\\TestTrophy\\ROOMITEMS016_ALLout.obj");
                        for (int i = 0; i < Verts.Length; i++)
                        {
                            Verts[i].Pos.X *= 3;
                            Verts[i].Pos.Y *= 3;
                            Verts[i].Pos.Z *= 3;
                        }
                        GXAttribGroup group = File.Roots[0].Attributes[0];
                        group.Attributes.RemoveAt(0);
                        foreach (GCX.GXAttr g in group.Attributes)
                            Console.WriteLine(g.Name.ToString());
                        //File.Roots[0].Attributes.Clear();
                        //File.Roots[0].Attributes.Add(group);
                        p.Attributes = group;

                        for (int i = 0; i < Verts.Length; i += 3)
                        {
                            GCX.GXVertex[] tri = new GCX.GXVertex[]
                            {
                            Verts[i+2], Verts[i+1], Verts[i]
                            };
                            p.DisplayLists.Add(Comp.CompressDisplayList(tri, p.Attributes, GCX.GXPrimitiveType.Triangles));
                        }

                        done = true;
                    }
                }
            }*/


            /*GXVertexCompressor Comp = new GXVertexCompressor(File);
            VertexBaker v = new VertexBaker();
            v.SetRoot(File);
            //8 and 10 is face
            // 5 and 6 are ears
            int[] NessHead = new int[] { 2, 3, 4, 7,  9,  11,
            20, 21, 24,
            27, 28, 29, 30, 31, 32,
            49, 50,
            61, 62,
            66, 67, 68, 69};
            // 2 3 4 used for Lucas Head
            DatJOBJ HeadBone = File.Roots[0].GetJOBJinOrderBreath()[24];
            int id = 0;
            foreach (DatDOBJ j in File.Roots[0].GetDataObjects())
            {
                j.Material.Flags = 0x14;
                j.Material.Texture_Specular = null;

                GXAttribGroup g = j.Polygons[0].Attributes;

                if (NessHead.Contains(id)) j.Polygons.Clear();

                if (id == 2)
                {
                    j.Material.Texture_Diffuse.ImageData = null;
                    j.Material.Texture_Diffuse.SetFromBitmap(new Bitmap("C:\\Users\\Owen\\Desktop\\Modlee\\LucasImport\\FitLucas_HaiComp.png"), TPL_TextureFormat.RGB565);
                    LoadPolygonFromOBJ("C:\\Users\\Owen\\Desktop\\Modlee\\LucasImport\\Lucas_Hairout.obj", j, g, Comp);
                }
                else
                {
                    foreach (DatPolygon poly in j.Polygons)
                    {
                        List<GCX.GXDisplayList> newDL = new List<GCX.GXDisplayList>();
                        foreach (Modlee.GCX.GXDisplayList d in poly.DisplayLists)
                        {
                            GCX.GXVertex[] vert = v.GetFormattedVertices(d, poly);
                            newDL.Add(Comp.CompressDisplayList(vert, poly.Attributes, d.PrimitiveType));
                        }
                        poly.DisplayLists = newDL;
                    }
                }
                id++;
            }
            File.DataBuffer = Comp.GetBuffer();*/
            //System.IO.File.WriteAllBytes("BufferDump.bin", Comp.GetBuffer());

            //File.PrintTree();
            //Compiler.Compile(File, "C:\\Users\\Owen\\Desktop\\PlLcNr.dat");
            //File = Decompiler.Decompile(System.IO.File.ReadAllBytes("C:\\Users\\Owen\\Desktop\\PlLcNr.dat"));
            ReadyToRender = true;
        }

        public void LoadPolygonFromOBJ(String fname, DatDOBJ Parent, GXAttribGroup group, GXVertexCompressor Comp)
        {
            DatPolygon p = new DatPolygon();
            p.Flags = 0xA001;
            p.ParentDOBJ = Parent;
            
            GXVertex[] Verts = OBJTriangleLoader.GetTrianglesFromFile(fname);

            // single bind fix 24
            Matrix4[] T = DATRenderer.CalculateTransforms(File.Roots[0]);
            Matrix4 bind = T[24].Inverted();

            List<DatBoneWeight> weights = new List<DatBoneWeight>();
            p.BoneWeightList.Add(weights);
            DatBoneWeight w = new DatBoneWeight();
            w.jobj = File.Roots[0].GetJOBJinOrder()[24];
            w.Weight = 1;
            weights.Add(w);

            for(int i =0; i < Verts.Length; i++)
            {
                Vector3 P = Vector3.TransformPosition(new Vector3(Verts[i].Pos.X, Verts[i].Pos.Y, Verts[i].Pos.Z), bind);
                Vector3 N = Vector3.TransformNormal(new Vector3(Verts[i].Nrm.X, Verts[i].Nrm.Y, Verts[i].Nrm.Z), bind);
                Verts[i].Pos.X = P.X * 1.08f;
                Verts[i].Pos.Y = P.Y * 1.045f - 0.5f;
                Verts[i].Pos.Z = P.Z * 1.07f - 0.25f;
                Verts[i].Nrm.X = N.X;
                Verts[i].Nrm.Y = N.Y;
                Verts[i].Nrm.Z = N.Z;
            }
          
            foreach (GXAttr g in group.Attributes)
            {
                Console.WriteLine(g.Name.ToString() + " " + g.CompType + " " + g.CompCount + " " + g.Scale);
            }

            p.AttributeGroup = group;

            for (int i = 0; i < Verts.Length; i += 3)
            {
                GXVertex[] tri = new GXVertex[]
                {
                            Verts[i+2], Verts[i+1], Verts[i]
                };
                p.DisplayLists.Add(Comp.CompressDisplayList(tri, GXPrimitiveType.Triangles, p.AttributeGroup));
            }
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            if (this.IsDisposed)
                return;

            if (ReadyToRender)
            {
                //glViewport.Invalidate();
                Render();
            }
        }

        public void Render()
        {
            glViewport.MakeCurrent();
            GL.Viewport(0, 0, glViewport.Width, glViewport.Height);

            // Bind the default framebuffer in case it was set elsewhere.
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            // Push all attributes so we don't have to clean up later
            GL.PushAttrib(AttribMask.AllAttribBits);
            GL.ClearColor(Color.DarkSlateGray);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
           

            if (glViewport.ClientRectangle.Contains(glViewport.PointToClient(Cursor.Position))
             && glViewport.Focused)
            {
                Camera.Update();
            }
            try
            {
                if (OpenTK.Input.Mouse.GetState() != null)
                    Camera.mouseSLast = OpenTK.Input.Mouse.GetState().WheelPrecise;
            }
            catch
            {
            }

            

            GL.UseProgram(0);
            GL.MatrixMode(MatrixMode.Modelview);
            Matrix4 mat = Camera.mvpMatrix;
            GL.LoadMatrix(ref mat);

            // objects shouldn't show through opaque parts of floor
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);
            //GL.Enable(EnableCap.CullFace);

            DrawTools.DrawFloor(Camera.mvpMatrix);

            //renderer.Render(ref mat, treeView1.Nodes);
            //DATRenderer.Render((DATRoot)File.Nodes[0]);
            foreach(DATRenderer r in Renderers)
            {
                r.Render(ref mat);
            }

            GL.PopAttrib();
            glViewport.SwapBuffers();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Render();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var timer = new Timer();
            timer.Interval = 1000 / 120;
            timer.Tick += new EventHandler(Application_Idle);
            timer.Start();

            Console.WriteLine("loading");
            foreach (DATRoot r in File.Roots)
            {
                if(r.Bones.Count > 1)
                {
                    DATRenderer renderer = new DATRenderer(r);
                    Renderers.Add(renderer);
                }
                if (r.Map_Head != null)
                {
                    foreach (Map_Model_Group g in r.Map_Head.ModelObjects)
                    {
                        DATRenderer renderer = new DATRenderer(g.BoneRoot);
                        Renderers.Add(renderer);
                    }
                }
                foreach(DatNode n in r.ExtraNodes)
                {
                    foreach(DATRoot root in n.GetRoots())
                    {
                        DATRenderer renderer = new DATRenderer(root);
                        Renderers.Add(renderer);
                    }
                }
            }
            Console.WriteLine("loaded");
        }

        private void glViewport_Resize(object sender, EventArgs e)
        {
            Camera.renderWidth = glViewport.Width;
            Camera.renderHeight = glViewport.Height;
            Camera.Update();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            glViewport.Visible = false;
            materialEditor1.Visible = false;
            if (treeView1.SelectedNode != null)
            {
                if (treeView1.SelectedNode.Tag is DatMaterial)
                {
                    materialEditor1.SetMaterial(((DatMaterial)treeView1.SelectedNode.Tag));
                    materialEditor1.Visible = true;
                }
                else
                {
                    glViewport.Visible = true;
                }
            }
        }
    }
}
