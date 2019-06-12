using System;
using System.Drawing;
using System.Windows.Forms;
using HSDLib;
using HSDLib.Common;
using HSDLib.Animation;
using HALSysDATViewer.Nodes;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using HALSysDATViewer.Rendering;
using HALSysDATViewer.Modeling;
using System.IO;

namespace HALSysDATViewer
{
    public partial class Form1 : Form
    {
        public HSDFile HSD;
        public JOBJRenderer Renderer;

        private ContextMenu DOBJ_Context;

        public Camera Camera = new Camera();
        bool ReadyToRender = false;

        private string FileName;

        public Form1()
        {
            InitializeComponent();
            DOBJ_Context = new ContextMenu();

            MenuItem m = new MenuItem("Clear Mesh");
            m.Click += (sender, args) =>
            {
                if (((Node_Generic)nodeTree.SelectedNode).Node is HSD_DOBJ dobj)
                {
                    dobj.POBJ = null;
                    ((Node_Generic)nodeTree.SelectedNode).Refresh();
                }
            };
            DOBJ_Context.MenuItems.Add(m);

            

            nodeTree.MouseDown += (sender, args) => nodeTree.SelectedNode = nodeTree.GetNodeAt(args.X, args.Y);
            nodeTree.NodeMouseClick += (sender, args) =>
            {
                if (nodeTree.SelectedNode != null && args.Button == MouseButtons.Right && ((Node_Generic)nodeTree.SelectedNode).Node is HSD_DOBJ)
                {
                    DOBJ_Context.Show(nodeTree, args.Location);
                }
            };
            ReadyToRender = true;
        }

        public void OpenDAT(string FNAME)
        {
            HSD = new HSDFile(FNAME);
            FileName = FNAME;
            RefreshNodes();
        }

        public void SaveDAT(string FNAME)
        {
            if(HSD != null)
                HSD.Save(FNAME);
        }

        private void RefreshNodes()
        {
            nodeTree.Nodes.Clear();
            nodeTree.BeginUpdate();
            foreach(HSDRoot root in HSD.Roots)
            {
                //FolderNode n = new FolderNode() { Text = root.Name };

                //if (root.Node is HSD_JOBJ)
                //    new Node_JOBJ((HSD_JOBJ)root.Node, n);

                Node_Generic generic = new Node_Generic(root.Node);
                nodeTree.Nodes.Add(generic);
                generic.Open();
            }
            nodeTree.EndUpdate();
            //nodeTree.ExpandAll();
        }

        private void nodeTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if(nodeTree.SelectedNode is IBaseNode n)
            {
            }
            if (nodeTree.SelectedNode is Node_JOBJ)
            {
                HSD_JOBJ j = ((Node_JOBJ)nodeTree.SelectedNode).JOBJ;
                if (j.Flags.HasFlag(JOBJ_FLAG.SKELETON_ROOT))
                    Renderer.RootNode = j;
            }
            if (nodeTree.SelectedNode is Node_Generic)
            {
                IHSDNode Node = ((Node_Generic)nodeTree.SelectedNode).Node;
                if(Node is HSD_JOBJ)
                {
                    HSD_JOBJ j = (HSD_JOBJ)Node;
                    if (j.Flags.HasFlag(JOBJ_FLAG.SKELETON_ROOT) || j.Flags.HasFlag(JOBJ_FLAG.ROOT_OPA))
                        Renderer.RootNode = j;
                }
                if (Node is HSD_FigaTree)
                {
                    Renderer.FigaTree = (HSD_FigaTree)Node;
                    trackBar1.Maximum = (int)((HSD_FigaTree)Node).FrameCount;
                }
                propertyGrid1.SelectedObject = Node;
            }
        }

        private void glControl_Resize(object sender, EventArgs e)
        {
            Camera.renderWidth = glControl.Width;
            Camera.renderHeight = glControl.Height;
            Camera.Update();
        }

        private void glControl_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var timer = new Timer();
            timer.Interval = 1000 / 120;
            timer.Tick += new EventHandler(Application_Idle);
            timer.Start();

            Renderer = new JOBJRenderer();
            //Renderer.SetHSD(smd.RootJOBJ);
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

        private void Render()
        {
            glControl.MakeCurrent();
            GL.Viewport(0, 0, glControl.Width, glControl.Height);

            // Bind the default framebuffer in case it was set elsewhere.
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            // Push all attributes so we don't have to clean up later
            GL.PushAttrib(AttribMask.AllAttribBits);
            GL.ClearColor(Color.DarkSlateGray);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            
            GL.UseProgram(0);
            GL.MatrixMode(MatrixMode.Modelview);
            Matrix4 mat = Camera.mvpMatrix;
            GL.LoadMatrix(ref mat);

            // objects shouldn't show through opaque parts of floor
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);
            //GL.Enable(EnableCap.CullFace);
            //GL.CullFace(CullFaceMode.Front);

            DrawTools.DrawFloor(Camera.mvpMatrix);

            //RenderHere
            Renderer.Render(ref Camera);

            GL.PopAttrib();
            glControl.SwapBuffers();
        }

        private void openDATToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "HSD |*.dat";

                if(ofd.ShowDialog() == DialogResult.OK)
                {
                    OpenDAT(ofd.FileName);
                }
            }
        }

        private void exportDATToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "HSD |*.dat";

                sfd.FileName = System.IO.Path.GetFileName(FileName);

                if(sfd.ShowDialog() == DialogResult.OK)
                {
                    SaveDAT(sfd.FileName);
                }
            }
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            Renderer.SetFrame(trackBar1.Value);
        }


        private void glControl_MouseMove(object sender, MouseEventArgs e)
        {
            Camera.Update();
            try
            {
                if (OpenTK.Input.Mouse.GetState() != null)
                    Camera.mouseSLast = OpenTK.Input.Mouse.GetState().WheelPrecise;
            }
            catch
            {
            }
        }

        private void exportSMDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog d = new SaveFileDialog())
            {
                d.Filter = "Source Model (*.smd)|*.smd";

                if(d.ShowDialog() == DialogResult.OK)
                {
                    ExportSMD(d.FileName, (HSD_JOBJ)((Node_Generic)nodeTree.Nodes[0]).Node);
                }
            }
        }

        private void ExportSMD(string FileName, HSD_JOBJ jobj)
        {
            var bones = ((HSD_JOBJ)HSD.Roots[0].Node).DepthFirstList;

            Console.WriteLine(bones.Count);

            using (StreamWriter w = new StreamWriter(new FileStream(FileName, FileMode.Create)))
            {
                w.WriteLine("#version 1");

                w.WriteLine("nodes");
                var index = 0;
                foreach (var j in bones)
                {
                    int parentIndex = -1;
                    foreach (var n in bones)
                        foreach (var child in n.Children)
                            if (child == j)
                                parentIndex = bones.IndexOf(n);
                    w.WriteLine($"{index} \"JOBJ_{index}\" {parentIndex}");
                    index++;
                }
                w.WriteLine("end");
                w.WriteLine("skeleton");
                index = 0;
                foreach (var j in bones)
                {
                    w.WriteLine($"{index} {j.Transforms.TX} {j.Transforms.TY} {j.Transforms.TZ} {j.Transforms.RX} {j.Transforms.RY} {j.Transforms.RZ}");
                    index++;
                }
                w.WriteLine("end");
            }

        }
    }
}
